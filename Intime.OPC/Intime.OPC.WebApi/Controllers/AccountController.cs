using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using Intime.OPC.Domain;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository;
using Intime.OPC.Service;
using Intime.OPC.WebApi.Core;
using Intime.OPC.WebApi.Core.MessageHandlers.AccessToken;
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
        private readonly IRoleRepository _roleRepository;

        public AccountController(IAccountService accountService, IRoleRepository roleRepository)
        {
            _accountService = accountService;
            _roleRepository = roleRepository;
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

            }, "");
        }


        [HttpPost]
        public IHttpActionResult ChangePassword([FromBody]ChangePasswordDto dto)
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

            var lst = _accountService.GetUsersByRoleID(roleId, 1, 1000).Result;

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
        public IHttpActionResult SelectUser([FromUri] string orgID, [FromUri] string SearchField, [FromUri] string SearchValue, [FromUri] int pageIndex, [FromUri] int pageSize = 20)
        {
            return DoFunction(() =>
            {
                if (SearchField == "0")
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
            if (null == user)
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
                return BadRequest(ModelState);
            }

            // ===========================================================
            // 验证用户登录信息
            // ===========================================================

            var currentUser = _accountService.Get(loginModel.UserName, loginModel.Password);

            if (currentUser == null)
            {
                return BadRequest("用户验证失败，请检查您的密码是否正确");
            }

            // ===========================================================
            // 验证通过
            // ===========================================================

            var expires = DateTime.Now.AddSeconds(60 * 60 * 24);

            var roles = _roleRepository.GetRolesByUserId(currentUser.Id);

            var profile = new UserProfile()
            {
                Id = currentUser.Id,
                Name = currentUser.Name,
                OrganizationId = currentUser.OrgId,
                SectionId = currentUser.SectionId,
                DataAuthId = currentUser.DataAuthId,
                Roles = roles.ToList().ConvertAll(p => p.Name)
            };

            return Ok(new TokenModel
            {
                AccessToken = SecurityUtils.CreateToken(profile, expires),
                UserId = currentUser.Id,
                UserName = loginModel.UserName,
                Expires = expires
            });
        }
    }
}