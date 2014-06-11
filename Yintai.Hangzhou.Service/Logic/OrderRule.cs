using com.intime.fashion.common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Transactions;
using Yintai.Architecture.Common.Data.EF;
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
        public static OrderComputeResult ComputeAmount(IEnumerable<ProductEntity> linq, int quantity)
        {
            var amount = linq.Sum(l=>(decimal?)l.Price*quantity);

            return new OrderComputeResult()
            {
                TotalAmount = amount??0m,
                ExtendPrice = amount??0m,
                TotalFee = 0m,
                TotalPoints = 0,
                TotalQuantity = quantity
            };

        }
        public static OrderComputeResult ComputeAmount_Combo(int comboId)
        {
            var linq = Context.Set<IMS_Combo2ProductEntity>().Where(icp => icp.ComboId == comboId)
                     .Join(Context.Set<ProductEntity>().Where(p => p.Is4Sale == true && p.Status == (int)DataStatus.Normal
                                        && Context.Set<InventoryEntity>().Where(i => i.Amount > 0).Any(i => i.ProductId == p.Id)),
                             o => o.ProductId,
                             i => i.Id,
                             (o, i) => i);
            var amount = linq.Sum(l=>(decimal?)l.Price*1)??0m;

            var discount = ComputeComboDiscount(comboId, linq.Select(l => l.Id));
            var totalAmount = amount-discount;

            return new OrderComputeResult()
            {
                TotalAmount = totalAmount,
                ExtendPrice = amount,
                TotalFee = 0m,
                TotalPoints = 0,
                TotalQuantity =linq.Count()
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
            decimal discountAmount = 0m;
            bool isSelfOrder = false;
            foreach (var product in request.OrderModel.Products)
            {
                var productEntity = _productRepo.Find(product.ProductId);
                if (productEntity == null)
                    return CommonUtil.RenderError(r => r.Message = string.Format("{0} 不存在！", product.ProductId));
                if (!productEntity.Is4Sale.HasValue || productEntity.Is4Sale.Value == false)
                    return CommonUtil.RenderError(r => r.Message = string.Format("{0} 不能销售！", productEntity.Id));
                totalAmount += productEntity.Price * product.Quantity;
                if (productEntity.ProductType.HasValue && productEntity.ProductType.Value == (int)ProductType.FromSelf) 
                {
                    isSelfOrder = true;
                }
            }
            if (request.OrderModel.ComboId.HasValue && request.OrderModel.ComboId > 0)
            {
                discountAmount = ComputeComboDiscount(request.OrderModel.ComboId.Value, request.OrderModel.Products.Select(p => p.ProductId));
                totalAmount = totalAmount - discountAmount;
            }
            if (totalAmount <= 0)
                return CommonUtil.RenderError(r => r.Message = "商品销售价信息错误！");


            var orderNo = OrderRule.CreateCode(0);
            var otherFee = OrderRule.ComputeFee();

          
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
                    OrderSource = request.Channel,
                    OrderProductType = isSelfOrder?(int)OrderProductType.SelfProduct:(int)OrderProductType.SystemProduct,
                     DiscountAmount = discountAmount,
                     PromotionFlag = discountAmount>0?true:false
                });
                foreach (var product in request.OrderModel.Products)
                {
                    var productEntity = _productRepo.Find(product.ProductId);
                    string salesCode = string.Empty;
                    if (productEntity.ProductType == (int)ProductType.FromSelf) {
                        var productSaleCodeEntity = Context.Set<ProductCode2StoreCodeEntity>().Where(p => p.ProductId == product.ProductId && p.Status == (int)DataStatus.Normal).First();
                        salesCode = productSaleCodeEntity.StoreProductCode;
                    }
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
                        Points = 0,
                        ProductType = productEntity.ProductType??(int)ProductType.FromSystem,
                        StoreSalesCode =salesCode

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
                if (request.OrderModel.StoreId.HasValue && request.OrderModel.StoreId > 0)
                {
                    
                    var associateEntity = Context.Set<IMS_AssociateEntity>().Find(request.OrderModel.StoreId);
                    if (associateEntity != null)
                        AssociateIncomeLogic.Create(associateEntity.UserId, orderEntity);

                }
                string exOrderNo = string.Empty;

                bool isSuccess = true;
                if (isSuccess)
                {   
                    exOrderNo = orderEntity.OrderNo;
                    ts.Complete();
                    createSuccess = true;
                    return CommonUtil.RenderSuccess<OrderResponse>(m => m.Data = new OrderResponse().FromEntity<OrderResponse>(orderEntity,o=>o.ExOrderNo=exOrderNo));
                }
                else
                {
                    return CommonUtil.RenderError(r => r.Message = "失败");
                }
            }
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

        public static bool SyncOrder2Erp(string orderNo, string orderSource = "wgw")
        {
            if (string.IsNullOrEmpty(orderNo))
            {
                throw new ArgumentNullException("orderNo");
            }

            var order =
                Context.Set<OrderEntity>().FirstOrDefault(o => o.OrderNo == orderNo && o.OrderSource == orderSource);

            if (order == null)
            {
                throw new NullReferenceException(string.Format("Not exist order ({0})",orderNo));
            }

            var dealCode =
                Context.Set<Map4OrderEntity>().First(o => o.OrderNo == orderNo && o.Channel == orderSource).ChannelOrderCode;

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
                dealCode,
                createTime = DateTime.Now.ToString(ErpServiceHelper.DATE_FORMAT),
                receiverMobile = order.ShippingContactPhone ?? string.Empty,

                receiverPhone = order.ShippingContactPhone??string.Empty,
                receiverName = order.ShippingContactPerson??string.Empty,

                dealPayFeeTicket = "0",
                dealNoteType = "UN_LABEL",
                recvfeeTime = string.Empty,
                dealPayFeeTotal = order.TotalAmount/100,
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

            if (!isSuccess) return false;

            Context.Set<Order2ExEntity>().Add(new Order2ExEntity()
            {
                ExOrderNo = exOrderNo,
                OrderNo = order.OrderNo,
                UpdateTime = DateTime.Now
            });
            if (order.Status == (int) OrderStatus.Paid)
            {
                var oderTransaction =
                    Context.Set<OrderTransactionEntity>().FirstOrDefault(ot => ot.OrderNo == order.OrderNo);
                if (oderTransaction != null)
                {
                    oderTransaction.CanSync = 0; //同步给衡和系统后可以同步支付状态
                }
            }
            Context.SaveChanges();
            return true;
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

        private static decimal ComputeComboDiscount(int comboId, IEnumerable<int> inProducts)
        {
            var comboEntity = Context.Set<IMS_ComboEntity>().Find(comboId);
            bool notMatchCombo = false;
            foreach (var product in Context.Set<IMS_Combo2ProductEntity>().Where(icp => icp.ComboId == comboId))
            {
                if (!inProducts.Contains(product.ProductId))
                {
                    notMatchCombo = true;
                    break;
                }
            }
            bool inPromotion = (comboEntity.IsInPromotion ?? false) &&
                                !notMatchCombo;

            return inPromotion ? (comboEntity.DiscountAmount ?? 0m) :0m;
        }

        public static string CreateGiftCardNo()
        {
            var code = string.Concat(string.Format("GC{0}", DateTime.Now.ToString("yyMMdd"))
                        , DateTime.UtcNow.Ticks.ToString().Reverse().Take(5)
                            .Aggregate(new StringBuilder(), (s, e) => s.Append(e), s => s.ToString())
                            .PadRight(5, '0'));
            IEFRepository<IMS_GiftCardOrderEntity> couponData = ServiceLocator.Current.Resolve<IEFRepository<IMS_GiftCardOrderEntity>>();
            var existingCodes = couponData.Get(c => c.No == code && c.CreateDate >= DateTime.Today).Count();
            if (existingCodes > 0)
                code = string.Concat(code, (existingCodes + 1).ToString());
            return code;
        }
    }
           
}
