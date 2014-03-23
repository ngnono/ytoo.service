using System;
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
    public class MenuController : ApiController
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }
     
        [HttpGet]
        public IHttpActionResult LoadMenu()
        {

            //TODO:check params
            return Ok(_menuService.Select());
        }
     
    }
}