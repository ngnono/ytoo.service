using com.intime.jobscheduler.Job.Wgw;
using System;
using System.Globalization;
using System.Linq;
using System.Transactions;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Service.Logic;

namespace com.intime.fashion.data.sync.Wgw.Response.Processor
{
    /// <summary>
    /// 处理已经支付的订单
    /// </summary>
    public class OrderProcessor : IProcessor
    {
        public OrderProcessor()
        {
            this.ErrorMessage = string.Empty;
        }

        /// <summary>
        /// 微购物上已经支付的订单同步处理
        /// </summary>
        /// <param name="orderInfo"><see cref="http://open.weigou.qq.com/api/wgwdeal/wgQueryDealList"/> wgQueryDealList返回的信息</param>
        /// <param name="orderDetail"><see cref="http://open.weigou.qq.com/api/wgwdeal/wgQueryDealDetail"/>wgQueryDealDetail返回的信息</param>
        /// <returns></returns>
        public bool Process(dynamic orderInfo, dynamic orderDetail)
        {
            if (orderDetail.errorCode != 0)
            {
                ErrorMessage = orderDetail.errorMessage;
                return false;
            }
            try
            {
                ProcessOrder(orderInfo, orderDetail.dealDetail);
                return true;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            return false;
        }

        /// <summary>
        /// 已同步过的不再处理
        /// </summary>
        /// <param name="orderInfo">订单列表查询wgQueryDealList返回的dealList.dealListVo</param>
        /// <param name="dealDetail">订单详情wgQueryDealDetail返回的Json对象</param>
        private void ProcessOrder(dynamic orderInfo, dynamic dealDetail)
        {
            UserEntity customer = FindCustomer(orderInfo);
            string dealCode = dealDetail.dealCode;
            if (IsOrderExists(customer.Id, dealCode)) //已经加载过的订单不再处理
            {
                return;
            }
            using (var ts = new TransactionScope())
            {
                using (var db = DbContextHelper.GetDbContext())
                {
                    OrderEntity order = CreateOrder(dealDetail);
                    order.CustomerId = customer.Id;
                    db.Orders.Add(order);
                    foreach (var item in dealDetail.trades)
                    {
                        int stockId;
                        if (!int.TryParse(item.stockLoCode.ToString(), out stockId))
                        {
                            throw new WgwSyncException(string.Format("无效的库存编码({0})", item.stockLoCode));
                        }

                        var map4Inventory =
                            db.Map4Inventories.FirstOrDefault(
                                m => m.Channel == ConstValue.WGW_CHANNEL_NAME && m.InventoryId == stockId);

                        if (map4Inventory == null)
                        {
                            throw new WgwSyncException(string.Format("商品({0})库存未映射至微购物",item.tilte));
                        }

                        var inventory =
                            db.Inventories.FirstOrDefault(x=>x.Id == stockId);
                        if (inventory == null)
                        {
                            throw new WgwSyncException(string.Format("不存在的商品库存 ID ({0})", stockId));
                        }

                        var product = db.Products.FirstOrDefault(t=>t.Id == inventory.ProductId);
                        if (product == null)
                        {
                            throw new WgwSyncException(string.Format("微购物订单({1})包含无效的商品 ({0})", item.tilte, dealDetail.dealCode));
                        }

                        int buyNum = int.Parse(item.buyNum.ToString());

                        if(inventory.Amount < buyNum)
                        {
                            throw new WgwSyncException(string.Format("商品{0}({1})库存不足,用户购买({2})件，实际库存({3})件", product.Name, product.Id, buyNum, inventory.Amount));
                        }

                        //是否要扣减库存?或者是OPC扣减??
                        inventory.Amount -= buyNum;

                        var colorEntity = db.ProductPropertyValues.FirstOrDefault(ppv => ppv.Id == inventory.PColorId);
                        var sizeEntity = db.ProductPropertyValues.FirstOrDefault(ppv=>ppv.Id == inventory.PSizeId);
                        db.OrderItems.Add(new OrderItemEntity()
                        {
                            OrderNo = order.OrderNo,
                            ProductId = product.Id,
                            BrandId = product.Brand_Id,
                            StoreId = product.Store_Id,
                            CreateUser = ConstValue.WGW_OPERATOR_USER,
                            CreateDate = DateTime.Now,
                            ItemPrice = item.price,
                            Quantity = item.buyNum,
                            ProductName = product.Name,
                            Status = (int)DataStatus.Normal,
                            UpdateDate = DateTime.Now,
                            UpdateUser = ConstValue.WGW_OPERATOR_USER,
                            ExtendPrice = item.disTotal,
                            ProductDesc = map4Inventory.attr,
                            ColorId = colorEntity == null ? 0 : colorEntity.PropertyId,
                            ColorValueId = colorEntity == null ? 0 : colorEntity.Id,
                            SizeId = sizeEntity == null ? 0 : sizeEntity.PropertyId,
                            SizeValueId = sizeEntity == null ? 0 : sizeEntity.Id,
                            ColorValueName = colorEntity == null ? string.Empty : colorEntity.ValueDesc,
                            SizeValueName = sizeEntity == null ? string.Empty : sizeEntity.ValueDesc,
                            StoreItemNo = product.SkuCode,
                            Points = 0,
                            UnitPrice = product.UnitPrice
                        });
                    }
                    db.Map4Orders.Add(new Map4Order
                    {
                        ChannelOrderCode = dealCode,
                        OrderNo = order.OrderNo,
                        Channel = ConstValue.WGW_CHANNEL_NAME,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        SyncStatus = (int)OrderOpera.FromCustomer,
                    });
                    db.OrderLogs.Add(new OrderLogEntity()
                    {
                        CreateDate = DateTime.Now,
                        CreateUser = ConstValue.WGW_OPERATOR_USER,
                        CustomerId = customer.Id,
                        Operation = string.Format("创建已支付订单"),
                        OrderNo = order.OrderNo,
                        Type = (int)OrderOpera.FromOperator
                    });
                    db.OrderTransactions.Add(new OrderTransactionEntity()
                    {
                        OrderNo = order.OrderNo,
                        PaymentCode = order.PaymentMethodCode,
                        Amount = order.TotalAmount,
                        CreateDate = DateTime.Now,
                        TransNo = string.Empty,
                        IsSynced = false,
                        CanSync = -1,
                        //OutsiteUId =  customer.
                        OrderType = (int)PaidOrderType.Self
                    });
                    db.SaveChanges();
                }
                ts.Complete();
            }
        }

        /// <summary>
        /// 订单是否已经同步
        /// </summary>
        /// <param name="customerId">用户Id</param>
        /// <param name="dealCode">微购物订单编码</param>
        /// <returns></returns>
        private bool IsOrderExists(int customerId, string dealCode)
        {
            using (var db = GetDbContext())
            {
                return
                    db.Orders.Where(o => o.CustomerId == customerId)
                        .Join(
                            db.Map4Orders.Where(m => m.Channel == ConstValue.WGW_CHANNEL_NAME && m.ChannelOrderCode == dealCode),
                            o => o.OrderNo, m => m.OrderNo, (o, m) => o).Any();
            }
        }

        /// <summary>
        /// 通过openid查找用户，如果找不到则创建一个联合登录用户
        /// </summary>
        /// <param name="orderInfo"></param>
        /// <returns></returns>
        private UserEntity FindCustomer(dynamic orderInfo)
        {
            string openId = orderInfo.buyerOpenid;
            OutsiteUserEntity outsiteUser = null;
            using (var db = DbContextHelper.GetDbContext())
            {
                outsiteUser =
                    db.OutsiteUsers.FirstOrDefault(
                        o => o.OutsiteUserId == openId && o.OutsiteType == (int)OutsiteType.WX);
            }
            if (outsiteUser == null)
            {
                return CreateNewUser(orderInfo);
            }
            using (var db = DbContextHelper.GetDbContext())
            {
                return db.Users.FirstOrDefault(u => u.Id == outsiteUser.AssociateUserId);
            }
        }

        /// <summary>
        /// 通过订单相关信息创建外站用户和用户
        /// </summary>
        /// <param name="orderInfo"></param>
        /// <returns></returns>
        private UserEntity CreateNewUser(dynamic orderInfo)
        {
            using (var db = GetDbContext())
            {
                var name = String.Format("__{0}{1}",
                    ((int)OutsiteType.WX).ToString(CultureInfo.InvariantCulture), orderInfo.buyerOpenid);
                var user = db.Set<UserEntity>().Add(new UserEntity()
                {
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    CreatedUser = ConstValue.WGW_OPERATOR_USER,
                    UpdatedUser = ConstValue.WGW_OPERATOR_USER,
                    EMail = String.Empty,
                    LastLoginDate = DateTime.Now,
                    Logo = String.Empty,
                    Nickname = orderInfo.buyerNickName,
                    Name = name,
                    Password = Guid.NewGuid().ToString(),
                    Status = (int)OrderStatus.Paid,
                    Store_Id = 0,
                    Region_Id = 0,
                    IsCardBinded = false,
                    UserLevel = (int)UserLevel.User,
                    Description = String.Empty,
                    Mobile = string.Empty,
                    Gender = (int)GenderType.Default
                });

                db.SaveChanges();

                db.OutsiteUsers.Add(new OutsiteUserEntity()
                {
                    CreatedDate = DateTime.Now,
                    CreatedUser = user.Id,
                    Description = String.Empty,
                    LastLoginDate = DateTime.Now,
                    AssociateUserId = user.Id,
                    OutsiteType = (int)OutsiteType.WX,
                    Status = (int)DataStatus.Normal,
                    OutsiteUserId = orderInfo.buyerOpenid
                });

                db.SaveChanges();
                return user;
            }
        }

        /// <summary>
        /// 创建订单编号，参考OrderRule
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        private string CreateOrderCode(int storeId)
        {
            return OrderRule.CreateCode(storeId);
        }

        /// <summary>
        /// 创建订单实体对象
        /// </summary>
        /// <param name="dealDetail"></param>
        /// <returns></returns>
        private OrderEntity CreateOrder(dynamic dealDetail)
        {
            decimal totalAmount = decimal.Parse(dealDetail.disTotalPay.ToString())/100;
            return new OrderEntity()
            {
                BrandId = 0,
                StoreId = 0,
                CreateDate = DateTime.Now,
                CreateUser = ConstValue.WGW_OPERATOR_USER,
                OrderNo = CreateOrderCode(0),
                InvoiceDetail = string.Empty,
                InvoiceSubject = string.Empty,
                NeedInvoice = false,
                Memo = dealDetail.noteContent,
                ShippingAddress = dealDetail.recv.recvInfo,
                ShippingContactPerson = dealDetail.recv.name,
                ShippingContactPhone = dealDetail.recv.mobile ?? dealDetail.recv.phone,
                ShippingZipCode = dealDetail.recv.postCode,
                ShippingFee = dealDetail.shipFee,
                Status = (int)OrderStatus.Paid,
                UpdateDate = DateTime.Now,
                UpdateUser = ConstValue.WGW_OPERATOR_USER,
                TotalAmount = totalAmount,
                PaymentMethodCode = dealDetail.bankType == 1 ? WgwConfigHelper.WGW_WX_PAYMENTCODE : WgwConfigHelper.WGW_CFT_PAYMENTCODE,
                PaymentMethodName = dealDetail.bankType == 1 ? WgwConfigHelper.WGW_WX_PAYMENTNAME : WgwConfigHelper.WGW_CFT_PAYMENTNAME,
                InvoiceAmount = totalAmount,
                TotalPoints = 0,
                OrderSource = ConstValue.WGW_CHANNEL_NAME
            };
        }

        public string ErrorMessage
        {
            get;
            private set;
        }

        private YintaiHangzhouContext GetDbContext()
        {
            return DbContextHelper.GetDbContext();
        }
    }
}
