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

namespace OPCApp.AuthManage.ViewModels
{
   [Export("Role2UserViewModel", typeof(Role2UserWindowViewModel))]
   public class Role2UserWindowViewModel:BindableBase
    {
        private List<OPC_AuthUser> _userlist;
        public List<OPC_AuthUser> UserList
        {

            get { return this._userlist; }
            set { SetProperty(ref this._userlist, value); }
        }
        private List<OPC_AuthUser> _rolelist;
        public List<OPC_AuthUser> RoleList
        {

            get { return this._rolelist; }
            set { SetProperty(ref this._rolelist, value); }
        }
        /*选中的用户Id*/
        private List<int> SelectedUserIdList { get; set; }
        /**/
        private OPC_AuthRole _role;
        public OPC_AuthRole SelectedRole
        {

            get { return this._role; }
            set { SetProperty(ref this._role, value); }
        }

       public Role2UserWindowViewModel() 
       {
           this.AuthorizationUserCommand = new DelegateCommand(this.AuthorizationUser);
           this.AddUserWindowCommand = new DelegateCommand(this.AddUsersWindow);
           this.DeleteUserListCommand = new DelegateCommand(this.DeleteUserList);
           this.DbGridClickCommand = new DelegateCommand(this.DBGridClick);
           this.GetSelectedCommand=new DelegateCommand(this.GetSelected);
           this.Init();
       }
       public void Init()
       {
           IRoleDataService roleDataService = AppEx.Container.GetInstance<IRoleDataService>();
           roleDataService.Search(null);
           IMenuDataService menuDataService = AppEx.Container.GetInstance<IMenuDataService>();
           menuDataService.GetMenus();//所有的 还是有权限
       }
       public DelegateCommand DeleteUserListCommand { get; set; }
       public DelegateCommand GetSelectedCommand { get; set; }
       public DelegateCommand AuthorizationUserCommand { get; set; }
       public DelegateCommand AddUserWindowCommand { get; set; }
       public DelegateCommand DbGridClickCommand { get; set; }

       private void DeleteUserList()
       {
           //liuyahua
          // var userSeleted = UserList.Where(e => e.IsSelected == true);
           UserList.Remove(e => e.IsSelected == true);
           //UserList.Remove()
       }

       private void DBGridClick()
       {
           IRole2UserService role2UserService = AppEx.Container.GetInstance<IRole2UserService>();
           if (SelectedRole == null) return;
          this.UserList= role2UserService.GetUserListByRole(SelectedRole.Id);

       }

       private void GetSelected()
       {
           this.SelectedUserIdList = UserList.Where(n => n.IsSelected == true).Select(e => e.Id).ToList();
       }

       private void AuthorizationUser()
       {
          IRole2UserService role2UserService = AppEx.Container.GetInstance<IRole2UserService>();
           if (SelectedRole == null || this.SelectedUserIdList==null) return;
          role2UserService.SetUserByRole(SelectedRole.Id, this.SelectedUserIdList);
       }

       
       private void AddUsersWindow()
       {
           UsersWindow obj = AppEx.Container.GetInstance<UsersWindow>("UsersView");
           if (obj.ShowDialog()==true)
           {
               this.UserList = obj.ViewModel.UserList;
           }
           ;
       }

    

}
}
