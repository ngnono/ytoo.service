using System;
using System.Web.Mvc;
using Yintai.Architecture.Common.Models;
using Yintai.Architecture.Common.Web.Mvc.ActionResults;
using Yintai.Architecture.Common.Web.Mvc.Controllers;
using Yintai.Architecture.Framework.Mapping;
using Yintai.Hangzhou.Contract.Comment;
using Yintai.Hangzhou.Contract.DTO.Request.Comment;
using Yintai.Hangzhou.Model;
using Yintai.Hangzhou.WebSupport.Mvc;

namespace Yintai.Hangzhou.WebApiCore.Areas.Api.Controllers
{
    public class CommentController : RestfulController
    {
        private readonly ICommentDataService _commentDataService;

        public CommentController(ICommentDataService commentDataService)
        {
            _commentDataService = commentDataService;
        }

        public ActionResult List(CommentListRequest request)
        {
            if (String.IsNullOrEmpty(request.Type) || request.Type.ToLower() != "refresh")
            {
                return new RestfulResult { Data = this._commentDataService.GetList(request) };
            }

            var refresh = Mapper.Map<CommentListRequest, CommentRefreshRequest>(request);
            refresh.Timestamp.TsType = TimestampType.New;

            return new RestfulResult { Data = this._commentDataService.GetListRefresh(refresh) };
        }

        [HttpPost]
        [RestfulAuthorize]
        public ActionResult Create(CommentCreateRequest request, int? authuid , UserModel authUser)
        {
            request.Content = UrlDecode(request.Content);
            request.AuthUid = authuid.Value;
            request.AuthUser = authUser;
            request.Files = Request.Files;

            return new RestfulResult { Data = this._commentDataService.Create(request) };
        }

        public ActionResult Detail(CommentDetailRequest request)
        {
            return new RestfulResult { Data = this._commentDataService.Detail(request) };
        }

        [HttpPost]
        [RestfulAuthorize]
        public ActionResult Destroy(CommentDestroyRequest request, int? authuid, UserModel authUser)
        {
            request.AuthUid = authuid.Value;
            request.AuthUser = authUser;
            return new RestfulResult { Data = this._commentDataService.Destroy(request) };
        }

        [HttpPost]
        [RestfulAuthorize]
        public ActionResult Update(CommentUpdateRequest request, int? authuid, UserModel authUser)
        {
            request.AuthUid = authuid.Value;
            request.AuthUser = authUser;
            request.Content = UrlDecode(request.Content);
            request.Files = Request.Files;

            return new RestfulResult { Data = this._commentDataService.Update(request) };
        }
    }
}
