// ***********************************************************************
// Assembly         : OPCApp.AuthManage
// Author           : Hyx
// Created          : 03-20-2014
//
// Last Modified By : Hyx
// Last Modified On : 03-20-2014
// ***********************************************************************
// <copyright file="Role2MenuWindowViewModel.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Commands;
using OPCApp.DataService.Interface;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;
using OPCApp.Infrastructure.Mvvm.View;
using OPCApp.AuthManage.Views;

/// <summary>
/// The ViewModels namespace.
/// </summary>
namespace OPCApp.AuthManage.ViewModels
{
    /// <summary>
    /// Class Role2MenuWindowViewModel.
    /// </summary>
    /// <summary>
    /// Class Role2MenuWindowViewModel.
    /// </summary>
   [Export("Role2MenuViewModel",typeof(Role2MenuWindowViewModel))]
   public class Role2MenuWindowViewModel:BindableBase
    {
        /// <summary>
        /// The _menulist
        /// </summary>
        private List<OPC_AuthMenu> _menulist;
        /// <summary>
        /// Gets or sets the menu list.
        /// </summary>
        /// <value>The menu list.</value>
        public List<OPC_AuthMenu> MenuList
        {

            get { return this._menulist; }
            set { SetProperty(ref this._menulist, value); }
        }
        /// <summary>
        /// The _rolelist
        /// </summary>
        private List<OPC_AuthUser> _rolelist;
        /// <summary>
        /// Gets or sets the role list.
        /// </summary>
        /// <value>The role list.</value>
        public List<OPC_AuthUser> RoleList
        {

            get { return this._rolelist; }
            set { SetProperty(ref this._rolelist, value); }
        }
        /*选中的用户Id*/
        /// <summary>
        /// Gets or sets the selected menu identifier list.
        /// </summary>
        /// <value>The selected menu identifier list.</value>
        private List<int> SelectedMenuIdList { get; set; }
        /**/
        /// <summary>
        /// The _role
        /// </summary>
        private OPC_AuthRole _role;
        /// <summary>
        /// Gets or sets the selected role.
        /// </summary>
        /// <value>The selected role.</value>
        public OPC_AuthRole SelectedRole
        {

            get { return this._role; }
            set { SetProperty(ref this._role, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Role2MenuWindowViewModel"/> class.
        /// </summary>
        public Role2MenuWindowViewModel() 
       {
           this.AuthorizationUserCommand    = new DelegateCommand(this.AuthorizationUser);
           this.DeleteUserListCommand       = new DelegateCommand(this.DeleteUserList);
           this.DbGridClickCommand          = new DelegateCommand(this.DBGridClick);
           this.GetSelectedCommand          =new DelegateCommand(this.GetSelected);
           this.Init();
       }

        public void Init()
        {
            IRoleDataService roleDataService = AppEx.Container.GetInstance<IRoleDataService>();
            roleDataService.Search(null);
            IMenuDataService menuDataService= AppEx.Container.GetInstance<IMenuDataService>();
            menuDataService.GetMenus();//所有的 还是有权限
        }

        /// <summary>
        /// Gets or sets the delete user list command.
        /// </summary>
        /// <value>The delete user list command.</value>
       public DelegateCommand DeleteUserListCommand { get; set; }
       /// <summary>
       /// Gets or sets the get selected command.
       /// </summary>
       /// <value>The get selected command.</value>
       public DelegateCommand GetSelectedCommand { get; set; }
       /// <summary>
       /// Gets or sets the authorization user command.
       /// </summary>
       /// <value>The authorization user command.</value>
       public DelegateCommand AuthorizationUserCommand { get; set; }
       /// <summary>
       /// Gets or sets the database grid click command.
       /// </summary>
       /// <value>The database grid click command.</value>
       public DelegateCommand DbGridClickCommand { get; set; }

       /// <summary>
       /// Deletes the user list.
       /// </summary>
       private void DeleteUserList()
       {
           MenuList.Remove(e => e.IsSelected == true);
       }

       /// <summary>
       /// Databases the grid click.
       /// </summary>
       private void DBGridClick()
       {
           IRole2MenuService role2MenuService = AppEx.Container.GetInstance<IRole2MenuService>();
           if (SelectedRole == null) return;
           this.MenuList = role2MenuService.GetMenuList(SelectedRole.Id);

       }

       /// <summary>
       /// Gets the selected.
       /// </summary>
       private void GetSelected()
       {
           this.SelectedMenuIdList =this.MenuList.Where(n => n.IsSelected == true).Select(e => e.Id).ToList();
       }

       /// <summary>
       /// Authorizations the user.
       /// </summary>
       private void AuthorizationUser()
       {
          IRole2UserService role2UserService = AppEx.Container.GetInstance<IRole2UserService>();
           if (SelectedRole == null || this.SelectedMenuIdList==null) return;
          role2UserService.SetUserByRole(SelectedRole.Id, this.SelectedMenuIdList);
       }

}
}
