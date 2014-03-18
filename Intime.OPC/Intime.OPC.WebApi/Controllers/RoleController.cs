using System;
using System.Web.Http;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository;
using Intime.OPC.Service;
using Intime.OPC.WebApi.Core.Security;
using Intime.OPC.WebApi.Models;

namespace Intime.OPC.WebApi.Controllers
{
    /// <summary>
    ///     账户相关接口
    /// </summary>
    public class RoleController : ApiController
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }
        [HttpPut]
        public IHttpActionResult AddRole([FromBody] OPC_AuthRole role)
        //public IHttpActionResult AddUser()
        {
            //TODO:check params
            if (_roleService.Create(role))
            {
                return Ok();
            }

            return InternalServerError();
        }
         [HttpPut]
        public IHttpActionResult UpdateRole([FromBody] OPC_AuthRole role)
        {

            //TODO:check params
            if (_roleService.Update(role))
            {
                return Ok();
            }

            return InternalServerError();
        }

        [HttpPut]
        public IHttpActionResult DeleteRole(int roleId)
        {

            //TODO:check params
            if (_roleService.Delete(roleId))
            {
                return Ok();
            }

            return InternalServerError();
        }

        [HttpGet]
        public IHttpActionResult SelectRole()
        {

            //TODO:check params
            return Ok(_roleService.Select());
        }
        public IHttpActionResult Stop(int roleId)
        {

            //TODO:check params
            if (_roleService.IsStop(roleId, true))
            {
                return Ok();
            }

            return InternalServerError();
        }
        public IHttpActionResult Enable(int roleId)
        {

            //TODO:check params
            if (_roleService.IsStop(roleId, false))
            {
                return Ok();
            }

            return InternalServerError();
        }
      
    }
}