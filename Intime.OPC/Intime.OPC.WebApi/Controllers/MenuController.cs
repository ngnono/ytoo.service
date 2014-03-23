using System;
using System.Collections.Generic;
using System.Web.Http;
using Intime.OPC.Domain.Models;
using Intime.OPC.Service;
using Intime.OPC.WebApi.Bindings;

namespace Intime.OPC.WebApi.Controllers
{
    /// <summary>
    ///     账户相关接口
    /// </summary>
    public class MenuController : ApiController
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        [HttpGet]
        public IHttpActionResult LoadMenu([UserId] int? userId)
        {
            if (userId == 0)
            {
                userId = 1;
            }
            if (userId.HasValue)
            {
                try
                {
                    IEnumerable<OPC_AuthMenu> lstMenu = _menuService.SelectByUserID(userId.Value);
                    return Ok(lstMenu);
                }
                catch (Exception ex)
                {
                    return BadRequest("获得用户菜单失败");
                }
            }
            return BadRequest("用户名未登录，或用户名为空");

            //TODO:check params
            return Ok();
        }
    }
}