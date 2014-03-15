﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OPCApp.AuthManage.ViewModels;
using OPCApp.Infrastructure.Mvvm.View;

namespace OPCApp.AuthManage.Views
{
    /// <summary>
    /// RoleWindow.xaml 的交互逻辑
    /// </summary>
    ///  
    [Export("UserListWindow", typeof(IBaseView))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class UserListWindow : UserControl,IBaseView
    {
        public UserListWindow()
        {
            InitializeComponent();
        }



        public void CloseView()
        {
        }

        public void Cancel()
        {
        }

        public bool? ShowDialog()
        {
            return false;
        }
    }
}
