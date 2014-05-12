using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Yintai.Architecture.Common.Data.EF;
using Yintai.Hangzhou.Contract.Customer;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Repository.Contract;
using System.Data.Entity;
using Yintai.Architecture.Framework.ServiceLocation;
using System.Transactions;
using Newtonsoft.Json;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Contract.DTO.Request;
using Yintai.Hangzhou.Contract.DTO.Response;
using com.intime.fashion.common.Wxpay;
using Yintai.Hangzhou.Contract.DTO.Request.Customer;
using Yintai.Hangzhou.Service.Logic;
using Yintai.Hangzhou.Model;
using com.intime.fashion.common;
using Yintai.Architecture.Common.Logger;

namespace Yintai.Hangzhou.WebApiCore.Areas.Ims.Controllers
{
    [ValidateInput(false)]
    public class PaymentController : Controller
    {
        private IEFRepository<OrderTransactionEntity> _orderTranRepo;
        private IEFRepository<PaymentNotifyLogEntity> _paymentNotifyRepo;
        private IOrderRepository _orderRepo;
        private IOrderLogRepository _orderlogRepo;
        private IEFRepository<ExOrderEntity> _exorderRepo;
        private Architecture.Common.Data.EF.IEFRepository<Data.Models.IMS_GiftCardOrderEntity> _giftcardOrderRepo;
        private IEFRepository<IMS_AssociateIncomeHistoryEntity> _incomehisRepo;

        public PaymentController(IEFRepository<OrderTransactionEntity> ordTranRepo
            , IEFRepository<PaymentNotifyLogEntity> paynotiRepo
            , IOrderRepository orderRepo
            , IOrderLogRepository orderLogRepo
            , IEFRepository<ExOrderEntity> exorderRepo
            , IEFRepository<IMS_GiftCardOrderEntity> giftcardOrderRepo,
            IEFRepository<IMS_AssociateIncomeHistoryEntity> incomehisRepo)
        {
            _orderTranRepo = ordTranRepo;
            _paymentNotifyRepo = paynotiRepo;
            _orderRepo = orderRepo;
            _orderlogRepo = orderLogRepo;

            _exorderRepo = exorderRepo;
            _giftcardOrderRepo = giftcardOrderRepo;
            _incomehisRepo = incomehisRepo;
        }


        public ActionResult Notify(WxNotifyPostRequest request)
        {
            Dictionary<string, string> sPara = GetQueryStringParams();

            if (sPara.Count > 0)
            {
                var requestSign = sPara["sign"];
                sPara.Remove("sign");
                var notifySigned = Util.NotifySignIMS(sPara);
                if (string.Compare(requestSign, notifySigned, true) != 0)
                    return Content("fail");
                //external order no
                string out_trade_no = sPara["out_trade_no"];
                string trade_no = sPara["transaction_id"];
                int trade_status = int.Parse(sPara["trade_state"]);
                string bank_bill_no = sPara["bank_billno"];
                var amount = decimal.Parse(sPara["total_fee"]) / 100;
                if (trade_status == 0)
                {
                    var orderEntity = Context.Set<OrderEntity>().Where(o => o.OrderNo == out_trade_no).FirstOrDefault();
                    if (orderEntity == null)
                        return Content("fail");
                    var orderType = (orderEntity.OrderProductType ?? (int)OrderProductType.SystemProduct) == (int)OrderProductType.SelfProduct ? (int)PaidOrderType.Self_ProductOfSelf : (int)PaidOrderType.Self;

                    var paymentEntity = Context.Set<OrderTransactionEntity>().Where(p => p.OrderNo == orderEntity.OrderNo
                                                && p.PaymentCode == WxPayConfig.PAYMENT_CODE4IMS
                                                && (!p.OrderType.HasValue || p.OrderType.Value == orderType)).FirstOrDefault();

                    if (paymentEntity == null)
                    {
                        OrderTransactionEntity orderTransaction = null;
                        using (var ts = new TransactionScope())
                        {
                            _paymentNotifyRepo.Insert(new PaymentNotifyLogEntity()
                            {
                                CreateDate = DateTime.Now,
                                OrderNo = orderEntity.OrderNo,
                                PaymentCode = WxPayConfig.PAYMENT_CODE4IMS,
                                PaymentContent = JsonConvert.SerializeObject(sPara)
                            });

                            orderTransaction = _orderTranRepo.Insert(new OrderTransactionEntity()
                            {
                                Amount = amount,
                                OrderNo = orderEntity.OrderNo,
                                CreateDate = DateTime.Now,
                                IsSynced = false,
                                PaymentCode = WxPayConfig.PAYMENT_CODE4IMS,
                                TransNo = trade_no,
                                OutsiteType = (int)OutsiteType.WX,
                                OutsiteUId = request.OpenId,
                                OrderType = orderType
                            });
                            if (orderEntity.Status != (int)OrderStatus.Paid && orderEntity.TotalAmount <= amount)
                            {

                                orderEntity.Status = (int)OrderStatus.Paid;
                                orderEntity.UpdateDate = DateTime.Now;
                                orderEntity.RecAmount = amount;
                                _orderRepo.Update(orderEntity);

                                _orderlogRepo.Insert(new OrderLogEntity()
                                {
                                    CreateDate = DateTime.Now,
                                    CreateUser = 0,
                                    CustomerId = orderEntity.CustomerId,
                                    Operation = "支付订单",
                                    OrderNo = orderEntity.OrderNo,
                                    Type = (int)OrderOpera.CustomerPay
                                });

                                AssociateIncomeLogic.Froze(orderEntity.OrderNo);

                            }

                            bool isSuccess = true;

                            if (isSuccess)
                            {
                                ts.Complete();
                                //notify wx async
                                NotifyWxAsync(AssociateOrderType.Product, orderEntity.OrderNo, request.OpenId);
                            }
                            else
                                return Content("fail");
                        }
                       
                    }

                }
                return Content("success");

            }
            else
            {
                return Content("fail");
            }
        }
        public ActionResult Notify_GiftCard(WxNotifyPostRequest request)
        {
            Dictionary<string, string> sPara = GetQueryStringParams();

            if (sPara.Count > 0)
            {
                var requestSign = sPara["sign"];
                sPara.Remove("sign");
                var notifySigned = Util.NotifySignIMS(sPara);

                if (string.Compare(requestSign, notifySigned, true) != 0)
                    return Content("fail");
                //external order no
                string out_trade_no = sPara["out_trade_no"];
                string trade_no = sPara["transaction_id"];
                int trade_status = int.Parse(sPara["trade_state"]);
                string bank_bill_no = sPara["bank_billno"];
                var amount = decimal.Parse(sPara["total_fee"]) / 100;

                if (trade_status == 0)
                {
                    var tradeNos = out_trade_no.Split('-');
                    if (tradeNos.Length < 3)
                    {
                        Log.Error(string.Format("order paid, but trade_no:{0} not follow the correct format", out_trade_no));
                        return Content("fail");
                    }
                    var trackOrderSource = false;
                    if (tradeNos.Length >= 4)
                        trackOrderSource = true;
                    var giftcardId = int.Parse(tradeNos[1]);
                    var orderUserId = int.Parse(tradeNos[2]);
                    var giftCardEntity = Context.Set<IMS_GiftCardItemEntity>().Find(giftcardId);
                    if (null == giftCardEntity)
                    {
                        Log.Error(string.Format("order paid, but gift card id:{0} not exists", giftcardId));
                        return Content("success");
                    }
                    if (giftCardEntity.Price > amount)
                    {
                        Log.Error(string.Format("order paid, but gift card price:{0} not match paid amount:{1}", giftCardEntity.Price, amount));
                        return Content("success");
                    }
                    var paymentEntity = Context.Set<OrderTransactionEntity>().Where(p => p.TransNo == trade_no
                                                && p.PaymentCode == WxPayConfig.PAYMENT_CODE4GIFTCARD
                                                && (!p.OrderType.HasValue || p.OrderType.Value == (int)PaidOrderType.GiftCard)).FirstOrDefault();

                    if (paymentEntity == null)
                    {
                        OrderTransactionEntity orderTransaction = null;
                        using (var ts = new TransactionScope())
                        {
                            var giftcardOrder = _giftcardOrderRepo.Insert(new IMS_GiftCardOrderEntity()
                            {
                                Amount = giftCardEntity.UnitPrice,
                                Price = giftCardEntity.Price,
                                CreateDate = DateTime.Now,
                                CreateUser = orderUserId,
                                GiftCardItemId = giftcardId,
                                OwnerUserId = orderUserId,
                                PurchaseUserId = orderUserId,
                                Status = (int)DataStatus.Normal,
                                No = OrderRule.CreateGiftCardNo()
                            });

                            _paymentNotifyRepo.Insert(new PaymentNotifyLogEntity()
                            {
                                CreateDate = DateTime.Now,
                                OrderNo = giftcardOrder.No,
                                PaymentCode = WxPayConfig.PAYMENT_CODE4GIFTCARD,
                                PaymentContent = JsonConvert.SerializeObject(sPara)
                            });

                            orderTransaction = _orderTranRepo.Insert(new OrderTransactionEntity()
                            {
                                Amount = amount,
                                OrderNo = giftcardOrder.No,
                                CreateDate = DateTime.Now,
                                IsSynced = false,
                                PaymentCode = WxPayConfig.PAYMENT_CODE4GIFTCARD,
                                TransNo = trade_no,
                                OutsiteType = (int)OutsiteType.WX,
                                OutsiteUId = request.OpenId,
                                OrderType = (int)PaidOrderType.GiftCard
                            });

                            if (trackOrderSource)
                            {
                                int assocateId = int.Parse(tradeNos[3]);
                                var associatEntity = Context.Set<IMS_AssociateEntity>().Find(assocateId);
                                if (associatEntity != null)
                                {
                                    AssociateIncomeLogic.Avail(associatEntity.UserId, giftcardOrder);
                                }
                            }
                            ts.Complete();
                            //notify wx async
                            NotifyWxAsync(AssociateOrderType.GiftCard, giftcardOrder.No, request.OpenId);
                        }
                    }

                }
                return Content("success");

            }
            else
            {
                return Content("fail");
            }
        }

        private Dictionary<string, string> GetQueryStringParams()
        {
            var requestParams = new Dictionary<string, string>();
            var unValidRequest = Request.Unvalidated;
            foreach (var key in unValidRequest.QueryString.AllKeys)
            {
                if (string.IsNullOrEmpty(unValidRequest.QueryString[key]))
                    continue;
                requestParams.Add(key.ToLower(), unValidRequest.QueryString[key]);
            }
            return requestParams;
        }
        private DbContext Context
        {
            get
            {
                return ServiceLocator.Current.Resolve<DbContext>();
            }
        }
        private ILog Log
        {
            get
            {
                return ServiceLocator.Current.Resolve<ILog>();
            }
        }

        private void NotifyWxAsync(AssociateOrderType type, string orderNo,string openId)
        {
            Task.Factory.StartNew(() =>
            {
                //step1: send paid message to user
                var order_type = string.Empty;
                var url_path = string.Empty;
                var daogou_url_path = "ims/store/stores/records";
                switch (type)
                { 
                    case AssociateOrderType.Product:
                        order_type="商品";
                        url_path = string.Format("ims/orders/{1}",orderNo);
                        break;
                    case AssociateOrderType.GiftCard:
                        order_type = "礼品卡";
                        url_path = "ims/card_orders/list";
                        break;
                    default:
                        return;
                }
                WxServiceHelper.SendMessage(new
                {
                    touser = openId,
                    template_id = WxPayConfig.MESSAGE_TEMPLATE_ID_MINI,
                    url = string.Format("{0}{1}", ConfigManager.AwsHost, url_path),
                    topcolor = "#FF0000",
                    data = new
                    {
                        title = new { value = "您的订单已支付", color = "#000000" },
                        storeName = new { value = "银泰商业", color = "#173177" },
                        orderId = new { value = orderNo, color = "#173177" },
                        orderType = new { value = order_type, color = "#173177" },
                        remark = new { value = "感谢支持，点击进入查看订单详情", color = "#173177" }
                    }
                }, null, null);
                //step2: send income message to dagou
                var associateOrder = Context.Set<IMS_AssociateIncomeHistoryEntity>().Where(iai => iai.SourceNo == orderNo && iai.SourceType == (int)type).FirstOrDefault();
                if (associateOrder != null)
                {
                    var userEntity = Context.Set<OutsiteUserEntity>().Where(o => o.AssociateUserId == associateOrder.AssociateUserId).First();
                    WxServiceHelper.SendMessage(new
                    {
                        touser = userEntity.OutsiteUserId,
                        template_id = WxPayConfig.MESSAGE_TEMPLATE_ID_MINI,
                        url = string.Format("{0}{1}", ConfigManager.AwsHost, daogou_url_path),
                        topcolor = "#FF0000",
                        data = new
                        {
                            title = new { value = "您的店铺有订单已支付", color = "#000000" },
                            storeName = new { value = "银泰商业", color = "#173177" },
                            orderId = new { value = orderNo, color = "#173177" },
                            orderType = new { value = order_type, color = "#173177" },
                            remark = new { value = "点击进入查看交易记录", color = "#173177" }
                        }
                    }, null, null);
                }
            });
      
        }
    }
}
