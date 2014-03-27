﻿using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.AuthManage.Views;
using OPCApp.DataService.Interface;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;

namespace OPCApp.AuthManage.ViewModels
{
    [Export("Role2UserViewModel", typeof (Role2UserWindowViewModel))]
    public class Role2UserWindowViewModel : BindableBase
    {
        private OPC_AuthRole _role;
        private List<OPC_AuthUser> _rolelist;
        private List<OPC_AuthUser> _userlist;

        public Role2UserWindowViewModel()
        {
            AuthorizationUserCommand = new DelegateCommand(AuthorizationUser);
            AddUserWindowCommand = new DelegateCommand(AddUsersWindow);
            DeleteUserListCommand = new DelegateCommand(DeleteUserList);
            DbGridClickCommand = new DelegateCommand(DBGridClick);
            GetSelectedCommand = new DelegateCommand(GetSelected);
            Init();
        }

        public List<OPC_AuthUser> UserList
        {
            get { return _userlist; }
            set { SetProperty(ref _userlist, value); }
        }

        public List<OPC_AuthUser> RoleList
        {
            get { return _rolelist; }
            set { SetProperty(ref _rolelist, value); }
        }

        /*选中的用户Id*/
        private List<int> SelectedUserIdList { get; set; }
        /**/

        public OPC_AuthRole SelectedRole
        {
            get { return _role; }
            set { SetProperty(ref _role, value); }
        }

        public DelegateCommand DeleteUserListCommand { get; set; }
        public DelegateCommand GetSelectedCommand { get; set; }
        public DelegateCommand AuthorizationUserCommand { get; set; }
        public DelegateCommand AddUserWindowCommand { get; set; }
        public DelegateCommand DbGridClickCommand { get; set; }

        public void Init()
        {
            var roleDataService = AppEx.Container.GetInstance<IRoleDataService>();
            roleDataService.Search(null);
            var menuDataService = AppEx.Container.GetInstance<IMenuDataService>();
            menuDataService.GetMenus(); //所有的 还是有权限
        }

        private void DeleteUserList()
        {
            //liuyahua
            // var userSeleted = UserList.Where(e => e.IsSelected == true);
            UserList.Remove(e => e.IsSelected);
            //UserList.Remove()
        }

        private void DBGridClick()
        {
            var role2UserService = AppEx.Container.GetInstance<IRole2UserService>();
            if (SelectedRole == null) return;
            UserList = role2UserService.GetUserListByRole(SelectedRole.Id);
        }

        private void GetSelected()
        {
            SelectedUserIdList = UserList.Where(n => n.IsSelected).Select(e => e.Id).ToList();
        }

        private void AuthorizationUser()
        {
            var role2UserService = AppEx.Container.GetInstance<IRole2UserService>();
            if (SelectedRole == null || SelectedUserIdList == null) return;
            role2UserService.SetUserByRole(SelectedRole.Id, SelectedUserIdList);
        }


        private void AddUsersWindow()
        {
            var obj = AppEx.Container.GetInstance<UsersWindow>("UsersView");
            if (obj.ShowDialog() == true)
            {
                UserList = obj.ViewModel.UserList;
            }
            ;
        }
    }
}