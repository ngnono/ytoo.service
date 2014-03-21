﻿using System;
using System.Web.Http;
using Intime.OPC.Domain.Models;
using Intime.OPC.Service;
using Intime.OPC.WebApi.Core.Security;
using Intime.OPC.WebApi.Models;

namespace Intime.OPC.WebApi.Controllers
{
    /// <summary>
    ///     账户相关接口
    /// </summary>
    public class AccountController : ApiController
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        [HttpPut]
        public IHttpActionResult AddUser([FromBody] OPC_AuthUser user)
        //public IHttpActionResult AddUser()
        {
            //TODO:check params
            if (_accountService.Create(user))
            {
                return Ok();
            }

            return InternalServerError();
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

        public IHttpActionResult DeleteUser(int userId)
        {

            //TODO:check params
            if (_accountService.Delete(userId))
            {
                return Ok();
            }

            return InternalServerError();
        }

        [HttpGet]
        public IHttpActionResult SelectUser([FromUri] string test1,[FromUri] string li)
        {

            //TODO:check params
            return Ok(_accountService.Select());
        }
        public IHttpActionResult Stop(int userId)
        {

            //TODO:check params
            if (_accountService.IsStop(userId, true))
            {
                return Ok();
            }

            return InternalServerError();
        }
        public IHttpActionResult Enable(int userId)
        {

            //TODO:check params
            if (_accountService.IsStop(userId, false))
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

            OPC_AuthUser user = _accountService.Get(loginModel.UserName, loginModel.Password);

            if (user == null)
            {
                return BadRequest("用户名或密码错误");
            }

            DateTime expiresDate = DateTime.Now.AddSeconds(60 * 60 * 24);

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