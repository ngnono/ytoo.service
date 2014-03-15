using System.Collections.Generic;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Commands;
using OPCApp.AuthManage.Views;
using OPCApp.Domain;
using OPCApp.DataService.Interface;
using OPCApp.DataService.Impl;

namespace OPCApp.AuthManage.ViewModels
{
   public class Role2UserWindowViewModel:BindableBase
    {
       public Role2UserWindowViewModel() 
       {
           this.AuthorizationUserCommand = new DelegateCommand(this.authorizationUserCommand);
           this.AddUserWindowCommand = new DelegateCommand(this.addUserWindowCommand);
           this.DbGridClickCommand = new DelegateCommand(this.dbGridClickCommand);
       }
       public DelegateCommand AuthorizationUserCommand { get; set; }
       public DelegateCommand AddUserWindowCommand { get; set; }
       public DelegateCommand DbGridClickCommand { get; set; }
       private void authorizationUserCommand()
       {


       }
       private void addUserWindowCommand() 
       {

       }
       private void dbGridClickCommand()
       {

       }


    }
}
