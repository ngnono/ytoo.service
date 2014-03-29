﻿using System.Collections.Generic;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository;

namespace Intime.OPC.Service.Support
{
    public class MenuService : BaseService, IMenuService
    {
        private readonly IMenuRepository _menuRepository;

        public MenuService(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        #region IMenuService Members

        public IEnumerable<OPC_AuthMenu> SelectByRoleID(int roleID)
        {
            return _menuRepository.GetMenusByRoleID(roleID);
        }

        public IEnumerable<OPC_AuthMenu> SelectByUserID(int userID)
        {
            return _menuRepository.GetMenusByUserID(userID);
        }

        public IList<OPC_AuthMenu> GetMenuList()
        {
            return _menuRepository.GetMenuList();
        }

        #endregion
    }
}