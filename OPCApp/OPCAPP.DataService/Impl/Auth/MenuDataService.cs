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
using System.Linq;
using OPCApp.DataService.Common;
using OPCApp.DataService.Interface;
using OPCApp.Domain;
using OPCApp.Domain.Models;

namespace OPCApp.DataService.Impl
{
    /// <summary>
    ///     Class MenuDataService.
    /// </summary>
    [Export(typeof (IMenuDataService))]
    public class MenuDataService : IMenuDataService
    {
        /// <summary>
        ///     获得登录用户的所有菜单
        /// </summary>
        /// <returns>IEnumerable{MenuGroup}.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IEnumerable<MenuGroup> GetMenus()
        {
            string paras = string.Format("UserId={0}", 1); //1 update curUserId 
            IList<OPC_AuthMenu> listMenu = RestClient.Get<OPC_AuthMenu>("menu/loadmenu", paras);
            List<OPC_AuthMenu> groupMenu1 = listMenu.Where(e => e.PraentMenuId == e.Id).ToList();
            List<MenuGroup> groupMenu =
                groupMenu1.Select(e => new MenuGroup {Id = e.Id, Sort = e.Sort, MenuName = e.MenuName})
                    .OrderBy(e => e.Sort)
                    .ToList();
            foreach (MenuGroup grpMenu in groupMenu)
            {
                List<OPC_AuthMenu> menus =
                    listMenu.Where(e => e.Id != e.PraentMenuId && e.PraentMenuId == grpMenu.Id)
                        .OrderBy(e => e.Sort)
                        .ToList();
                if (menus.Count > 0)
                {
                    grpMenu.Items = menus;
                }
            }
            return groupMenu;
        }
    }
}