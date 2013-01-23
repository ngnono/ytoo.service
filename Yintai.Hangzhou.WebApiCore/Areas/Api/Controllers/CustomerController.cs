using System;
using System.Web.Mvc;
using Yintai.Architecture.Common;
using Yintai.Architecture.Common.Models;
using Yintai.Architecture.Common.Web.Mvc.ActionResults;
using Yintai.Architecture.Common.Web.Mvc.Controllers;
using Yintai.Hangzhou.Contract.Customer;
using Yintai.Hangzhou.Contract.DTO.Request.Customer;
using Yintai.Hangzhou.Model;
using Yintai.Hangzhou.WebSupport.Binder;
using Yintai.Hangzhou.WebSupport.Mvc;

namespace Yintai.Hangzhou.WebApiCore.Areas.Api.Controllers
{
    /// <summary>
    /// CLR Version: 4.0.30319.269
    /// NameSpace: Yintai.Hangzhou.WebApiCore.Areas.Api.Controllers
    /// FileName: CustomerController
    ///
    /// Created at 11/13/2012 11:18:37 AM
    /// Description: 
    /// </summary>
    public class CustomerController : RestfulController
    {
        private readonly ICustomerDataService _customerService;

        public CustomerController(ICustomerDataService customerService)
        {
            this._customerService = customerService;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public RestfulResult OutSiteLogin(OutSiteLoginRequest request)
        {
            request.OutsiteNickname = UrlDecode(request.OutsiteNickname);

            var response = this._customerService.OutSiteLogin(request);

            return new RestfulResult { Data = response };
        }

        [RestfulAuthorize]
        public RestfulResult Detail(UpdateCustomerRequest request, int? authuid, UserModel authUser)
        {
            request.AuthUid = authuid.Value;
            request.AuthUser = authUser;

            if (String.Compare(request.Method, DefineRestfulMethod.Update, StringComparison.OrdinalIgnoreCase) == 0)
            {
                //更新
                return new RestfulResult
                {
                    Data = this._customerService.Update(request)
                };


            }

            return new RestfulResult
            {
                Data = this._customerService.GetUserInfo(new GetUserInfoRequest
                    {
                        AuthUid = request.AuthUid,
                        Method = request.Method,
                        AuthUser = request.AuthUser,
                        Token = request.Token
                    })
            };
        }

        [RestfulAuthorize]
        [HttpPost]
        public RestfulResult Portrait(PortraitRequest request, int? authuid, UserModel authUser)
        {
            request.AuthUid = authuid.Value;
            request.AuthUser = authUser;

            switch (request.Method.ToLower())
            {
                case DefineRestfulMethod.Create:
                    var r = new UploadLogoRequest(request) { Files = Request.Files };
                    return new RestfulResult { Data = this._customerService.UploadLogo(r) };
                case DefineRestfulMethod.Destroy:
                    var d = new DestroyLogoRequest(request);
                    return new RestfulResult { Data = this._customerService.DestroyLogo(d) };
            }

            return new RestfulResult { Data = new ExecuteResult { StatusCode = StatusCode.ClientError, Message = "参数错误" } };
        }

        public RestfulResult Show(ShowCustomerRequest request, [FetchRestfulAuthUser(IsCanMissing = true, KeyName = Define.Token)]UserModel currentAuthUser)
        {
            return Daren(request, currentAuthUser);
        }

        public RestfulResult Daren(ShowCustomerRequest request, [FetchRestfulAuthUser(IsCanMissing = true, KeyName = Define.Token)]UserModel currentAuthUser)
        {
            request.CurrentAuthUser = currentAuthUser;

            return new RestfulResult { Data = this._customerService.GetShowCustomer(request) };
        }
    }
}
