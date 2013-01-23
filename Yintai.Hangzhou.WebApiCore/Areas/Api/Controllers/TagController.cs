using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Yintai.Architecture.Common.Web.Mvc.ActionResults;
using Yintai.Architecture.Common.Web.Mvc.Controllers;
using Yintai.Hangzhou.Contract.DTO.Request.Tag;
using Yintai.Hangzhou.Contract.Tag;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.WebSupport.Mvc;

namespace Yintai.Hangzhou.WebApiCore.Areas.Api.Controllers
{
    public class TagController : RestfulController
    {
        private readonly ITagDataService _tagDataService;

        public TagController(ITagDataService tagDataService)
        {
            this._tagDataService = tagDataService;
        }

        public RestfulResult Detail(TagGetRequest request)
        {
            return new RestfulResult { Data = this._tagDataService.Get(request) };
        }

        public RestfulResult All(TagGetAllRequest request)
        {
            if (String.IsNullOrEmpty(request.Type))
            {
                return new RestfulResult { Data = this._tagDataService.GetAll(request) };
            }

            if (request.Type.ToLower() == "refresh")
            {
                return Refresh(new TagGetRefreshRequest
                {
                    RTs = request.Refreshts
                });
            }

            return new RestfulResult { Data = this._tagDataService.GetAll(request) };
        }

        public RestfulResult Refresh(TagGetRefreshRequest request)
        {
            return new RestfulResult { Data = this._tagDataService.GetRefresh(request) };
        }

        [RestfulRoleAuthorize(UserRole.Admin | UserRole.Manager)]
        [HttpPost]
        public RestfulResult Create(TagCreateRequest request, int? authuid)
        {
            request.Name = UrlDecode(request.Name);
            request.Description = UrlDecode(request.Description);
            request.AuthUid = authuid.Value;

            return new RestfulResult { Data = this._tagDataService.Create(request) };
        }

        [RestfulRoleAuthorize(UserRole.Admin|UserRole.Manager)]
        [HttpPost]
        public RestfulResult Update(TagUpdateRequest request, int? authuid)
        {
            request.Name = UrlDecode(request.Name);
            request.Description = UrlDecode(request.Description);
            request.AuthUid = authuid.Value;

            return new RestfulResult { Data = this._tagDataService.Update(request) };
        }
    }
}
