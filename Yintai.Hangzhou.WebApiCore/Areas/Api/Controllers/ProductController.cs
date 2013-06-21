﻿using System;
using System.Linq;
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

        public ProductController(IProductDataService productDataService, 
            IBrandDataService brandDataService,
            IOrderRepository orderRepo,
            IOrderItemRepository orderItemRepo,
            IOrderLogRepository orderLogRepo,
            IProductRepository productRepo)
        {
            _productDataService = productDataService;
            _passHelper = new PassHelper(brandDataService);
            _orderRepo = orderRepo;
            _orderItemRepo = orderItemRepo;
            _orderLogRepo = orderLogRepo;
            _productRepo = productRepo;

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
                    //result.Data.CouponCodeResponse.Pass = _passHelper.GetPass(ControllerContext,)

                    //result.Data.CouponCodeResponse.Pass = PassController.GetPass(ControllerContext, result.Data.CouponCodeResponse.Id,
                    //                                                             result.Data.CouponCodeResponse.CouponId,
                    //                                                             result.Data.CouponCodeResponse
                    //                                                                   .ProductName,
                    //                                                             result.Data.CouponCodeResponse
                    //                                                                   .ProductDescription, null, result.Data.CouponCodeResponse.User_Id);

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
            var productEntity = _productRepo.Find(p=>p.Id==request.OrderModel.ProductId);
            var totalAmount = productEntity.Price * request.OrderModel.Quantity;
            if (totalAmount<=0)
                return this.RenderError(r=>r.Message="商品价格信息错误！");
            var orderNo = OrderRule.CreateCode();
            using (var ts = new TransactionScope())
            {
                var orderEntity = _orderRepo.Insert(new OrderEntity()
                {
                    BrandId = productEntity.Brand_Id,
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
                    ShippingFee = OrderRule.ComputeFee(request),
                    ShippingZipCode = request.OrderModel.ShippingAddress.ShippingZipCode,
                    Status = (int)OrderStatus.Create,
                    StoreId = productEntity.Store_Id,
                    UpdateDate = DateTime.Now,
                    UpdateUser = request.AuthUser.Id,
                    TotalAmount = totalAmount,
                    OrderNo = orderNo

                });
                _orderItemRepo.Insert(new OrderItemEntity()
                {
                    BrandId = productEntity.Brand_Id,
                    CreateDate = DateTime.Now,
                    CreateUser = request.AuthUser.Id,
                    ItemPrice = productEntity.Price,
                    OrderNo = orderNo,
                    ProductId = productEntity.Id,
                    Quantity = request.OrderModel.Quantity,
                    Status = (int)DataStatus.Normal,
                    StoreId = productEntity.Store_Id,
                    UnitPrice = productEntity.UnitPrice,
                    UpdateDate = DateTime.Now,
                    UpdateUser = request.AuthUser.Id,
                    ExtendPrice = productEntity.Price * request.OrderModel.Quantity,
                    ProductDesc = request.OrderModel.ProductDesc


                });
                _orderLogRepo.Insert(new OrderLogEntity()
                {
                    CreateDate = DateTime.Now,
                    CreateUser = request.AuthUser.Id,
                    CustomerId = request.AuthUser.Id,
                    Operation = string.Format("创建订单"),
                    OrderNo = orderNo,
                    Type = (int)OrderOpera.FromCustomer
                });
                ts.Complete();
                return new RestfulResult() { Data = new OrderResponse().FromEntity<OrderEntity>(orderEntity) };
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
                                   (o, i) => new { P = o, R = i.OrderByDescending(r => r.SortOrder).FirstOrDefault() })
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
                res.Properties = propertyLinq;
                res.RMAPolicy = ConfigManager.RMAPolicy;
                res.SupportPayments = context.Set<PaymentMethodEntity>().Where(p => p.Status != (int)DataStatus.Deleted)
                                .Select(p => new PaymentMethodResponse() { 
                                     Code = p.Code,
                                      Name = p.Name
                                });
            });
            return new RestfulResult() { Data = data };
        }

    


    }
}
