using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using MahApps.Metro;
using OPCApp.TransManage.ViewModels;

namespace OPCApp.TransManage.Views
{
    /// <summary>
    /// PrintInvoiceViewModel.xaml 的交互逻辑
    /// </summary>
    /// 
    [Export("StoreOut")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class StoreOut 
    {
        public StoreOut()
        {
            InitializeComponent();
            this.DataContext = new StoreOutViewModel();
        }
    }
}
