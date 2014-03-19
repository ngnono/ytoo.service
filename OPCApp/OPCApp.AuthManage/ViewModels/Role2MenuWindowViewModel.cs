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
   [Export("Role2MenuViewModel",typeof(Role2MenuWindowViewModel))]
   public class Role2MenuWindowViewModel:BindableBase
    {
        private List<OPC_AuthMenu> _menulist;
        public List<OPC_AuthMenu> MenuList
        {

            get { return this._menulist; }
            set { SetProperty(ref this._menulist, value); }
        }
        private List<OPC_AuthUser> _rolelist;
        public List<OPC_AuthUser> RoleList
        {

            get { return this._rolelist; }
            set { SetProperty(ref this._rolelist, value); }
        }
        /*选中的用户Id*/
        private List<int> SelectedMenuIdList { get; set; }
        /**/
        private OPC_AuthRole _role;
        public OPC_AuthRole SelectedRole
        {

            get { return this._role; }
            set { SetProperty(ref this._role, value); }
        }

        public Role2MenuWindowViewModel() 
       {
           this.AuthorizationUserCommand = new DelegateCommand(this.AuthorizationUser);
           this.DeleteUserListCommand = new DelegateCommand(this.DeleteUserList);
           this.DbGridClickCommand = new DelegateCommand(this.DBGridClick);
           this.GetSelectedCommand=new DelegateCommand(this.GetSelected);
       }
       public DelegateCommand DeleteUserListCommand { get; set; }
       public DelegateCommand GetSelectedCommand { get; set; }
       public DelegateCommand AuthorizationUserCommand { get; set; }
       public DelegateCommand DbGridClickCommand { get; set; }

       private void DeleteUserList()
       {
           //liuyahua
          // var userSeleted = UserList.Where(e => e.IsSelected == true);
           MenuList.Remove(e => e.IsSelected == true);
           //UserList.Remove()
       }

       private void DBGridClick()
       {
           IRole2MenuService role2MenuService = AppEx.Container.GetInstance<IRole2MenuService>();
           if (SelectedRole == null) return;
           this.MenuList = role2MenuService.GetMenuList(SelectedRole.Id);

       }

       private void GetSelected()
       {
           this.SelectedMenuIdList =this.MenuList.Where(n => n.IsSelected == true).Select(e => e.Id).ToList();
       }

       private void AuthorizationUser()
       {
          IRole2UserService role2UserService = AppEx.Container.GetInstance<IRole2UserService>();
           if (SelectedRole == null || this.SelectedMenuIdList==null) return;
          role2UserService.SetUserByRole(SelectedRole.Id, this.SelectedMenuIdList);
       }

}
}
