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
    public class UserAddWindowViewModel : BindableBase
    {
        private User cUser;
        public DelegateCommand SubmitCommand { get; set; }
        public DelegateCommand ResetCommand { get; set; }
        public UserAddWindowViewModel() 
        {
            this.cUser = new User();
            //this.SubmitCommand = new DelegateCommand(this.OnSubmit);
            //this.ResetCommand = new DelegateCommand(this.OnReset);
        }
       
        //public void OnSubmit() 
        //{
            
        //}
        //public void OnReset() 
        //{
        //}
        public User User
        {
            get { return this.cUser; }
            set { SetProperty(ref this.cUser, value); }
        }
    }
   
}

