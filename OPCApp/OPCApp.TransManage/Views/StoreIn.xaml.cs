using System.ComponentModel.Composition;
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
    [Export("StoreIn",typeof(UserControl))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class StoreIn 
    {
        public StoreIn()
        {
            InitializeComponent();
            this.DataContext = new StoreInViewModel();
        }
    }
}
