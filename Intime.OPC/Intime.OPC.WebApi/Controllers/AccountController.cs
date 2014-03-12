using Intime.OPC.Service;
using Intime.OPC.WebApi.Core.Security;
using Intime.OPC.WebApi.Models;

using System;
using System.Web.Http;

namespace Intime.OPC.WebApi.Controllers
{
    /// <summary>
    /// 账户相关接口
    /// </summary>
    public class AccountController : ApiController
    {
        private IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost]
        public IHttpActionResult Token([FromBody]LoginModel loginModel)
        {
            if (!this.ModelState.IsValid || loginModel == null)
            {
                return BadRequest("请求参数不正确");
            }

            var user = _accountService.Get(loginModel.UserName, loginModel.Password);

            if (user == null)
            {
                return BadRequest("用户名或密码错误");
            }

            var expiresDate = DateTime.Now.AddSeconds(60 * 60 * 24);
            return Ok(new TokenModel()
            {
                AccessToken = SecurityUtils.CreateAccessToken(user.Id, expiresDate),
                UserId = user.Id,
                UserName = loginModel.UserName,
                Expires = expiresDate
            });
        }
    }
}