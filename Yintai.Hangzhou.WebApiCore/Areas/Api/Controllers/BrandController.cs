using System;
using Yintai.Architecture.Common.Web.Mvc.ActionResults;
using Yintai.Architecture.Common.Web.Mvc.Controllers;
using Yintai.Hangzhou.Contract.Brand;
using Yintai.Hangzhou.Contract.DTO.Request.Brand;

namespace Yintai.Hangzhou.WebApiCore.Areas.Api.Controllers
{
    public class BrandController : RestfulController
    {
        private readonly IBrandDataService _brandDataService;

        public BrandController(IBrandDataService brandDataService)
        {
            this._brandDataService = brandDataService;
        }

        public RestfulResult Detail(BrandDetailRequest request)
        {
            return new RestfulResult { Data = this._brandDataService.GetBrand(request) };
        }

        public RestfulResult All(BrandAllRequest request)
        {
            if (String.IsNullOrEmpty(request.Type))
            {
                return new RestfulResult { Data = this._brandDataService.GetAll(request) };
            }

            if (request.Type.ToLower() == "refresh")
            {
                return Refresh(new BrandRefreshRequest
                    {
                        Refreshts = request.Refreshts
                    });
            }

            return new RestfulResult { Data = this._brandDataService.GetAll(request) };
        }

        public RestfulResult Refresh(BrandRefreshRequest request)
        {
            return new RestfulResult { Data = this._brandDataService.GetRefresh(request) };
        }
    }
}