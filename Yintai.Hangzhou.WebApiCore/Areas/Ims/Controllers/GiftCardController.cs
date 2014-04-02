using com.intime.fashion.common.Extension;
using com.intime.fashion.common.IT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;
using Yintai.Architecture.Common.Data.EF;
using Yintai.Hangzhou.Contract.DTO.Request;
using Yintai.Hangzhou.Contract.DTO.Response;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Repository.Contract;
using Yintai.Hangzhou.WebSupport.Mvc;

namespace Yintai.Hangzhou.WebApiCore.Areas.Ims.Controllers
{
    public class GiftCardController : RestfulController
    {
        private static string GIFTCARD_STORE_ID = "1000000";
        private ICustomerRepository _customerRepo;
        private IEFRepository<ResourceEntity> _resourceRepo;
        private IEFRepository<OrderTransactionEntity> _orderTranRepo;
        private IEFRepository<IMS_GiftCardEntity> _cardRepo;
        private IEFRepository<IMS_GiftCardOrderEntity> _orderRepo;
        private IEFRepository<IMS_GiftCardTransfersEntity> _transRepo;
        private IEFRepository<IMS_GiftCardUserEntity> _userRepo;
        private IEFRepository<IMS_GiftCardItemEntity> _itemRepo;
        private IEFRepository<IMS_GiftCardRechargeEntity> _rechargeRepo;

        public GiftCardController(
            ICustomerRepository customerRepo, 
            IEFRepository<ResourceEntity> resourceRepo, 
            IEFRepository<OrderTransactionEntity> orderTranRepo,
            IEFRepository<IMS_GiftCardEntity> cardRepo,
            IEFRepository<IMS_GiftCardOrderEntity> orderRepo,
            IEFRepository<IMS_GiftCardTransfersEntity> transRepo,
            IEFRepository<IMS_GiftCardUserEntity> userRepo,
            IEFRepository<IMS_GiftCardItemEntity> itemRepo,
            IEFRepository<IMS_GiftCardRechargeEntity> rechargeRepo)
        {
            this._customerRepo = customerRepo;
            this._resourceRepo = resourceRepo;
            this._orderRepo = orderRepo;
            this._transRepo = transRepo;
            this._orderTranRepo = orderTranRepo;
            this._cardRepo = cardRepo;
            this._userRepo = userRepo;
            this._itemRepo = itemRepo;
            this._rechargeRepo = rechargeRepo;
        }

        [RestfulAuthorize]
        public ActionResult IsBind(string phone, int authuid)
        {
            var is_binded = this.IsPhoneBinded(phone);
            if (is_binded)
            {
                var user = _customerRepo.GetItem(authuid);
                _userRepo.Insert(new IMS_GiftCardUserEntity()
                {
                    UserId = authuid,
                    GiftCardAccount = phone,
                    CreateDate = DateTime.Now,
                    Name = user.Name,
                    CreateUser = authuid
                });
            }
            return this.RenderSuccess<dynamic>(c => c.Data = new { is_binded });
        }

        [RestfulAuthorize]
        public ActionResult Bind(string phone, int authuid)
        {
            try
            {
                var cardAccount = _userRepo.Find(x => x.GiftCardAccount == phone);
                if (cardAccount != null)
                {
                    if (cardAccount.UserId == authuid)
                    {
                        return this.RenderError(x => x.Message = "You have bond the phone!");
                    }
                    return this.RenderError(x => x.Message = "The phone have been bound by some others.");
                }
                if (IsPhoneBinded(phone))
                {
                    var user = _customerRepo.GetItem(authuid);
                    _userRepo.Insert(new IMS_GiftCardUserEntity()
                    {
                        UserId = authuid,
                        GiftCardAccount = phone,
                        CreateDate = DateTime.Now,
                        Name = user.Name,
                        CreateUser = authuid
                    });
                    try
                    {
                        var balance = GetUserAccountBalance(phone);
                        return this.RenderSuccess<dynamic>(c => c.Data = new
                        {
                            amount = balance,
                            phone
                        });

                    }
                    catch (GetUserBalanceExcepiton ex)
                    {
                        Logger.Error(ex);
                        return this.RenderError(r => r.Message = ex.Message);
                    }
                }
                return this.RenderError(r => r.Message = "This phone has't been bond, please register it.");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return this.RenderError(r => r.Message = "Error occurred while bind user!");
            }
        }

        [RestfulAuthorize]
        public ActionResult My(int authuid)
        {
            var cardAccount = _userRepo.Get(t => t.UserId == authuid).FirstOrDefault();

            if (cardAccount == null)
            {
                return this.RenderError(r => r.Message = "未绑定充值卡账号，请先绑定!");
            }

            try
            {
                var balance = GetUserAccountBalance(cardAccount.GiftCardAccount);
                return this.RenderSuccess<dynamic>(c => c.Data = new
                {
                    is_binded = true,
                    amount = balance,
                    phone = cardAccount.GiftCardAccount
                });

            }
            catch (GetUserBalanceExcepiton ex)
            {
                Logger.Error(ex);
                return this.RenderError(r=>r.Message = ex.Message);
            }

        }

        [RestfulAuthorize]
        public ActionResult Create(string phone, string pwd, string charge_no, string identity_no, int authuid)
        {
            var cardAccount = _userRepo.Find(x => x.GiftCardAccount == phone && x.UserId == authuid);
            if (cardAccount == null)
            {
                return this.RenderError(r => r.Message = "用户未绑定手机，请先绑定手机!");
            }

            IMS_GiftCardOrderEntity giftCardOrder = _orderRepo.Find(t => t.No == charge_no);
            if (giftCardOrder == null)
            {
                return this.RenderError(r => r.Message = "不存在该礼品卡!");
            }

            if (_rechargeRepo.Find(x => x.OrderNo == giftCardOrder.No) != null)
            {
                return this.RenderError(r => r.Message = "该礼品卡已充值!");
            }

            var orderItem = _itemRepo.Find(t => t.GiftCardId == giftCardOrder.GiftCardItemId);

            if (orderItem == null)
            {
                return this.RenderError(r => r.Message = "不存在该礼品卡!");
            }

            OrderTransactionEntity orderTran = _orderTranRepo.Find(t => t.OrderNo == giftCardOrder.No);

            if (orderTran == null)
            {
                return this.RenderError(r => r.Message = "礼品卡没有支付信息!");
            }

            try
            {
                dynamic rechargeRsp = null;
#if !DEBUG
                bool result =
                    !ITServiceHelper.SendHttpMessage(
                        new Request(
                            new
                            {
                                phone,
                                idcard = identity_no,
                                amount = orderTran.Amount,
                                discount = giftCardOrder.Amount - orderTran.Amount,
                                transcode = orderTran.TransNo,
                                totalpay = orderTran.Amount,
                                storeid = GIFTCARD_STORE_ID,
                                password = pwd
                            }), r => rechargeRsp = r, null);
                if (!result || rechargeRsp == null)
                {
                    return this.RenderError(r => r.Message = "充值失败，请稍候重试.");
                }

                if (rechargeRsp.code != "200")
                {
                    return this.RenderError(r => r.Message = rechargeRsp.message);
                }
#endif
                using (var ts = new TransactionScope())
                {
                    giftCardOrder.Status = (int)GiftCardOrderStatus.Recharge;
                    giftCardOrder.OwnerUserId = authuid;
                    _orderRepo.Update(giftCardOrder);
                    _rechargeRepo.Insert(new IMS_GiftCardRechargeEntity()
                    {
                        OrderNo = giftCardOrder.No,
                        ChargePhone = phone,
                        ChargeUserId = authuid,
                        CreateDate = DateTime.Now,
                        CreateUser = authuid
                    });
                    ts.Complete();
                }

                return this.RenderSuccess<dynamic>(null);
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Recharge eorror, card no :({0}), phone ({1})", charge_no, phone));
                Logger.Error(ex);
                return this.RenderError(r => r.Message = ex.Message);
            }
        }

        [RestfulAuthorize]
        public ActionResult Recharge(string charge_no, int authuid)
        {
            Logger.Error(string.Format("Recharge orderno ({0})",charge_no));
            var cardUser = _userRepo.Find(x => x.UserId == authuid);

            if (cardUser == null)
            {
                return this.RenderError(r => r.Message = "用户未绑定手机账号!");
            }

            IMS_GiftCardOrderEntity order = _orderRepo.Find(x => x.No == charge_no);
            if (order == null)
            {
                return this.RenderError(r => r.Message = string.Format("无效的充值卡编码： ({0})", charge_no));
            }

            if (order.Status == (int)GiftCardOrderStatus.Recharge)
            {
                return this.RenderError(r => r.Message = "礼品卡已经充值，不能重复充值");
            }

            var orderItem = _itemRepo.Find(order.GiftCardItemId);

            if (orderItem == null)
            {
                return this.RenderError(r => r.Message = "不存在的礼品卡!");
            }

            OrderTransactionEntity orderTran = _orderTranRepo.Find(x => x.OrderNo == order.No);
            if (orderTran == null)
            {
                return this.RenderError(r => r.Message = "礼品卡无支付信息!");
            }
            try
            {
#if !DEBUG

                dynamic rechargeRsp = null;
                bool result =
                    !ITServiceHelper.SendHttpMessage(
                        new Request(
                            new
                            {
                                phone = cardUser.GiftCardAccount,
                                amount = orderTran.Amount,
                                discount = order.Amount - orderTran.Amount,
                                transcode = orderTran.TransNo,
                                totalpay = orderTran.Amount,
                                storeid = GIFTCARD_STORE_ID
                            }), r => rechargeRsp = r, null);
                if (!result || rechargeRsp == null)
                {
                    return this.RenderError(r => r.Message = "充值失败，请稍候重试.");
                }

                if (rechargeRsp.code != "200")
                {
                    return this.RenderError(r => r.Message = rechargeRsp.message);
                }
#endif
                var transfer = _transRepo.Find(x => x.IsDecline == 0 && x.IsActive == 0);

                using (var ts = new TransactionScope())
                {
                    order.Status = (int)GiftCardOrderStatus.Recharge;
                    order.OwnerUserId = authuid;
                    _orderRepo.Update(order);
                    _rechargeRepo.Insert(new IMS_GiftCardRechargeEntity()
                    {
                        OrderNo = order.No,
                        ChargePhone = cardUser.GiftCardAccount,
                        ChargeUserId = authuid,
                        CreateDate = DateTime.Now,
                        CreateUser = authuid
                    });

                    if (transfer != null)
                    {
                        transfer.IsActive = 1;
                        transfer.OperateDate = DateTime.Now;
                        transfer.OperateUser = authuid;
                        transfer.ToUserId = authuid;
                        _transRepo.Update(transfer);
                    }

                    ts.Complete();
                }

                return this.RenderSuccess<dynamic>(null);
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Recharge eorror, card no :({0})", charge_no));
                Logger.Error(ex);
                return this.RenderError(r => r.Message = ex.Message);
            }
        }

        [RestfulAuthorize]
        public ActionResult Transfer_Detail(string charge_no, int authuid)
        {
            var trans =
                _transRepo.Get(x => x.OrderNo == charge_no && x.ToUserId == authuid)
                    .OrderByDescending(t => t.CreateDate)
                    .FirstOrDefault();
            if (trans == null)
            {
                return this.RenderError(r => r.Message = "礼品卡没有赠送信息！");
            }

            var user = _customerRepo.GetItem(trans.FromUserId);
            if (user == null)
            {
                return this.RenderError(r => r.Message = "无效的用户!");
            }

            var order = _orderRepo.Find(x => x.No == charge_no);
            if (order == null)
            {
                return this.RenderError(r => r.Message = "无效的礼品卡");
            }

            return
                this.RenderSuccess<dynamic>(
                    c =>
                        c.Data =
                            new
                            {
                                comment = trans.Comment,
                                sender = user.Name,
                                phone = trans.Phone,
                                amount = order.Price,
                                status = (int)this.SetStatus4Receiver(order,trans)
                            });
        }

        [RestfulAuthorize]
        public ActionResult Items(IMSGiftcardListRequest request, int id, int authuid)
        {
            var card = _cardRepo.Find(id);
            if (card == null)
            {
                return this.RenderError(r => r.Message = "Can't find card items!");
            }

            var pageSize = request.Pagesize;
            var page = request.Page;

            var items = _itemRepo.Get(x => x.GiftCardId == id).Skip((page - 1) * pageSize).Take(pageSize);

            var data = new List<dynamic>();
            foreach (var item in items)
            {
                data.Add(new { id = item.Id, unit_price = item.UnitPrice, price = item.Price });
            }

            var resource = _resourceRepo.Find(x => x.Type == (int) SourceType.GiftCard && x.SourceId == card.Id);

            return
                this.RenderSuccess<dynamic>(
                    c =>
                        c.Data =
                            new
                            {
                                id,
                                image = resource == null ? "" : resource.Name.Image320Url(),
                                desc = card.Name,
                                items = data
                            });
        }
        [RestfulAuthorize]
        public ActionResult List(IMSGiftcardListRequest request, int type, int authuid)
        {
            PagerInfoResponse<dynamic> rsp;
            rsp = type == 1 ? ListItemsBuyed(request, authuid) : ListItemsReceived(request, authuid);
            return this.RenderSuccess<PagerInfoResponse<dynamic>>(c=>c.Data = rsp);
        }

        [RestfulAuthorize]
        public ActionResult Send(string charge_no, string comment, string phone, int authuid)
        {
            var order = _orderRepo.Find(x => x.No == charge_no);
            if (order.Status != (int)GiftCardOrderStatus.Create)
            {
                return this.RenderError(r => r.Message = "该礼品卡已经充值了，不能赠送");
            }

            if (_transRepo.Get(x => x.OrderNo == charge_no && x.IsDecline == 0 && x.IsActive == 0 && x.FromUserId == authuid).Any())
            {
                return this.RenderError(r => r.Message = "您已经赠送了此礼品卡，请通知好友领取！");
            }

            var preTran = _transRepo.Find(x => x.OrderNo == charge_no && x.IsActive == 0 && x.IsDecline == 0);

            using (var ts = new TransactionScope())
            {
                if (preTran != null)
                {
                    preTran.IsActive = 1;
                    preTran.ToUserId = authuid;
                    preTran.OperateDate = DateTime.Now;
                    preTran.OperateUser = authuid;
                    _transRepo.Update(preTran);
                }
                _transRepo.Insert(new IMS_GiftCardTransfersEntity()
                {
                    Phone = phone,
                    Comment = comment,
                    CreateDate = DateTime.Now,
                    CreateUser = authuid,
                    FromUserId = authuid,
                    OrderNo = order.No,
                    IsActive = 0,
                    IsDecline = 0,
                    PreTransferId = preTran == null ? 0 : preTran.Id,
                    OperateDate = DateTime.Now,
                    OperateUser = authuid,
                });
                ts.Complete();
            }

            return this.RenderSuccess<dynamic>(null);
        }

        [RestfulAuthorize]
        public ActionResult Change_Pwd(string pwd_old, string pwd_new, int authuid)
        {
            var cardAccount = _userRepo.Find(x => x.UserId == authuid);
            if (cardAccount == null)
            {
                return this.RenderError(r => r.Message = "该用户未绑定储值卡.");
            }
#if !DEBUG
            dynamic rsp = null;
            if (
                !ITServiceHelper.SendHttpMessage(
                    new Request(new { phone = cardAccount.GiftCardAccount, oldpassword = pwd_old, newpassword = pwd_new }), r => rsp = r, null) ||
                rsp == null)
            {

                return this.RenderError(r => r.Message = "修改密码失败，请稍候重试.");
            }
            if (rsp.code != 200)
            {
                return this.RenderError(r => r.Message = rsp.message);
            }
#endif
            return this.RenderSuccess<dynamic>(null);
        }

        [RestfulAuthorize]
        public ActionResult Reset_Pwd(string pwd_new, int authuid)
        {
            var cardAccount = _userRepo.Find(x => x.UserId == authuid);
            if (cardAccount == null)
            {
                return this.RenderError(r => r.Message = "该用户未绑定储值卡!");
            }
#if !DEBUG
            dynamic rsp = null;
            if (
                !ITServiceHelper.SendHttpMessage(
                    new Request(new { phone = cardAccount.GiftCardAccount, newpassword = pwd_new }), r => rsp = r, null) ||
                rsp == null)
            {

                return this.RenderError(r => r.Message = "充值密码失败，请稍候重试.");
            }
            if (rsp.code != 200)
            {
                return this.RenderError(r => r.Message = rsp.message);
            }
#endif
            return this.RenderSuccess<dynamic>(null);
        }
        [RestfulAuthorize]
        public ActionResult Refuse(string charge_no, int authuid)
        {
            var trans = _transRepo.Find(x => x.OrderNo == charge_no && x.IsDecline == 0 && x.IsActive == 0);
            if (trans != null)
            {
                trans.IsDecline = 1;
                trans.ToUserId = authuid;
                trans.OperateDate = DateTime.Now;
                trans.OperateUser = authuid;
                _transRepo.Update(trans);
            }
            return this.RenderSuccess<dynamic>(null);
        }

        private bool IsPhoneBinded(string phone)
        {
#if DEBUG
            return true;
#endif
            dynamic bindRsp = null;
            if (!ITServiceHelper.SendHttpMessage(new Request(new { phone }), r => bindRsp = r, null) || bindRsp == null)
            {
                return false;
            }
            return bindRsp.Data.result == 1;
        }

        private PagerInfoResponse<dynamic> ListItemsReceived(IMSGiftcardListRequest request, int authuid)
        {
            var count = _transRepo.Get(x => x.ToUserId == authuid).Count();

            var orders =
                _transRepo.Get(x => x.ToUserId == authuid)
                    .OrderByDescending(t => t.CreateDate)
                    .Skip((request.Page - 1) * request.Pagesize)
                    .Take(request.Pagesize)
                    .Join(_orderRepo.Get(o => o.Status != (int)GiftCardOrderStatus.Void), x => x.OrderNo, o => o.No,
                        (x, o) => new { transfer = x, order = o })
                    .Join(_customerRepo.GetAll(), t => t.transfer.FromUserId, u => u.Id,
                        (o, u) => new { o.order, o.transfer, user = u });
            dynamic items = new List<dynamic>();
            foreach (var o in orders)
            {
                items.Add(new
                {
                    from = o.user.Nickname,
                    card_no = o.order.No,
                    amount = o.order.Amount,
                    status_i = SetStatus4Receiver(o.order,o.transfer),
                    operation_date = o.transfer.OperateDate.ToString("yyyy-MM-dd hh:mm:ss")
                });
            }
            return new PagerInfoResponse<dynamic>(request.PagerRequest,count){Items=items};

        }

        private GiftCardListItemStatus SetStatus4Receiver(IMS_GiftCardOrderEntity order, IMS_GiftCardTransfersEntity transfers)
        {
            if (transfers.IsDecline == 1)
            {
                return GiftCardListItemStatus.Refuse;
            }
            if (transfers.IsActive == 1)
            {
                if (_transRepo.Get(x => x.FromUserId == transfers.ToUserId).Any())
                {
                    return GiftCardListItemStatus.ReTransfer;
                }
                if (order.Status == (int) GiftCardOrderStatus.Recharge)
                {
                    return GiftCardListItemStatus.Received;
                }
            }
            return GiftCardListItemStatus.None;
        }

        private PagerInfoResponse<dynamic> ListItemsBuyed(IMSGiftcardListRequest request, int authuid)
        {
            int page = request.Page;
            int pageSize = request.Pagesize;
            var count = _orderRepo.Get(x => x.PurchaseUserId == authuid).Count();
            var orders =
                _orderRepo.Get(x => x.PurchaseUserId == authuid)
                    .OrderByDescending(t => t.CreateDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .GroupJoin(_transRepo.Get(x => x.FromUserId == authuid), o => o.No, x => x.OrderNo,
                        (o, trans) =>
                            new { order = o, tran = trans.OrderByDescending(x => x.CreateDate).FirstOrDefault() })
                    .GroupJoin(_rechargeRepo.Get(r => r.ChargeUserId == authuid), ot => ot.order.No, r => r.OrderNo,
                        (ot, r) => new { order = ot.order, ransfer = ot.tran, recharge = r.FirstOrDefault() });


            var items = new List<dynamic>();
            foreach (var o in orders)
            {
                items.Add(new
                {
                    card_no = o.order.No,
                    amount = o.order.Amount,
                    purchase_date = o.order.CreateDate,
                    charge_date = o.recharge != null ? o.recharge.CreateDate.ToString("yyyy-MM-dd hh:mm:ss") : "null",
                    status_i = SetStatusForBuyer(o.order, o.ransfer, o.recharge),
                    verify_phone = o.recharge != null ? o.recharge.ChargePhone : "null",
                    send_date = o.ransfer != null ? o.ransfer.CreateDate.ToString("yyyy-MM-dd hh:mm:ss") : "null",
                    //receive_date = o.ransfer != null && o.ransfer.IsActive == 1 ? o.ransfer.CreateDate.ToString("yyyy-MM-dd hh:mm:ss") : "null"
                });
            }
            return new PagerInfoResponse<dynamic>(request.PagerRequest, count) { Items  = items};
        }

        private GiftCardListItemStatus SetStatusForBuyer(IMS_GiftCardOrderEntity order, IMS_GiftCardTransfersEntity transfer, IMS_GiftCardRechargeEntity recharge)
        {
            if (transfer != null)
            {
                if (transfer.IsDecline == 1)
                {
                    return GiftCardListItemStatus.Refuse;
                }
                return GiftCardListItemStatus.Sent;
            }
            if (recharge == null)
            {
                return GiftCardListItemStatus.None;
            }
            return GiftCardListItemStatus.Recharged;
        }


        private dynamic GetUserAccountBalance(string phone)
        {
#if DEBUG
            return new Random(5000).NextDouble();
#endif
            dynamic balanceRsp = null;
            if (
                !ITServiceHelper.SendHttpMessage(new Request(new { phone }),
                    r => balanceRsp = r.Data, null) || balanceRsp == null || balanceRsp.Code != 200)
            {
                throw new GetUserBalanceExcepiton("获取账号余额失败，请稍候重试");
            }
            return balanceRsp.Data.balance;
        }
    }

    public class GetUserBalanceExcepiton : Exception
    {
        public GetUserBalanceExcepiton(string message) : base(message)
        {
        }
    }

    public enum GiftCardListItemStatus
    {
        /// <summary>
        /// 未操作
        /// </summary>
        None = 0,

        /// <summary>
        /// 已领取
        /// </summary>
        Received = 1,

        /// <summary>
        /// 已拒收
        /// </summary>
        Refuse = 2,

        /// <summary>
        /// 已转赠
        /// </summary>
        ReTransfer = 3,

        /// <summary>
        /// 已充值
        /// </summary>
        Recharged = 4,

        /// <summary>
        /// 已赠送
        /// </summary>
        Sent = 5,
    }

    public enum GiftCardOrderStatus
    {
        /// <summary>
        /// 取消
        /// </summary>
        Void = -10,

        /// <summary>
        /// 创建
        /// </summary>
        Create = 0,

        /// <summary>
        /// 充值
        /// </summary>
        Recharge = 10,
    }
}
