// ***********************************************************************
// Assembly         : OPCApp.DataService
// Author           : Liuyh
// Created          : 03-14-2014 21:16:36
//
// Last Modified By : Liuyh
// Last Modified On : 03-14-2014 21:18:45
// ***********************************************************************
// <copyright file="MenuDataService.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Collections.Generic;
using System.ComponentModel.Composition;
using OPCApp.DataService.Interface;
using OPCApp.Domain;

namespace OPCApp.DataService.Impl.Auth
{
    /// <summary>
    /// Class MenuDataService.
    /// </summary>
    [Export(typeof(IMenuDataService))]
    public class MenuDataService : IMenuDataService
    {
        readonly IList<MenuGroup> _lstMenuGroup;

        public MenuDataService() {
            _lstMenuGroup = new List<MenuGroup>();

            var mg = new MenuGroup {Text = "权限管理"};
            mg.Items.Add(new MenuInfo { Sort = 1, Text = "用户管理", ResourceUrl = "UserListWindow" });
            mg.Items.Add(new MenuInfo { Sort = 1, Text = "角色管理", ResourceUrl = "RoleListWindow" });
            lstMenuGroup.Add(mg);

            mg = new MenuGroup {Text = "基本信息"};
            mg.Items.Add(new MenuInfo { Sort = 1, Text = "门店管理", ResourceUrl = "StoreManage" });
          
            _lstMenuGroup.Add(mg);
        }

        /// <summary>
        /// 获得登录用户的所有菜单
        /// </summary>
        /// <returns>IEnumerable{MenuGroup}.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IEnumerable<MenuGroup> GetMenus()
        {
            return _lstMenuGroup;
        }
    }
}
