using com.intime.fashion.common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Transactions;
using Yintai.Architecture.Common.Logger;
using Yintai.Architecture.Common.Web.Mvc.ActionResults;
using Yintai.Architecture.Framework.ServiceLocation;
using Yintai.Hangzhou.Contract.DTO.Request;
using Yintai.Hangzhou.Contract.DTO.Response;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Repository.Contract;

namespace Yintai.Hangzhou.Service.Logic
{
    public class OrderRule
    {
        public static string CreateCode(int storeId)
        {
            var code = string.Concat(string.Format("1{0}", DateTime.Now.ToString("yyMMdd"))
                        , DateTime.UtcNow.Ticks.ToString().Reverse().Take(5)
                            .Aggregate(new StringBuilder(), (s, e) => s.Append(e), s => s.ToString())
                            .PadRight(5, '0'));
            IOrderRepository couponData = ServiceLocator.Current.Resolve<IOrderRepository>();
            var existingCodes = couponData.Get(c => c.OrderNo == code && c.CreateDate>=DateTime.Today).Count();
            if (existingCodes > 0)
                code = string.Concat(code, (existingCodes + 1).ToString());
            return code;
        }
        public static decimal ComputeFee(OrderRequest order)
        {
            return 0;
        }
        public static string CreateShippingCode(string storeId)
        {
            var code = string.Concat(string.Format("O{0}{1}",storeId.PadLeft(3,'0'), DateTime.Now.ToString("yyMMdd"))
                               , DateTime.UtcNow.Ticks.ToString().Reverse().Take(5)
                                   .Aggregate(new StringBuilder(), (s, e) => s.Append(e), s => s.ToString())
                                   .PadRight(5, '0'));
            IOutboundRepository outboundRepo = ServiceLocator.Current.Resolve<IOutboundRepository>();
            var existingCodes = outboundRepo.Get(c => c.OutboundNo == code && c.CreateDate >= DateTime.Today).Count();
            if (existingCodes > 0)
                code = string.Concat(code, (existingCodes + 1).ToString());
            return code;
        }

        public static string CreateRMACode()
        {
            var code = string.Concat(string.Format("R{0}", DateTime.Now.ToString("yyMMdd"))
                       , DateTime.UtcNow.Ticks.ToString().Reverse().Take(5)
                           .Aggregate(new StringBuilder(), (s, e) => s.Append(e), s => s.ToString())
                           .PadRight(5, '0'));
            IRMARepository rmadata = ServiceLocator.Current.Resolve<IRMARepository>();
            var existingCodes = rmadata.Get(c => c.RMANo == code && c.CreateDate >= DateTime.Today).Count();
            if (existingCodes > 0)
                code = string.Concat(code, (existingCodes + 1).ToString());
            return code;
        }

        public static OrderComputeResult ComputeAmount(ProductEntity linq, int quantity)
        {
            return new OrderComputeResult() { 
                 TotalAmount= linq.Price *quantity,
                  ExtendPrice = linq.Price * quantity,
                   TotalFee = 0m,
                    TotalPoints = 0,
                     TotalQuantity = quantity
            };
          
        }
        public static OrderComputeResult ComputeFee()
        {
            return new OrderComputeResult()
            {
                TotalAmount = 0,
                ExtendPrice = 0,
                TotalFee = 0m,
                TotalPoints = 0,
                TotalQuantity = 0
            };
        }
        public static RestfulResult Create(OrderRequest request,UserModel authUser,out bool createSuccess)
        {
            request.AuthUser = authUser;
            createSuccess = false;
            var _productRepo = ServiceLocator.Current.Resolve<IProductRepository>();
            var _orderRepo = ServiceLocator.Current.Resolve<IOrderRepository>();
            var _orderLogRepo = ServiceLocator.Current.Resolve<IOrderLogRepository>();
            var _orderItemRepo = ServiceLocator.Current.Resolve<IOrderItemRepository>();
            var _inventoryRepo = ServiceLocator.Current.Resolve<IInventoryRepository>();
            var _orderexRepo = ServiceLocator.Current.Resolve<IOrder2ExRepository>();

            decimal totalAmount = 0m;

            foreach (var product in request.OrderModel.Products)
            {
                var productEntity = _productRepo.Find(product.ProductId);
                if (productEntity == null)
                    return CommonUtil.RenderError(r => r.Message = string.Format("{0} 不存在！", product.ProductId));
                if (!productEntity.Is4Sale.HasValue || productEntity.Is4Sale.Value == false)
                    return CommonUtil.RenderError(r => r.Message = string.Format("{0} 不能销售！", productEntity.Id));
                totalAmount += productEntity.Price * product.Quantity;
            }

            if (totalAmount <= 0)
                return CommonUtil.RenderError(r => r.Message = "商品销售价信息错误！");


            var orderNo = OrderRule.CreateCode(0);
            var otherFee = OrderRule.ComputeFee();

            var erpOrder = new
            {
                orderSource = request.Channel??string.Empty,
                dealPayType = request.OrderModel.Payment.PaymentCode,
                shipType = request.OrderModel.ShippingType,
                needInvoice = request.OrderModel.NeedInvoice ? 1 : 0,
                invoiceTitle = request.OrderModel.InvoiceTitle??string.Empty,
                invoiceMemo = request.OrderModel.InvoiceDetail??string.Empty,
                orderMemo = request.OrderModel.Memo??string.Empty,
                lastUpdateTime = DateTime.Now.ToString(ErpServiceHelper.DATE_FORMAT),
                payTime = string.Empty,
                recvfeeReturnTime = string.Empty,
                dealState = "STATE_WG_WAIT_PAY",
                buyerName = authUser.Nickname??string.Empty,
                sellerConsignmentTime = string.Empty,
                receiverPostcode = "",
                freight = 0,
                couponFee = "0",
                comboInfo = string.Empty,
                dealRateState = "DEAL_RATE_NO_EVAL",
                totalCash = "0",
                dealNote = request.OrderModel.Memo??string.Empty,
                dealFlag = string.Empty,
                dealCode = orderNo,
                createTime = DateTime.Now.ToString(ErpServiceHelper.DATE_FORMAT),
                receiverMobile = request.OrderModel.ShippingAddress==null?string.Empty:request.OrderModel.ShippingAddress.ShippingContactPhone,

                receiverPhone = request.OrderModel.ShippingAddress == null ? string.Empty : request.OrderModel.ShippingAddress.ShippingContactPhone,
                receiverName = request.OrderModel.ShippingAddress == null ? string.Empty : request.OrderModel.ShippingAddress.ShippingContactPerson,

                dealPayFeeTicket = "0",
                dealNoteType = "UN_LABEL",
                recvfeeTime = string.Empty,
                dealPayFeeTotal = totalAmount,
                ppCodId = string.Empty,


                receiverAddress = request.OrderModel.ShippingAddress == null ? string.Empty : request.OrderModel.ShippingAddress.ShippingAddress,
                transportType = "TRANSPORT_NONE",
                wuliuId = "0",

                vipCard = string.Empty,
                itemList = new List<dynamic>()

            };
            using (var ts = new TransactionScope())
            {
                var orderEntity = _orderRepo.Insert(new OrderEntity()
                {
                    BrandId = 0,
                    CreateDate = DateTime.Now,
                    CreateUser = request.AuthUser.Id,
                    CustomerId = request.AuthUser.Id,
                    InvoiceDetail = request.OrderModel.InvoiceDetail,
                    InvoiceSubject = request.OrderModel.InvoiceTitle,
                    NeedInvoice = request.OrderModel.NeedInvoice,
                    Memo = request.OrderModel.Memo,
                    PaymentMethodCode = request.OrderModel.Payment.PaymentCode,
                    PaymentMethodName = request.OrderModel.Payment.PaymentName,
                    ShippingAddress = request.OrderModel.ShippingAddress==null?string.Empty:request.OrderModel.ShippingAddress.ShippingAddress,
                    ShippingContactPerson = request.OrderModel.ShippingAddress == null ? string.Empty : request.OrderModel.ShippingAddress.ShippingContactPerson,
                    ShippingContactPhone = request.OrderModel.ShippingAddress == null ? string.Empty : request.OrderModel.ShippingAddress.ShippingContactPhone,
                    ShippingFee = otherFee.TotalFee,
                    ShippingZipCode = request.OrderModel.ShippingAddress == null ? string.Empty : request.OrderModel.ShippingAddress.ShippingZipCode,
                    Status = (int)OrderStatus.Create,
                    StoreId = 0,
                    UpdateDate = DateTime.Now,
                    UpdateUser = request.AuthUser.Id,
                    TotalAmount = totalAmount,
                    InvoiceAmount = totalAmount,
                    OrderNo = orderNo,
                    TotalPoints = otherFee.TotalPoints,
                    OrderSource = request.Channel

                });
                foreach (var product in request.OrderModel.Products)
                {
                    var productEntity = _productRepo.Find(product.ProductId);
                    var inventoryEntity = Context.Set<InventoryEntity>().Where(pm => pm.ProductId == product.ProductId && pm.PColorId == product.Properties.ColorValueId && pm.PSizeId == product.Properties.SizeValueId).FirstOrDefault();
                    if (inventoryEntity == null)
                        return CommonUtil.RenderError(r => r.Message = string.Format("{0}库存 不存在！", productEntity.Id));
                    if (inventoryEntity.Amount < product.Quantity)
                        return CommonUtil.RenderError(r => r.Message = string.Format("{0}库存不足！", productEntity.Id));
                    var productSizeEntity = Context.Set<ProductPropertyValueEntity>().Where(ppv => ppv.Id == product.Properties.SizeValueId).FirstOrDefault();
                    var productColorEntity = Context.Set<ProductPropertyValueEntity>().Where(ppv => ppv.Id == product.Properties.ColorValueId).FirstOrDefault();
                    _orderItemRepo.Insert(new OrderItemEntity()
                    {
                        BrandId = productEntity.Brand_Id,
                        CreateDate = DateTime.Now,
                        CreateUser = request.AuthUser.Id,
                        ItemPrice = productEntity.Price,
                        OrderNo = orderNo,
                        ProductId = productEntity.Id,
                        ProductName = productEntity.Name,
                        Quantity = product.Quantity,
                        Status = (int)DataStatus.Normal,
                        StoreId = productEntity.Store_Id,
                        UnitPrice = productEntity.UnitPrice,
                        UpdateDate = DateTime.Now,
                        UpdateUser = request.AuthUser.Id,
                        ExtendPrice = productEntity.Price * product.Quantity,
                        ProductDesc = product.ProductDesc,
                        ColorId = productColorEntity == null ? 0 : productColorEntity.PropertyId,
                        ColorValueId = productColorEntity == null ? 0 : productColorEntity.Id,
                        SizeId = productSizeEntity == null ? 0 : productSizeEntity.PropertyId,
                        SizeValueId = productSizeEntity == null ? 0 : productSizeEntity.Id,
                        ColorValueName = productColorEntity == null ? string.Empty : productColorEntity.ValueDesc,
                        SizeValueName = productSizeEntity == null ? string.Empty : productSizeEntity.ValueDesc,
                        StoreItemNo = productEntity.SkuCode,
                        Points = 0

                    });
                    inventoryEntity.Amount = inventoryEntity.Amount - product.Quantity;
                    inventoryEntity.UpdateDate = DateTime.Now;
                    _inventoryRepo.Update(inventoryEntity);
                    int? storeId = null;
                    int? saleCodeId = null;
                    if (product.StoreId.HasValue && product.StoreId != 0 && product.SectionId.HasValue && product.SectionId != 0)
                    {
                        var storeEntity = Context.Set<StoreEntity>().Find(product.StoreId.Value);
                        storeId = storeEntity.ExStoreId;
                        var sectionEntity = Context.Set<SectionEntity>().Find(product.SectionId.Value);
                        saleCodeId = sectionEntity.ChannelSectionId;
                    }
                    erpOrder.itemList.Add(new
                    {
                        itemName = productEntity.Name,
                        itemFlag = string.Empty,
                        itemCode = string.Empty,
                        account = string.Empty,

                        refundStateDesc = string.Empty,
                        itemAdjustPrice = "0",
                        itemRetailPrice = productEntity.UnitPrice,
                        tradePropertymask = "256",
                        itemDealState = "STATE_WG_WAIT_PAY",

                        itemDealCount = product.Quantity,
                        skuId = inventoryEntity.ChannelInventoryId,
                        itemDealPrice = productEntity.Price,

                        storeId = storeId.HasValue?storeId.ToString():string.Empty,
                        saleCodeSid = saleCodeId.HasValue?saleCodeId.ToString():string.Empty
                    });
                }
                _orderLogRepo.Insert(new OrderLogEntity()
                {
                    CreateDate = DateTime.Now,
                    CreateUser = request.AuthUser.Id,
                    CustomerId = request.AuthUser.Id,
                    Operation = string.Format("创建订单"),
                    OrderNo = orderNo,
                    Type = (int)OrderOpera.FromCustomer
                });
                string exOrderNo = string.Empty;
                bool isSuccess = ErpServiceHelper.SendHttpMessage(ConfigManager.ErpBaseUrl, new { func = "DivideOrderToSaleFromJSON", OrdersJSON = erpOrder }, r => exOrderNo = r.order_no
                    , null);
                if (isSuccess)
                {
                   var exOrderEntity= _orderexRepo.Insert(new Order2ExEntity()
                    {
                        ExOrderNo = exOrderNo,
                        OrderNo = orderEntity.OrderNo,
                        UpdateTime = DateTime.Now
                    });
                    ts.Complete();
                    createSuccess = true;
                    return CommonUtil.RenderSuccess<OrderResponse>(m => m.Data = new OrderResponse().FromEntity<OrderResponse>(orderEntity,o=>o.ExOrderNo = exOrderEntity.ExOrderNo));
                }
                else
                {
                    return CommonUtil.RenderError(r => r.Message = "失败");
                }
            }
        }
        public static bool OrderPaid2Erp(OrderTransactionEntity order,bool isOnlinePay = true)
        {
            string vipCard = string.Empty;
            var log = ServiceLocator.Current.Resolve<ILog>();
            if (order.OutsiteType.HasValue && order.OutsiteType.Value == (int)OutsiteType.WX
                && !string.IsNullOrEmpty(order.OutsiteUId))
            {
                try
                {
                    AwsHelper.SendHttpMessage(string.Format("{0}card/find", ConfigManager.AwsHost), new
                    {
                        uid = order.OutsiteUId
                    }, ConfigManager.AwsHttpPublicKey, ConfigManager.AwsHttpPrivateKey, r => vipCard = r.data, null);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }
            var paymentName = string.Empty;
            using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
            {
                var paymentEntity = db.Set<PaymentMethodEntity>().Where(p => p.Code == order.PaymentCode).FirstOrDefault();
                if (paymentEntity == null)
                {
                    log.Error(string.Format("orderno :{1} not support payment code paid:{0}",order.PaymentCode,order.OrderNo));
                    return false;
                }
                paymentName = paymentEntity.Name;
            }
            var paidFunc = isOnlinePay ? "WebOrdersPaid" : "WebSalesPaid";
            bool isSuccess = ErpServiceHelper.SendHttpMessage(ConfigManager.ErpBaseUrl, new { func = paidFunc, dealCode = order.OrderNo, PAY_TYPE = order.PaymentCode, PaymentName = paymentName, TRADE_NO = order.TransNo, CardNo = vipCard }, null
                          , null);
            if (isSuccess)
            {
                using (var db = new YintaiHangzhouContext("YintaiHangzhouContext"))
                {
                    order.IsSynced = true;
                    order.SyncDate = DateTime.Now;
                    db.Entry(order).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return isSuccess;
        }

        public static DbContext Context
        {
            get { return ServiceLocator.Current.Resolve<DbContext>(); }
        }

        public class OrderComputeResult
        {

            public int TotalQuantity { get; set; }
            public int TotalPoints { get; set; }
            public decimal TotalFee { get; set; }
            public decimal ExtendPrice { get; set; }
            public decimal TotalAmount { get; set; }

        }

        public static bool SyncPaidOrder2Erp(string orderNo, string orderSource = "wgw")
        {
            if (string.IsNullOrEmpty(orderNo))
            {
                throw new ArgumentNullException("orderNo");
            }

            var order =
                Context.Set<OrderEntity>().FirstOrDefault(o => o.OrderNo == orderNo && o.OrderSource == orderSource);

            if (order == null)
            {
                throw new NullReferenceException(string.Format("未找到订单号为({0})订单",orderNo));
            }

            var customer = Context.Set<UserEntity>().FirstOrDefault(u => u.Id == order.CustomerId);

            var erpOrder = new
            {
                orderSource,
                dealPayType = order.PaymentMethodCode,
                shipType = (int)ShipType.TrdParty, //request.OrderModel.ShippingType,
                needInvoice = order.NeedInvoice.HasValue && order.NeedInvoice.Value ? 1 : 0,
                invoiceTitle = order.InvoiceSubject ?? string.Empty,
                invoiceMemo = order.InvoiceDetail ?? string.Empty,
                orderMemo = order.Memo ?? string.Empty,
                lastUpdateTime = DateTime.Now.ToString(ErpServiceHelper.DATE_FORMAT),
                payTime = string.Empty,
                recvfeeReturnTime = string.Empty,
                dealState = "STATE_WG_WAIT_PAY",
                buyerName = customer != null ? customer.Name: string.Empty,
                sellerConsignmentTime = string.Empty,
                receiverPostcode = "",
                freight = 0,
                couponFee = "0",
                comboInfo = string.Empty,
                dealRateState = "DEAL_RATE_NO_EVAL",
                totalCash = "0",
                dealNote = order.Memo ?? string.Empty,
                dealFlag = string.Empty,
                dealCode = orderNo,
                createTime = DateTime.Now.ToString(ErpServiceHelper.DATE_FORMAT),
                receiverMobile = order.ShippingContactPhone ?? string.Empty,

                receiverPhone = order.ShippingContactPhone??string.Empty,
                receiverName = order.ShippingContactPerson??string.Empty,

                dealPayFeeTicket = "0",
                dealNoteType = "UN_LABEL",
                recvfeeTime = string.Empty,
                dealPayFeeTotal = order.TotalAmount,
                ppCodId = string.Empty,

                receiverAddress = order.ShippingAddress,
                transportType = "TRANSPORT_NONE",
                wuliuId = "0",

                vipCard = string.Empty,
                itemList = new List<dynamic>()

            };
            foreach (var item in Context.Set<OrderItemEntity>().Where(o => o.OrderNo == orderNo))
            {
                erpOrder.itemList.Add(CreateErpItem(item));
            }
            string exOrderNo = string.Empty;
            bool isSuccess = ErpServiceHelper.SendHttpMessage(ConfigManager.ErpBaseUrl, new { func = "DivideOrderToSaleFromJSON", OrdersJSON = erpOrder }, r => exOrderNo = r.order_no
                , null);
            if (isSuccess)
            {
                Context.Set<Order2ExEntity>().Add(new Order2ExEntity()
                {
                    ExOrderNo = exOrderNo,
                    OrderNo = order.OrderNo,
                    UpdateTime = DateTime.Now
                });
                Context.SaveChanges();
            }
            return isSuccess;
        }

        private static dynamic CreateErpItem(OrderItemEntity item)
        {
            var skuId = Context.Set<InventoryEntity>().Where(i=>i.ProductId == item.ProductId && i.PColorId == item.ColorValueId && i.PSizeId == item.SizeValueId).Select(t=>t.ChannelInventoryId).FirstOrDefault();
            var store =
                Context.Set<StoreEntity>()
                    .Join(Context.Set<ProductEntity>().Where(p => p.Id == item.ProductId), s => s.Id,
                        p => p.Store_Id, (s, p) => s.ExStoreId);

            var erpItem = new 
            {
                itemName = item.ProductName,
                itemFlag = string.Empty,
                itemCode = string.Empty,
                account = string.Empty,

                refundStateDesc = string.Empty,
                itemAdjustPrice = "0",
                itemRetailPrice = item.ItemPrice,
                tradePropertymask = "256",
                itemDealState = "STATE_WG_WAIT_PAY",
                itemDealCount = item.Quantity,
                skuId,
                itemDealPrice = item.ItemPrice,

                ////微客多上拿不到以下信息
                storeId = store,
                saleCodeSid = string.Empty
                //saleCodeSid = saleCodeId.HasValue ? saleCodeId.ToString() : string.Empty
            };
            return erpItem;
        }
    }
           
}
