using System.Windows;
using System.Windows.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using OPCApp.Infrastructure.Mvvm.View;
using OPCApp.AuthManage.ViewModels;
namespace OPCApp.AuthManage.Views
{
    /// <summary>
    /// RoleWindow.xaml 的交互逻辑
    /// </summary>
    /// 
    [Export("UserListWindow", typeof(UserControl))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial  class RoleListWindow :UserControl,IBaseView
    {
        public RoleListWindowViewModel rwv = new RoleListWindowViewModel();
        public RoleListWindow()
        {
            InitializeComponent();
            this.DataContext = rwv;
        }

        public void CloseView()
        {
            throw new NotImplementedException();
        }

        public void Cancel()
        {
            throw new NotImplementedException();
        }


        public bool? ShowDialog()
        {
            throw new NotImplementedException();
        }
    }
}
