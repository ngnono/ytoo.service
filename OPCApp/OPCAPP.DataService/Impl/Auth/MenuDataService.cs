
﻿// ***********************************************************************
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
using OPCApp.DataService.Interface;
using OPCApp.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCApp.DataService.Impl
{
    /// <summary>
    /// Class MenuDataService.
    /// </summary>
    [Export(typeof(IMenuDataService))]
    public class MenuDataService : IMenuDataService
    {
        IList<MenuGroup> lstMenuGroup;

        public MenuDataService() {
            lstMenuGroup = new List<MenuGroup>();

            var mg = new MenuGroup();
            mg.Text = "权限管理";
            mg.Items.Add(new MenuInfo { Sort = 1, Text = "用户管理", ResourceUrl = "UserListViewModel" });
            mg.Items.Add(new MenuInfo { Sort = 1, Text = "角色管理", ResourceUrl = "RoleListViewModel" });
            lstMenuGroup.Add(mg);

            mg = new MenuGroup();
            mg.Text = "基本信息";
            mg.Items.Add(new MenuInfo { Sort = 1, Text = "门店管理", ResourceUrl = "StoreManage" });
          
            lstMenuGroup.Add(mg);
        }

        /// <summary>
        /// 获得登录用户的所有菜单
        /// </summary>
        /// <returns>IEnumerable{MenuGroup}.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IEnumerable<MenuGroup> GetMenus()
        {
            return lstMenuGroup;
        }
    }
}
