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
namespace OPCApp.AuthManage.ViewModels
{
   public class AuthNavigationItemViewModel :BindableBase
    {
       public DelegateCommand<string> MenuClickCommand { get; set; }
       public DelegateCommand ClickCommand { get; set; }
        
        public AuthNavigationItemViewModel() 
        {
        }
       
        //public void OnSubmit() 
        //{
            
        //}
        //public void OnReset() 
   
    }
   
}

