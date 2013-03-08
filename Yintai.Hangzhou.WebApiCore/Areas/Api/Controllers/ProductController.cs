using System;
using System.Web.Mvc;
using Yintai.Architecture.Common;
using Yintai.Architecture.Common.Models;
using Yintai.Architecture.Common.Web.Mvc.ActionResults;
using Yintai.Architecture.Common.Web.Mvc.Controllers;
using Yintai.Hangzhou.Contract.Brand;
using Yintai.Hangzhou.Contract.DTO.Request.Product;
using Yintai.Hangzhou.Contract.Product;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.WebSupport.Binder;
using Yintai.Hangzhou.WebSupport.Mvc;

namespace Yintai.Hangzhou.WebApiCore.Areas.Api.Controllers
{
    public class ProductController : RestfulController
    {
        private readonly IProductDataService _productDataService;

        private readonly PassHelper _passHelper;

        public ProductController(IProductDataService productDataService, IBrandDataService brandDataService)
        {
            _productDataService = productDataService;
            _passHelper = new PassHelper(brandDataService);
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

        [RestfulRoleAuthorize(UserRole.Admin | UserRole.Manager, UserLevel.Daren)]
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

        [RestfulRoleAuthorize(UserRole.Admin | UserRole.Manager | UserRole.Operators, UserLevel.Daren)]
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
            if (((authUser.UserRole & UserRole.Admin) != 0) || (authUser.UserRole & UserRole.Operators) != 0)
            {
                t = true;
            }
            else
            {
                //2
                if ((authUser.UserRole & UserRole.Manager) != 0)
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
            }
            request.AuthUid = authuid.Value;
            request.AuthUser = authUser;

            return t ? new RestfulResult { Data = this._productDataService.DestroyProduct(request) } : new RestfulResult { Data = new ExecuteResult { StatusCode = StatusCode.ClientError, Message = "您没有权限操作他人的商品" } };
        }

        //[RestfulRoleAuthorize(UserRole.Admin | UserRole.Manager | UserRole.Operators, UserLevel.Daren)]
        //[HttpGet]
        //public ActionResult Destroy(DestroyProductRequest request, int? authuid, UserModel authUser, [FetchProduct(KeyName = "productid", IsCanMissing = true)]ProductEntity entity)
        //{
        //    if (entity == null || authUser == null)
        //    {
        //        return new RestfulResult() { Data = new ExecuteResult { StatusCode = StatusCode.ClientError, Message = "参数错误" } };
        //    }

        //    //达人只能删除自己的商品，
        //    //店长 可以删除自己店铺下的商品
        //    //运营 管理员权限的用户才可以删除他人的商品
        //    var t = false;
        //    //3
        //    if (((authUser.UserRole & UserRole.Admin) != 0) || (authUser.UserRole & UserRole.Operators) != 0)
        //    {
        //        t = true;
        //    }
        //    else
        //    {
        //        //2
        //        if ((authUser.UserRole & UserRole.Manager) != 0)
        //        {
        //            t = true;
        //        }
        //        else
        //        {
        //            if (authUser.Id == entity.RecommendUser && (authUser.Level & UserLevel.Daren) != 0)
        //            {
        //                t = true;
        //            }
        //        }
        //    }
        //    request.AuthUid = authuid.Value;
        //    request.AuthUser = authUser;

        //    return t ? new RestfulResult { Data = this._productDataService.DestroyProduct(request) } : new RestfulResult { Data = new ExecuteResult { StatusCode = StatusCode.ClientError, Message = "您没有权限操作他人的商品" } };
        //}

    }
}
