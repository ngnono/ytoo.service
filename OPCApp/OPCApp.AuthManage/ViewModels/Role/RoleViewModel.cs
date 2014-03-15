// ***********************************************************************
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

﻿using System.ComponentModel.Composition;
using OPCApp.Infrastructure.Mvvm;
using OPCApp.Domain;
namespace OPCApp.AuthManage.ViewModels
{
    /// <summary>
    /// Class RoleViewModel.
    /// </summary>
    [Export("RoleViewModel", typeof(IViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class RoleViewModel : BaseViewModel<Role>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoleViewModel"/> class.
        /// </summary>
        public RoleViewModel()
            : base("RoleAddView")
        {
            this.Model = new Role();
        }
    }
}
