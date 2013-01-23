using System.Web.Mvc;
using Yintai.Architecture.Common.Web.Mvc.ActionResults;
using Yintai.Architecture.Common.Web.Mvc.Controllers;
using Yintai.Hangzhou.Contract.Coupon;
using Yintai.Hangzhou.Contract.DTO.Request.Coupon;
using Yintai.Hangzhou.Model;
using Yintai.Hangzhou.WebSupport.Mvc;

namespace Yintai.Hangzhou.WebApiCore.Areas.Api.Controllers
{
    [RestfulAuthorize]
    public class CouponController : RestfulController
    {
        private readonly ICouponDataService _couponDataService;

        public CouponController(ICouponDataService couponDataService)
        {
            this._couponDataService = couponDataService;
        }

        public ActionResult List(CouponInfoGetListRequest request, int? authuid, UserModel authUser)
        {
            request.AuthUid = authuid.Value;
            request.AuthUser = authUser;

            return new RestfulResult { Data = this._couponDataService.GetList(request) };
        }

        public ActionResult Detail(CouponInfoGetRequest request, int? authuid, UserModel authUser)
        {
            request.AuthUid = authuid.Value;
            request.AuthUser = authUser;

            return new RestfulResult { Data = this._couponDataService.Get(request) };
        }
    }
}
