using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Commands;
using OPCApp.Domain;
namespace OPCApp.BaseInfoManage.ViewModels
{
   public class NavigationItemViewModel :BindableBase
    {
        public DelegateCommand UserListCommand { get; set; }
        public DelegateCommand RoleListCommand { get; set; }
        public NavigationItemViewModel() 
        {
            //this.SubmitCommand = new DelegateCommand(this.OnSubmit);
            //this.ResetCommand = new DelegateCommand(this.OnReset);
        }
       
        //public void OnSubmit() 
        //{
            
        //}
        //public void OnReset() 
   
    }
   
}

