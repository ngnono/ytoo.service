﻿using System;
using System.Web;
using System.Web.Mvc;
using Yintai.Architecture.Common;
using Yintai.Architecture.Common.Models;
using Yintai.Architecture.Common.Web.Mvc.ActionResults;
using Yintai.Architecture.Common.Web.Mvc.Controllers;
using Yintai.Hangzhou.Contract.Brand;
using Yintai.Hangzhou.Contract.DTO.Request.Promotion;
using Yintai.Hangzhou.Contract.Promotion;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.WebSupport.Binder;
using Yintai.Hangzhou.WebSupport.Mvc;

namespace Yintai.Hangzhou.WebApiCore.Areas.Api.Controllers
{
    public class PromotionController : RestfulController
    {
        private readonly IPromotionDataService _promotionDataService;
        private readonly PassHelper _passHelper;

        public PromotionController(IPromotionDataService promotionDataService, IBrandDataService brandDataService)
        {
            _promotionDataService = promotionDataService;
            _passHelper = new PassHelper(brandDataService);
        }

        [RestfulRoleAuthorize(UserRole.Admin | UserRole.Manager, UserLevel.Daren)]
        [HttpPost]
        public RestfulResult Create(FormCollection formCollection, CreatePromotionRequest request, int? authuid, UserModel authUser)
        {
            request.AuthUid = authuid.Value;
            request.AuthUser = authUser;

            request.Files = Request.Files;
            request.Description = UrlDecode(request.Description);
            request.Name = UrlDecode(request.Name);

            var result = this._promotionDataService.CreatePromotion(request);

            return new RestfulResult() { Data = result };
        }

        public RestfulResult List(GetPromotionListRequest request, int? authuid, UserModel authUser)
        {
            request.Type = UrlDecode(request.Type);

            if (!String.IsNullOrEmpty(request.Type))
            {
                if (request.Type.ToLower() == "refresh")
                {
                    return Refresh(new GetPromotionListForRefresh
                                       {
                                           Lat = request.Lat,
                                           Lng = request.Lng,
                                           PageSize = request.Pagesize,
                                           RefreshTs = request.RefreshTs,
                                           Sort = request.Sort
                                       });
                }
            }

            return new RestfulResult { Data = this._promotionDataService.GetPromotionList(request) };
        }

        public RestfulResult Detail(GetPromotionInfoRequest request, [FetchRestfulAuthUser(IsCanMissing = true, KeyName = Define.Token)]UserModel currentAuthUser)
        {
            request.CurrentAuthUser = currentAuthUser;

            return new RestfulResult { Data = this._promotionDataService.GetPromotionInfo(request) };
        }

        public RestfulResult Refresh(GetPromotionListForRefresh request)
        {
            return new RestfulResult { Data = this._promotionDataService.GetPromotionListForRefresh(request) };
        }

        [RestfulAuthorize]

        public RestfulResult Favor(FormCollection formCollection, PromotionFavorCreateRequest request, int? authuid, UserModel authUser)
        {
            request.AuthUid = authuid.Value;
            request.AuthUser = authUser;

            if (System.String.Compare(request.Method, DefineRestfulMethod.Destroy, System.StringComparison.OrdinalIgnoreCase) == 0)
            {
                return new RestfulResult
                {
                    Data = this._promotionDataService.DestroyFavor(new PromotionFavorDestroyRequest(request))
                };
            }
            //default
            if (String.IsNullOrWhiteSpace(request.Method))
            {
                return new RestfulResult
                {
                    Data = this._promotionDataService.CreateFavor(new PromotionFavorCreateRequest(request))
                };
            }

            return new RestfulResult() { Data = new ExecuteResult() { StatusCode = StatusCode.ClientError, Message = "方法错误" } };
        }

        //[RestfulAuthorize]
        //[HttpGet]
        //public RestfulResult Favor(PromotionFavorCreateRequest request, int? authuid, UserModel authUser)
        //{
        //    request.AuthUid = authuid.Value;
        //    request.AuthUser = authUser;

        //    if (System.String.Compare(request.Method, DefineRestfulMethod.Destroy, System.StringComparison.OrdinalIgnoreCase) == 0)
        //    {
        //        return new RestfulResult
        //        {
        //            Data = this._promotionDataService.DestroyFavor(new PromotionFavorDestroyRequest(request))
        //        };
        //    }
        //    //default
        //    if (String.IsNullOrWhiteSpace(request.Method))
        //    {
        //        return new RestfulResult
        //        {
        //            Data = this._promotionDataService.CreateFavor(new PromotionFavorCreateRequest(request))
        //        };
        //    }

        //    return new RestfulResult() { Data = new ExecuteResult() { StatusCode = StatusCode.ClientError, Message = "方法错误" } };
        //}

        //[HttpGet]
        //[RestfulAuthorize]
        //public RestfulResult Share(PromotionShareCreateRequest request, int? authuid, UserModel authUser)
        //{
        //    request.AuthUid = authuid.Value;
        //    request.AuthUser = authUser;
        //    request.Description = UrlDecode(request.Description);
        //    request.Name = UrlDecode(request.Name);

        //    return new RestfulResult
        //    {
        //        Data = this._promotionDataService.CreateShare(request)
        //    };
        //}

        //[RestfulAuthorize]
        //[HttpGet]
        //public RestfulResult Coupon(PromotionCouponCreateRequest request, int? authuid, UserModel authUser)
        //{
        //    request.AuthUid = authuid.Value;
        //    request.AuthUser = authUser;
        //    if (System.String.Compare(request.Method, DefineRestfulMethod.Create, System.StringComparison.OrdinalIgnoreCase) == 0)
        //    {
        //        var result =
        //        this._promotionDataService.CreateCoupon(request);

        //        if (request.IsPass == 1 && result.Data.CouponCodeResponse != null)
        //        {
        //            result.Data.CouponCodeResponse.Pass = PassController.GetPass(ControllerContext, result.Data.CouponCodeResponse.Id,
        //                                                                         result.Data.CouponCodeResponse.CouponId,
        //                                                                         result.Data.CouponCodeResponse
        //                                                                               .ProductName,
        //                                                                         result.Data.CouponCodeResponse
        //                                                                               .ProductDescription, null, result.Data.CouponCodeResponse.User_Id);
        //        }

        //        return new RestfulResult
        //        {
        //            Data = result
        //        };
        //    }

        //    return new RestfulResult { Data = new ExecuteResult { StatusCode = StatusCode.ClientError, Message = "参数错误" } };
        //}

        [HttpPost]
        [RestfulAuthorize]
        public RestfulResult Share(FormCollection formCollection, PromotionShareCreateRequest request, int? authuid, UserModel authUser)
        {
            request.AuthUid = authuid.Value;
            request.AuthUser = authUser;
            request.Description = UrlDecode(request.Description);
            request.Name = UrlDecode(request.Name);

            return new RestfulResult
            {
                Data = this._promotionDataService.CreateShare(request)
            };
        }

        [RestfulAuthorize]
        // [HttpPost]
        public RestfulResult Coupon(FormCollection formCollection, PromotionCouponCreateRequest request, int? authuid, UserModel authUser)
        {
            request.AuthUid = authuid.Value;
            request.AuthUser = authUser;
            if (System.String.Compare(request.Method, DefineRestfulMethod.Create, System.StringComparison.OrdinalIgnoreCase) == 0)
            {
                var result =
                this._promotionDataService.CreateCoupon(request);

                if (request.IsPass == 1 && result.Data != null && result.Data.CouponCodeResponse != null)
                {
                    var code = result.Data.CouponCodeResponse;
                    result.Data.CouponCodeResponse.Pass = _passHelper.GetPass(ControllerContext.HttpContext, code.Id,
                                                                              code.CouponId, code.User_Id);

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

        [RestfulRoleAuthorize(UserRole.Admin | UserRole.Manager | UserRole.Operators, UserLevel.Daren)]
        [HttpPost]
        public ActionResult Destroy(DestroyPromotionRequest request, int? authuid, UserModel authUser, [FetchPromotion(KeyName = "promotionid", IsCanMissing = true)]PromotionEntity entity)
        {
            if (entity == null || authUser == null)
            {
                return new RestfulResult { Data = new ExecuteResult { StatusCode = StatusCode.ClientError, Message = "参数错误" } };
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

            return t ? new RestfulResult { Data = this._promotionDataService.DestroyPromotion(request) } : new RestfulResult { Data = new ExecuteResult { StatusCode = StatusCode.ClientError, Message = "您没有权限删除其他用户的活动" } };
        }

    }
}
