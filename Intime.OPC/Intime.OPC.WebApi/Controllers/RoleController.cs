using System;
using System.Web.Http;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository;
using Intime.OPC.WebApi.Bindings;
using Intime.OPC.WebApi.Core;

namespace Intime.OPC.WebApi.Controllers
{
    /// <summary>
    ///     账户相关接口
    /// </summary>
    public class RoleController : BaseController
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        ///     设定用户的角色
        /// </summary>
        /// <param name="roleUserDto">The role user dto.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult SetUsers([FromBody] RoleUserDto roleUserDto)
        {
            try
            {
                _roleService.SetUsers(roleUserDto);
                return Ok();
            }
            catch (Exception ex)
            {
                this.GetLog().Error(ex);
            }
            return InternalServerError();

        }

        [HttpPost]
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

            try
            {
                if (_roleService.Update(role))
                {
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                this.GetLog().Error(ex);
            }

            return InternalServerError();
        }

        [HttpPost]
        public IHttpActionResult SetMenus([FromBody] RoleMenuDto menuDto,[UserId] int userId)
        {
            bool bl = _roleService.SetMenus(menuDto.RoleID,userId,menuDto.MenuList);
            if (bl)
                return Ok();
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