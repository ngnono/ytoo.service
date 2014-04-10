using System.ComponentModel.Composition;
using System.Windows.Controls;

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
        public object ViewModel
        {
            set { DataContext = value; }
            get { return DataContext; }
        }
    }
}