using System;
using System.Collections.Generic;
using System.Linq;
using Intime.OPC.Domain;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository;

namespace Intime.OPC.Service.Support
{
    public class RoleService : BaseService<OPC_AuthRole>, IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IRoleMenuRepository _roleMenuRepository;

        public RoleService(IRoleRepository roleRepository,IRoleMenuRepository roleMenuRepository):base(roleRepository)
        {
            _roleRepository = roleRepository;
            _roleMenuRepository = roleMenuRepository;
        }

        #region IRoleService Members

       
        public PageResult<OPC_AuthRole> Select(int pageIndex, int pageSize)
        {
            return _repository.GetAll(pageIndex, pageSize);
        }

        public bool IsStop(int roleId, bool bValid)
        {
            return _roleRepository.SetEnable(roleId, bValid);
        }

        public bool SetMenus(int roleId, int userID, IEnumerable<int> menuids)
        {
            var bl = _roleMenuRepository.DeleteByRoleMenu(roleId);
            if (bl)
            {
                return _roleMenuRepository.AddMenus(roleId, userID, menuids);
            }
            return false;
        }

        public PageResult<OPC_AuthRole> GetByUserID(int userID,int pageIndex, int pageSize = 20)
        {
            return _roleRepository.GetByUserID(userID,pageIndex,pageSize);
        }

        public bool SetUsers(RoleUserDto roleUserDto)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}