using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using OPCApp.Customer.ViewModels;

namespace OPCApp.TransManage.Views
{
    /// <summary>
    ///     PrintInvoiceViewModel.xaml 的交互逻辑
    /// </summary>
    [Export("CustomerReturnGoods", typeof(UserControl))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class CustomerReturnGoods
    {
        public CustomerReturnGoods()
        {
            InitializeComponent();
        }

        [Import("CustomerReturnGoodsViewModel")]
        public CustomerReturnGoodsViewModel ViewModel
        {
            set { DataContext = value; }
            get { return DataContext as CustomerReturnGoodsViewModel; }
        }
    }
}