using System.ComponentModel.Composition;
using System.Windows.Controls;
using OPCApp.Customer.ViewModels;

namespace OPCApp.Customer.Views
{
    /// <summary>
    ///     PrintInvoiceViewModel.xaml 的交互逻辑
    /// </summary>
    [Export("CustomerSelfNetReturnGoods", typeof(UserControl))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class CustomerSelfNetReturnGoods : UserControl
    {
        public CustomerSelfNetReturnGoods()
        {
            InitializeComponent();
        }

        [Import(typeof(CustomerSelfNetReturnGoodsViewModel))]
        public object ViewModel
        {
            set { DataContext = value; }
            get { return DataContext as CustomerSelfNetReturnGoodsViewModel; }
        }
    }
}