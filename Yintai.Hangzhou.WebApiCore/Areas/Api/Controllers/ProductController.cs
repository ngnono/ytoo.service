using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Yintai.Architecture.Common;
using Yintai.Architecture.Common.Models;
using Yintai.Architecture.Common.Web.Mvc.ActionResults;
using Yintai.Architecture.Common.Web.Mvc.Controllers;
using Yintai.Hangzhou.Contract.Brand;
using Yintai.Hangzhou.Contract.DTO.Request;
using Yintai.Hangzhou.Contract.DTO.Request.Product;
using Yintai.Hangzhou.Contract.Product;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.WebSupport.Binder;
using Yintai.Hangzhou.WebSupport.Mvc;

using Yintai.Hangzhou.WebApiCore;
using System.Transactions;
using Yintai.Hangzhou.Repository.Contract;
using Yintai.Hangzhou.Contract.DTO.Response;
using Yintai.Hangzhou.Service.Logic;
using Yintai.Hangzhou.Contract.DTO.Response.Resources;
using com.intime.fashion.common;

namespace Yintai.Hangzhou.WebApiCore.Areas.Api.Controllers
{
    public class ProductController : RestfulController
    {
        private readonly IProductDataService _productDataService;

        private readonly PassHelper _passHelper;
        private IOrderRepository _orderRepo;
        private IOrderItemRepository _orderItemRepo;
        private IOrderLogRepository _orderLogRepo;
        private IProductRepository _productRepo;
        private IFavoriteRepository _favoriteRepo;
        private IPromotionRepository _promotionRepo;
        private IOrder2ExRepository _orderexRepo;
        private IRMA2ExRepository _rmaexRepo;

        public ProductController(IProductDataService productDataService, 
            IBrandDataService brandDataService,
            IOrderRepository orderRepo,
            IOrderItemRepository orderItemRepo,
            IOrderLogRepository orderLogRepo,
            IProductRepository productRepo,
            IFavoriteRepository favoriteRepo,
            IPromotionRepository promotionRepo,
            IOrder2ExRepository orderexRepo,
            IRMA2ExRepository rmaexRepo)
        {
            _productDataService = productDataService;
            _passHelper = new PassHelper(brandDataService);
            _orderRepo = orderRepo;
            _orderItemRepo = orderItemRepo;
            _orderLogRepo = orderLogRepo;
            _productRepo = productRepo;
            _favoriteRepo = favoriteRepo;
            _promotionRepo = promotionRepo;
            _orderexRepo = orderexRepo;
            _rmaexRepo = rmaexRepo;

        }

        public ActionResult List(GetProductListRequest request)
        {
            request.Type = UrlDecode(request.Type);

            if (!String.IsNullOrEmpty(request.Type))
            {
                if (request.Type.ToLower() == "refresh")
                {
                    return new RestfulResult
                    {
                        Data = _productDataService.RefreshProduct(new GetProductRefreshRequest
                            {
                                Page = request.Page,
                                Pagesize = request.Pagesize,
                                RefreshTs = request.RefreshTs,
                                Sort = request.Sort,
                                BrandId = request.BrandId,
                                Lat = request.Lat,
                                Lng = request.Lng,
                                TagId = request.TagId,
                                TopicId = request.TopicId,
                                PromotionId = request.PromotionId
                            })
                    };
                }
            }
            else
            {
                return new RestfulResult { Data = this._productDataService.GetProductList(request) };
            }

            return new RestfulResult();
        }

        public ActionResult Detail(GetProductInfoRequest request, [FetchRestfulAuthUserAttribute(IsCanMissing = true, KeyName = Define.Token)]UserModel currentAuthUser)
        {
            request.CurrentAuthUser = currentAuthUser;

            return new RestfulResult { Data = this._productDataService.GetProductInfo(request) };
        }

        [RestfulRoleAuthorize(UserRole.Admin, UserLevel.Daren)]
        [HttpPost]
        public ActionResult Create(CreateProductRequest request, int? authuid, UserModel authUser)
        {
            request.AuthUid = authuid.Value;
            request.AuthUser = authUser;
            request.Description = UrlDecode(request.Description);
            request.Favorable = UrlDecode(request.Favorable);
            request.Name = UrlDecode(request.Name);
            request.RecommendedReason = UrlDecode(request.RecommendedReason);
            request.RecommendUser = request.AuthUid;
            request.Files = Request.Files;

            return new RestfulResult { Data = this._productDataService.CreateProduct(request) };
        }

        [RestfulAuthorize]
        [HttpPost]
        [Obsolete]
        public ActionResult Update(UpdateProductRequest request, int? authuid)
        {
            request.AuthUid = authuid.Value;
            request.Description = UrlDecode(request.Description);
            request.Favorable = UrlDecode(request.Favorable);
            request.Name = UrlDecode(request.Name);
            request.RecommendedReason = UrlDecode(request.RecommendedReason);

            return new RestfulResult { Data = this._productDataService.UpdateProduct(request) };
        }

        [HttpPost]
        [RestfulAuthorize]
        public ActionResult Favor(CreateFavorProductRequest request, int? authuid, UserModel authUser)
        {
            request.AuthUid = authuid.Value;
            request.AuthUser = authUser;

            if (System.String.Compare(request.Method, DefineRestfulMethod.Destroy, System.StringComparison.OrdinalIgnoreCase) == 0)
            {
                //d
                return new RestfulResult { Data = this._productDataService.DestroyFavor(new DestroyFavorProductRequest(request)) };
            }

            if (String.IsNullOrWhiteSpace(request.Method))
            {
                return new RestfulResult { Data = this._productDataService.CreateFavor(new CreateFavorProductRequest(request)) };
            }

            return new RestfulResult { Data = new ExecuteResult() { StatusCode = StatusCode.ClientError, Message = "参数错误" } };
        }

        [RestfulAuthorize]
        [HttpPost]
        public ActionResult Share(CreateShareProductRequest request, int? authuid, UserModel authUser)
        {
            request.AuthUid = authuid.Value;
            request.AuthUser = authUser;
            return new RestfulResult { Data = this._productDataService.CreateShare(request) };
        }

        [RestfulAuthorize]
        [HttpPost]
        public ActionResult Coupon(CreateCouponProductRequest request, int? authuid, UserModel authUser)
        {
            request.AuthUid = authuid.Value;
            request.AuthUser = authUser;
            if (System.String.Compare(request.Method, DefineRestfulMethod.Create, System.StringComparison.OrdinalIgnoreCase) == 0)
            {
                var result = this._productDataService.CreateCoupon(request);

                if (request.IsPass == 1 && result.Data != null && result.Data.CouponCodeResponse != null)
                {
                    var code = result.Data.CouponCodeResponse;
                    result.Data.CouponCodeResponse.Pass = _passHelper.GetPass(ControllerContext.HttpContext, code.Id,
                                                                              code.CouponId, code.User_Id);
                   
                }

                return new RestfulResult
                {
                    Data = result
                };
            }

            return new RestfulResult { Data = new ExecuteResult { StatusCode = StatusCode.ClientError, Message = "参数错误" } };
        }

        public ActionResult Daren(GetProductListRequest request, [FetchUser(KeyName = "userid")]UserModel showUser)
        {
            if (System.String.Compare(request.Method, DefineRestfulMethod.List, System.StringComparison.OrdinalIgnoreCase) != 0)
            {
                return new RestfulResult { Data = new ExecuteResult { StatusCode = StatusCode.ClientError, Message = "方法错误" } };
            }

            request.UserModel = showUser;

            return new RestfulResult { Data = this._productDataService.GetProductList(request) };
        }

        [RestfulRoleAuthorize(UserRole.Admin , UserLevel.Daren)]
        [HttpPost]
        public ActionResult Destroy(FormCollection formCollection, DestroyProductRequest request, int? authuid, UserModel authUser, [FetchProduct(KeyName = "productid", IsCanMissing = true)]ProductEntity entity)
        {
            if (entity == null || authUser == null)
            {
                return new RestfulResult() { Data = new ExecuteResult { StatusCode = StatusCode.ClientError, Message = "参数错误" } };
            }

            //达人只能删除自己的商品，
            //店长 可以删除自己店铺下的商品
            //运营 管理员权限的用户才可以删除他人的商品
            var t = false;
            //3
            if (((authUser.UserRole & (int)UserRole.Admin) != 0) )
            {
                t = true;
            }
            else
            {
                    if (authUser.Id == entity.RecommendUser && (authUser.Level & UserLevel.Daren) != 0)
                    {
                        t = true;
                    }
            }
            request.AuthUid = authuid.Value;
            request.AuthUser = authUser;

            return t ? new RestfulResult { Data = this._productDataService.DestroyProduct(request) } : new RestfulResult { Data = new ExecuteResult { StatusCode = StatusCode.ClientError, Message = "您没有权限操作他人的商品" } };
        }
       

        [RestfulAuthorize]
        [HttpPost]
        public ActionResult Order(OrderRequest request,UserModel authUser)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.Where(v => v.Errors.Count() > 0).First();
                return this.RenderError(r => r.Message = error.Errors.First().ErrorMessage);
            }
            request.AuthUser = authUser;
            decimal totalAmount = 0m;
            
            foreach (var product in request.OrderModel.Products)
            { 
                var productEntity =_productRepo.Find(product.ProductId); 
                if (productEntity ==null)
                    return this.RenderError(r=>r.Message=string.Format("{0} 不存在！",productEntity.Id));
                if (!productEntity.Is4Sale.HasValue || productEntity.Is4Sale.Value==false)
                    return this.RenderError(r=>r.Message=string.Format("{0} 不能销售！",productEntity.Id));
                totalAmount += productEntity.Price * product.Quantity;
            }

            if (totalAmount<=0)
                return this.RenderError(r=>r.Message="商品销售价信息错误！");

            
            var orderNo = OrderRule.CreateCode(0);
            var otherFee = OrderRule.ComputeFee();
           
           var erpOrder =  new { 
                dealPayType = "TENPAY",
                lastUpdateTime = DateTime.Now.ToString(ErpServiceHelper.DATE_FORMAT),
                payTime = string.Empty,
                recvfeeReturnTime = string.Empty,
                dealState = "STATE_WG_WAIT_PAY",
                dealPayFeeCommission = "0",
                buyerName = authUser.Nickname,
                sellerConsignmentTime = string.Empty,
                receiverPostcode = request.OrderModel.ShippingAddress.ShippingZipCode,
                dealStateDesc = "",
                buyerRemark = string.Empty,
                freight = 0,
                couponFee = "0",
                comboInfo = string.Empty,
                dealRateState = "DEAL_RATE_NO_EVAL",
                totalCash="0",
                dealNote = request.OrderModel.Memo,
                dealFlag = string.Empty,
                dealCode = orderNo,
                createTime = DateTime.Now.ToString(ErpServiceHelper.DATE_FORMAT),
                receiverMobile = request.OrderModel.ShippingAddress.ShippingContactPhone,
                buyerUin = string.Empty,
                propertymask = string.Empty,
                receiverPhone = request.OrderModel.ShippingAddress.ShippingContactPhone,
                receiverName = request.OrderModel.ShippingAddress.ShippingContactPerson,
                dealEndTime = string.Empty,
                dealPayFeeTicket = "0",
                dealNoteType = "UN_LABEL",
                recvfeeTime = string.Empty,
                dealPayFeeTotal = totalAmount,
                ppCodId= string.Empty,
                availableAction = string.Empty,
                payReturnTime = string.Empty,
                receiverAddress = request.OrderModel.ShippingAddress.ShippingAddress,
                transportType="TRANSPORT_NONE",
                wuliuId="0",
                tenpayCode = string.Empty,
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
                    ShippingAddress = request.OrderModel.ShippingAddress.ShippingAddress,
                    ShippingContactPerson = request.OrderModel.ShippingAddress.ShippingContactPerson,
                    ShippingContactPhone = request.OrderModel.ShippingAddress.ShippingContactPhone,
                    ShippingFee = otherFee.TotalFee,
                    ShippingZipCode = request.OrderModel.ShippingAddress.ShippingZipCode,
                    Status = (int)OrderStatus.Create,
                    StoreId = 0,
                    UpdateDate = DateTime.Now,
                    UpdateUser = request.AuthUser.Id,
                    TotalAmount = totalAmount,
                    InvoiceAmount = totalAmount,
                    OrderNo = orderNo,
                    TotalPoints = otherFee.TotalPoints

                });
                foreach (var product in request.OrderModel.Products)
                {
                    var productEntity = _productRepo.Find(product.ProductId);
                    var inventoryEntity = Context.Set<InventoryEntity>().Where(pm=>pm.ProductId ==product.ProductId &&pm.PColorId==product.Properties.ColorId && pm.PSizeId == product.Properties.SizeId).FirstOrDefault();
                    if (inventoryEntity == null)
                         return this.RenderError(r=>r.Message=string.Format("{0}库存 不存在！",productEntity.Id));
                    var productSizeEntity = Context.Set<ProductPropertyEntity>().Where(pp => pp.Id == product.Properties.ColorId)
                                            .Join(Context.Set<ProductPropertyValueEntity>().Where(ppv => ppv.Id == product.Properties.ColorValueId)
                                                    , o => o.Id
                                                    , i => i.PropertyId
                                                    , (o, i) => new { PP = o, PPV = i }).FirstOrDefault();
                    var productColorEntity = Context.Set<ProductPropertyEntity>().Where(pp => pp.Id == product.Properties.SizeId)
                                            .Join(Context.Set<ProductPropertyValueEntity>().Where(ppv => ppv.Id == product.Properties.SizeValueId)
                                                    , o => o.Id
                                                    , i => i.PropertyId
                                                    , (o, i) => new { PP = o, PPV = i }).FirstOrDefault();
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
                        ColorId = productColorEntity==null?0:productColorEntity.PP.Id,
                        ColorValueId = productColorEntity==null?0:productColorEntity.PPV.Id,
                        SizeId = productSizeEntity==null?0:productSizeEntity.PP.Id,
                        SizeValueId = productSizeEntity==null?0:productSizeEntity.PPV.Id,
                        Points = 0

                    });
                    erpOrder.itemList.Add(new {
                        itemName = productEntity.Name,
                        itemFlag = string.Empty,
                        itemCode = productEntity.Id.ToString(),
                        account = string.Empty,
                        stockAttr=string.Format("{0}:{1}|{2}:{3}",productColorEntity.PP.PropertyDesc,productColorEntity.PPV.ValueDesc,productSizeEntity.PP.PropertyDesc,productSizeEntity.PPV.ValueDesc),
                        stockLocalCode = string.Empty,
                        dealSubCode = string.Empty,
                        itemLocalCode = string.Empty,
                        refundStateDesc = string.Empty,
                        itemAdjustPrice = "0",
                        itemRetailPrice = productEntity.UnitPrice,
                        tradePropertymask = "256",
                        itemDealState ="STATE_WG_WAIT_PAY",
                        refundState = string.Empty,
                        itemCodeHistory = string.Empty,
                        itemDealCount = product.Quantity,
                        skuId = inventoryEntity.ChannelInventoryId,
                        itemDealPrice = productEntity.Price,
                        availableAction = string.Empty,
                        itemDealStateDesc = string.Empty,
                        itemDiscountFee = "0"
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
                    ,null);
                if (isSuccess)
                {
                    _orderexRepo.Insert(new Order2ExEntity() { 
                         ExOrderNo = exOrderNo,
                          OrderNo = orderEntity.OrderNo,
                           UpdateTime = DateTime.Now
                    });
                    ts.Complete();
                    return this.RenderSuccess<OrderResponse>(m => m.Data = new OrderResponse().FromEntity<OrderResponse>(orderEntity));
                }
                else
                {
                   return this.RenderError(r => r.Message = "失败");
                }
            }
           
        }

        /// <summary>
        /// get the purchase detail product info
        /// </summary>
        /// <param name="request"></param>
        /// <param name="currentAuthUser"></param>
        /// <returns></returns>
        public ActionResult Detail4P(GetProductInfo4PRequest request)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.Where(v => v.Errors.Count() > 0).First();
                return this.RenderError(r => r.Message = error.Errors.First().ErrorMessage);
            }
            var context = _productRepo.Context;
            var linq = _productRepo.Get(p => p.Id == request.ProductId && p.Is4Sale.HasValue && p.Is4Sale.Value)
                       .GroupJoin(context.Set<ResourceEntity>().Where(r => r.Status != (int)DataStatus.Deleted &&
                                                                           r.SourceType == (int)SourceType.Product &&
                                                                           r.Type == (int)ResourceType.Image),
                                   o => o.Id,
                                   i => i.SourceId,
                                   (o, i) => new { P = o, R = i.OrderByDescending(r => r.SortOrder).OrderBy(r => r.CreatedDate).FirstOrDefault() })
                       .ToList().FirstOrDefault();

            if (linq == null)
                return this.RenderError(r => r.Message = "产品不存在!");
            var propertyLinq = context.Set<ProductPropertyEntity>().Where(p => p.ProductId == request.ProductId && p.Status != (int)DataStatus.Deleted)
                                .GroupJoin(context.Set<ProductPropertyValueEntity>().Where(pv => pv.Status != (int)DataStatus.Deleted),
                                    o => o.Id,
                                    i => i.PropertyId,
                                    (o, i) => new { P = o, PV = i })
                                .ToList()
                                .Select(p => new ProductPropertyResponse()
                                {
                                    PropertyId = p.P.Id,
                                    PropertyName = p.P.PropertyDesc,
                                    Values = p.PV.Select(pv => new ProductPropertyValueReponse()
                                    {
                                        ValueId = pv.Id,
                                        ValueName = pv.ValueDesc
                                    })
                                });
            if (propertyLinq.Count() <= 0)
                propertyLinq = context.Set<CategoryPropertyEntity>().Where(p => p.CategoryId == linq.P.Tag_Id && p.Status != (int)DataStatus.Deleted)
                                .GroupJoin(context.Set<CategoryPropertyValueEntity>().Where(pv => pv.Status != (int)DataStatus.Deleted),
                                    o => o.Id,
                                    i => i.PropertyId,
                                    (o, i) => new { P = o, PV = i })
                                .ToList()
                                 .Select(p => new ProductPropertyResponse()
                                 {
                                     PropertyId = p.P.Id,
                                     PropertyName = p.P.PropertyDesc,
                                     Values = p.PV.Select(pv => new ProductPropertyValueReponse()
                                     {
                                         ValueId = pv.Id,
                                         ValueName = pv.ValueDesc
                                     })
                                 });
            var data = new GetProductInfo4PResponse().FromEntity<GetProductInfo4PResponse>(linq.P, res =>
            {
                res.Resource = new ResourceInfoResponse().FromEntity<ResourceInfoResponse>(linq.R);
                var dimensionEntity = Context.Set<ResourceEntity>().Where(r => r.Status == (int)DataStatus.Normal &&
                                    r.IsDimension.HasValue &&
                                    r.IsDimension.Value == true &&
                                    r.SourceId == linq.P.Id).FirstOrDefault();
                if (dimensionEntity!=null)
                    res.DimensionResource = new ResourceInfoResponse().FromEntity<ResourceInfoResponse>(dimensionEntity);
                res.Properties = propertyLinq;
                var rmaMsg = Context.Set<ConfigMsgEntity>().Where(c=>c.MKey=="O_C_RMAPolicy").FirstOrDefault();
                res.RMAPolicy = rmaMsg==null?string.Empty:rmaMsg.Message;
                res.SupportPayments = context.Set<PaymentMethodEntity>().Where(p => p.Status != (int)DataStatus.Deleted)
                                .Select(p => new PaymentMethodResponse() { 
                                     Code = p.Code,
                                      Name = p.Name
                                });
            });
            return this.RenderSuccess<GetProductInfo4PResponse>(m => m.Data = data);
        }

        /// <summary>
        /// return the compute order amount
        /// </summary>
        /// <param name="request"></param>
        /// <param name="currentAuthUser"></param>
        /// <returns></returns>
        public ActionResult ComputeAmount(ComputeAmountRequest request)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.Where(v => v.Errors.Count() > 0).First();
                return this.RenderError(r => r.Message = error.Errors.First().ErrorMessage);
            }
            var linq = Context.Set<ProductEntity>().Where(p=>p.Id== request.ProductId).FirstOrDefault();
            if (linq == null)
                return this.RenderError(m=>m.Message="商品不存在");
            var model = OrderRule.ComputeAmount(linq, request.Quantity);

            return this.RenderSuccess<dynamic>(m => m.Data = new { 
                totalquantity = model.TotalQuantity,
                totalfee = model.TotalFee,
                totalpoints = model.TotalPoints,
                totalamount = model.TotalAmount,
                extendprice = model.ExtendPrice
            });
 
        }

        /// <summary>
        /// get the operations available to current user for this product.
        /// operations includes:
        /// 1. if can get coupon
        /// 2. if favored
        /// </summary>
        /// <param name="authUser"></param>
        /// <returns></returns>
        public ActionResult AvailOperations(GetProductInfoRequest request,[FetchRestfulAuthUserAttribute(IsCanMissing = true, KeyName = Define.Token)] UserModel authUser)
        {
            //是否收藏
            bool isFavored = false;
            bool ifCanCoupon = false;
            var withUserId = (authUser != null && authUser.Id > 0);
            if (withUserId)
            {
               isFavored = _favoriteRepo.Get(f => f.User_Id == authUser.Id && f.FavoriteSourceType == (int)SourceType.Product && f.FavoriteSourceId == request.ProductId && f.Status != (int)DataStatus.Deleted).Any();
            }

            var dbContext = Context;
            var linq = dbContext.Set<Promotion2ProductEntity>().Where(pp => pp.ProdId == request.ProductId && pp.Status != (int)DataStatus.Deleted)
                .Join(dbContext.Set<PromotionEntity>().Where(p => p.Status == (int)DataStatus.Normal && p.EndDate > DateTime.Now),
                        o => o.ProId,
                        i => i.Id,
                        (o, i) => new { Pro = i })
                 .ToList();
           ifCanCoupon =  linq.Any(l => {
               bool hadGetCoupon = false;
               if (withUserId)
               {
                   hadGetCoupon= dbContext.Set<CouponHistoryEntity>().Where(c => c.User_Id == authUser.Id && c.FromPromotion == l.Pro.Id).Any();
               }
                if (l.Pro.PublicationLimit == null || l.Pro.PublicationLimit == -1)
                {
                   return  (!hadGetCoupon) || 
                                  (hadGetCoupon && (!l.Pro.IsLimitPerUser.HasValue ||l.Pro.IsLimitPerUser.Value==false));
                }
                else
                {
                    return l.Pro.InvolvedCount < l.Pro.PublicationLimit &&
                                 (!hadGetCoupon || (hadGetCoupon && (!l.Pro.IsLimitPerUser.HasValue || l.Pro.IsLimitPerUser.Value == false)));
                }
               
           });
           return this.RenderSuccess<GetAvailOperationsResponse>(m=>m.Data=new GetAvailOperationsResponse() { 
                 IsFavored = isFavored,
                  IfCanCoupon = ifCanCoupon
               });
           
        }
    }
}
