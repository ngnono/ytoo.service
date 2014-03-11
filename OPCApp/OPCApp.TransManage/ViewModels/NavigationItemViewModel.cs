using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Commands;
using OPCAPP.Domain;
namespace OPCApp.TransManage.ViewModels
{
   public class NavigationItemViewModel :BindableBase
    {
       public DelegateCommand PrintInvoiceCommand { get; set; }
       public DelegateCommand StoreInCommand { get; set; }
       public DelegateCommand StoreOutCommand { get; set; }
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

