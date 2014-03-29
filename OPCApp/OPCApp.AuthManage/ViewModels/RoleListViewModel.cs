﻿// ***********************************************************************
// Assembly         : OPCApp.AuthManage
// Author           : Liuyh
// Created          : 03-15-2014 14:47:14
//
// Last Modified By : Liuyh
// Last Modified On : 03-15-2014 21:03:27
// ***********************************************************************
// <copyright file="RoleListViewModel.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.ComponentModel.Composition;
using System.Linq;
using OPCApp.DataService.Interface;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;
using OPCApp.Infrastructure.DataService;
using OPCApp.Infrastructure.Mvvm;

namespace OPCApp.AuthManage.ViewModels
{
    /// <summary>
    ///     Class RoleListViewModel.
    /// </summary>
    [Export("RoleListViewModel", typeof (IViewModel))]
    public class RoleListViewModel : BaseListViewModel<OPC_AuthRole>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RoleListViewModel" /> class.
        /// </summary>
        public RoleListViewModel()
            : base("RoleListWindow")
        {
            EditViewModeKey = "RoleViewModel";
            AddViewModeKey = "RoleViewModel";
            
        }

        protected override void Load()
        {
            SearchCommand.Execute(null);
        }

        /// <summary>
        ///     Gets the data service.
        /// </summary>
        /// <returns>IBaseDataService{Role}.</returns>
        protected override IBaseDataService<OPC_AuthRole> GetDataService()
        {
            return AppEx.Container.GetInstance<IRoleDataService>();
        }
    }
}