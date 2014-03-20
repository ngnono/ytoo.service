// ***********************************************************************
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
using OPCApp.DataService.Interface;
using OPCApp.Domain;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;
using OPCApp.Infrastructure.DataService;
using OPCApp.Infrastructure.Mvvm;
using System.ComponentModel.Composition;
namespace OPCApp.AuthManage.ViewModels
{

    /// <summary>
    /// Class RoleListViewModel.
    /// </summary>
    [Export("RoleListViewModel", typeof(IViewModel))]
    public class RoleListViewModel : BaseListViewModel<OPC_AuthRole>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoleListViewModel"/> class.
        /// </summary>
        public RoleListViewModel()
            : base("RoleListWindow")
        {
            this.EditViewModeKey = "RoleViewModel";
            this.AddViewModeKey = "RoleViewModel";
        }

        /// <summary>
        /// Gets the data service.
        /// </summary>
        /// <returns>IBaseDataService{Role}.</returns>
        protected override IBaseDataService<OPC_AuthRole> GetDataService()
        {
            return AppEx.Container.GetInstance<IRoleDataService>();
        }
       
    }

}
