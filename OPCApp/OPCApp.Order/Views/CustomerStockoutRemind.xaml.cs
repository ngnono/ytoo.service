using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace OPCApp.Customer.Views
{
    /// <summary>
    ///     PrintInvoiceViewModel.xaml 的交互逻辑
    /// </summary>
    [Export("CustomerStockoutRemind", typeof(UserControl))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class CustomerStockoutRemind
    {
        public CustomerStockoutRemind()
        {
            InitializeComponent();
        }

        [Import("CustomerInquiriesViewModel")]
        public object ViewModel
        {
            set { DataContext = value; }
            get { return DataContext; }
        }
    }
}