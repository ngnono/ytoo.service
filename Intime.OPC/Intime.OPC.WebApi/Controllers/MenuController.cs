// ***********************************************************************
// Assembly         : 03_Intime.OPC.WebApi
// Author           : Liuyh
// Created          : 03-23-2014 11:51:13
//
// Last Modified By : Liuyh
// Last Modified On : 03-24-2014 01:55:51
// ***********************************************************************
// <copyright file="MenuController.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Web.Http;
using Intime.OPC.Domain.Models;
using Intime.OPC.Service;
using Intime.OPC.WebApi.Bindings;

namespace Intime.OPC.WebApi.Controllers
{
    /// <summary>
    /// 账户相关接口
    /// </summary>
    public class MenuController : ApiController
    {
        /// <summary>
        /// The _menu service
        /// </summary>
        private readonly IMenuService _menuService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuController"/> class.
        /// </summary>
        /// <param name="menuService">The menu service.</param>
        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        /// <summary>
        /// Loads the menu.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>IHttpActionResult.</returns>
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