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
using System.Windows.Forms;
using Microsoft.Practices.Prism.Commands;
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
            SetStopRoleCommand = new DelegateCommand(SetStopRole);
        }

        public DelegateCommand SetStopRoleCommand { get; set; }

        private void SetStopRole()
        {
            if (Models == null || Models.CurrentItem == null)
            {
                MessageBox.Show("请选择要操作的角色", "提示");
                return;
            }
            var iRole = GetDataService() as IRoleDataService;
            iRole.SetIsEnable((OPC_AuthRole) Models.CurrentItem);
            SearchAction();
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