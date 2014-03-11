// ***********************************************************************
// Assembly         : OPCApp.Order
// Author           : Liuyh
// Created          : 03-09-2014 22:15:01
//
// Last Modified By : Liuyh
// Last Modified On : 02-21-2014 21:34:46
// ***********************************************************************
// <copyright file="OrderModule.cs" company="">
//     Copyright (c) Liuyh. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
//ddd
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using OPCApp.Main.Infrastructure;
using OPCApp.Order.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// The Order namespace.
/// </summary>
namespace OPCApp.Order
{
    /// <summary>
    /// Class OrderModule.
    /// </summary>
    [ModuleExport(typeof(OrderModule))]
    public class OrderModule : IModule
    {
        /// <summary>
        /// The region manager
        /// </summary>
        [Import]
        public IRegionManager RegionManager;
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Initialize()
        {
            this.RegionManager.RegisterViewWithRegion(RegionNames.MainNavigationRegion, typeof(OrderNavigationItemView));
        }
    }
}
