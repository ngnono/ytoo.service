using System;
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
using OPCApp.Financial.ViewModels;

namespace OPCApp.Financial.Views
{
    /// <summary>
    /// WebSiteSalesStatisticsView.xaml 的交互逻辑
    /// </summary>
    [Export("WebSiteSalesStatisticsView", typeof(UserControl))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class WebSiteSalesStatisticsView : UserControl
    {
        public WebSiteSalesStatisticsView()
        {
            InitializeComponent();
        }
        [Import("WebSiteSalesStatisticsViewModel")]
        public OPCApp.Financial.ViewModels.WebSiteSalesStatisticsViewModel ViewModel
        {
            set { DataContext = value; }
            get { return DataContext as WebSiteSalesStatisticsViewModel; }
        }
    }
}
