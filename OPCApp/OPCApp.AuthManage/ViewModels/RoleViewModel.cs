﻿// ***********************************************************************
// Assembly         : OPCApp.AuthManage
// Author           : Liuyh
// Created          : 03-15-2014 14:45:59
//
// Last Modified By : Liuyh
// Last Modified On : 03-15-2014 21:01:37
// ***********************************************************************
// <copyright file="RoleViewModel.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.ComponentModel.Composition;
using OPCApp.DataService.Interface;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;
using OPCApp.Infrastructure.DataService;
using OPCApp.Infrastructure.Mvvm;

namespace OPCApp.AuthManage.ViewModels
{
    /// <summary>
    ///     Class RoleViewModel.
    /// </summary>
    [Export("RoleViewModel", typeof (IViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class RoleViewModel : BaseViewModel<OPC_AuthRole>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RoleViewModel" /> class.
        /// </summary>
        public RoleViewModel()
            : base("RoleAddView")
        {
            if (AppEx.LoginModel != null)
            {
                Model = new OPC_AuthRole {CreateUserId = AppEx.LoginModel.UserID};
            }
            else
            {
                Model = new OPC_AuthRole {CreateUserId = 0};
            }
        }

        protected override IBaseDataService<OPC_AuthRole> GetDataService()
        {
            return AppEx.Container.GetInstance<IRoleDataService>();
        }
    }
}