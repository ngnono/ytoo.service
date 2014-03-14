// ***********************************************************************
// Assembly         : OPCApp.AuthManage
// Author           : Liuyh
// Created          : 03-14-2014 20:27:36
//
// Last Modified By : Liuyh
// Last Modified On : 03-14-2014 21:21:00
// ***********************************************************************
// <copyright file="MenuViewModel.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using OPCApp.Domain;
using OPCApp.Infrastructure.Mvvm;
using OPCApp.DataService.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace OPCApp.AuthManage.ViewModels
{
    /// <summary>
    /// Class MenuViewModel.
    /// </summary>
    [Export(typeof(MenuViewModel))]
    public class MenuViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MenuViewModel"/> class.
        /// </summary>
        /// <param name="menuService">The menu service.</param>
        [ImportingConstructor]
        public MenuViewModel(IMenuDataService menuService ) {
            this.Items = menuService.GetMenus();
        }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>The items.</value>
        public IEnumerable<MenuGroup> Items { get; set; }
    }
}
