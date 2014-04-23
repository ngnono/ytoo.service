using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using Intime.OPC.Domain;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Models;
using Intime.OPC.Service;
using Intime.OPC.WebApi.Core;
using Intime.OPC.WebApi.Core.Security;
using Intime.OPC.WebApi.Models;

namespace Intime.OPC.WebApi.Controllers
{
    /// <summary>
    ///     账户相关接口
    /// </summary>
    public class AccountController : BaseController
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost]
        public IHttpActionResult AddUser([FromBody] OPC_AuthUser user)
        {
           if (_accountService.Add(user))
            {
                return Ok();
            }
            return InternalServerError();
        }

        [HttpPut]
        public IHttpActionResult ResetPassword([FromBody] int userId)
        {
            return DoFunction(() =>
            {
                _accountService.ResetPassword(userId);
                return true;

            },"");
        }


        [HttpPost]
        public IHttpActionResult ChangePassword( [FromBody]ChangePasswordDto dto)
        {
            return DoAction(() => _accountService.ChangePassword(dto.UserID, dto.OldPassword, dto.NewPassword));
        }

        [HttpPut]
        public IHttpActionResult UpdateUser([FromBody] OPC_AuthUser user)
        {
            //TODO:check params
            if (_accountService.Update(user))
            {
                return Ok();
            }

            return InternalServerError();
        }

        [HttpPost]
        public IHttpActionResult GetUsersByRoleID(int roleId)
        {

            var lst = _accountService.GetUsersByRoleID(roleId,1,1000).Result;

            return Ok(lst);
        }

        [HttpPut]
        public IHttpActionResult DeleteUser([FromBody] int? userId)
        {
            if (userId != 0)
            {
                if (_accountService.DeleteById(userId.Value))
                {
                    return Ok();
                }
            }
            //TODO:check params

            return InternalServerError();
        }

        [HttpPost]
        public IHttpActionResult SelectUser([FromUri] string orgID,  [FromUri] string SearchField, [FromUri] string SearchValue, [FromUri] int pageIndex, [FromUri] int pageSize = 20)
        {
            return DoFunction(() =>
            {
                if (SearchField=="0")
                {
                    return _accountService.SelectByLogName(orgID, SearchValue, pageIndex, pageSize);
                }
                if (SearchField == "1")
                {
                    return _accountService.Select(orgID, SearchValue, pageIndex, pageSize);
                }
                return BadRequest("查询条件错误");
            }, "查询用户信息失败");
        }


        [HttpPut]
        public IHttpActionResult Enable([FromBody] OPC_AuthUser user)
        {
            if (null==user)
            {
                return BadRequest("用户对象为空");
            }
            //TODO:check params
            if (_accountService.IsStop(user.Id, !(user.IsValid.Value)))
            {
                return Ok();
            }

            return InternalServerError();
        }

        [HttpPost]
        public IHttpActionResult Token([FromBody] LoginModel loginModel)
        {
            if (!ModelState.IsValid || loginModel == null)
            {
                return BadRequest("请求参数不正确");
            }

            var user = _accountService.Get(loginModel.UserName, loginModel.Password);

            if (user == null)
            {
                return BadRequest("用户名或密码错误");
            }

            DateTime expiresDate = DateTime.Now.AddSeconds(60*60*24);

           HttpContext.Current.User=new ClaimsPrincipal();
            return Ok(new TokenModel
            {
                AccessToken = SecurityUtils.CreateAccessToken(user.Id, expiresDate),
                UserId = user.Id,
                UserName = loginModel.UserName,
                Expires = expiresDate
            });
        }
    }
}