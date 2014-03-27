using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;

namespace OPCApp.BaseInfoManage.ViewModels
{
    public class NavigationItemViewModel : BindableBase
    {
        public DelegateCommand UserListCommand { get; set; }
        public DelegateCommand RoleListCommand { get; set; }

        //public void OnSubmit() 
        //{

        //}
        //public void OnReset() 
    }
}