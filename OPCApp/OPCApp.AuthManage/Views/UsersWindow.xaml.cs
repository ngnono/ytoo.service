using MahApps.Metro.Controls;
using Microsoft.Practices.Prism.Commands;
using OPCApp.AuthManage.ViewModels;
using System.ComponentModel.Composition;

namespace OPCApp.AuthManage.Views
{
   [Export("UsersView", typeof(UsersWindow))]
   [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class UsersWindow : MetroWindow

   {


       public UsersWindow()
        {
            InitializeComponent();
            
        }
       [Import("UsersViewModel", typeof(UsersWindowViewModel))]
       public UsersWindowViewModel ViewModel
       {
           get { return this.DataContext as UsersWindowViewModel; }
           set
           {
               this.DataContext = value;
               ViewModel.CancelCommand = new DelegateCommand(this.Cancel);
               ViewModel.OkCommand = new DelegateCommand(this.CloseView);
           }
       }

        public void Cancel() 
        {
            this.DialogResult = false;
            this.Close();
        }

        public void CloseView()
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
