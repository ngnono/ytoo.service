using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace OPCApp.Customer.Views
{
    /// <summary>
    ///     PrintInvoiceViewModel.xaml 的交互逻辑
    /// </summary>
    [Export("CustomerSelfNetRuturnGoods", typeof(UserControl))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class CustomerSelfNetRuturnGoods:UserControl
    {
        public CustomerSelfNetRuturnGoods()
        {
            InitializeComponent();
        }

        [Import("CustomerRuturnGoodsViewModel")]
        public object ViewModel
        {
            set { DataContext = value; }
            get { return DataContext; }
        }
    }
}