﻿using System.ComponentModel.Composition;
using System.Windows.Controls;
using OPCApp.Financial.ViewModels;

namespace OPCApp.Financial.Views
{
    /// <summary>
    ///     WebSiteSalesStatisticsView.xaml 的交互逻辑
    /// </summary>
    [Export("WebSiteSalesStatisticsView", typeof (UserControl))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class WebSiteSalesStatisticsView : UserControl
    {
        public WebSiteSalesStatisticsView()
        {
            InitializeComponent();
        }

        [Import("WebSiteSalesStatisticsViewModel")]
        public WebSiteSalesStatisticsViewModel ViewModel
        {
            set { DataContext = value; }
            get { return DataContext as WebSiteSalesStatisticsViewModel; }
        }
    }
}