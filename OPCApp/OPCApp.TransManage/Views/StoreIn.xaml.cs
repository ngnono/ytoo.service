﻿using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using MahApps.Metro;
using OPCApp.TransManage.ViewModels;
using System.Windows.Controls;


namespace OPCApp.TransManage.Views
{
    /// <summary>
    /// PrintInvoiceViewModel.xaml 的交互逻辑
    /// </summary>
    /// 
    [Export("StoreIn", typeof(UserControl))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class StoreIn 
    {
        [Import("StoreInViewModel")]
        public object ViewModel
        {
            set
            {
                this.DataContext = value;
            }
            get
            {
                return this.DataContext;
            }
        }
        public StoreIn()
        {
            InitializeComponent();
        }
    }
}
