using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace OPCApp.TransManage.Views
{
    /// <summary>
    ///     PrintInvoiceViewModel.xaml 的交互逻辑
    /// </summary>
    [Export("CustomerRuturnGoods", typeof(UserControl))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class CustomerRuturnGoods
    {
        public CustomerRuturnGoods()
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