using System;
using MahApps.Metro.Controls;
using OPCApp.AuthManage.ViewModels;
using System.ComponentModel.Composition;
using OPCApp.Infrastructure.Mvvm.View;

namespace OPCApp.AuthManage.Views
{
   [Export("UsersView", typeof(IBaseView))]
   [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class UsersAddWindow : MetroWindow, IBaseView
    {

       public UsersAddWindow()
        {
            InitializeComponent();

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
