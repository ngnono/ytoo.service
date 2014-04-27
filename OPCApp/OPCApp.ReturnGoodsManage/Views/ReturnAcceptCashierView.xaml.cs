using System.ComponentModel.Composition;
using System.Windows.Controls;
using OPCApp.ReturnGoodsManage.ViewModel;

namespace OPCApp.ReturnGoodsManage.View
{
    /// <summary>
    ///     ReturnAcceptCashierView.xaml 的交互逻辑
    /// </summary>
    [Export("ReturnAcceptCashierView", typeof (UserControl))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ReturnAcceptCashierView : UserControl
    {
        public ReturnAcceptCashierView()
        {
            InitializeComponent();
        }

        [Import(typeof (ReturnAcceptCashierViewModel))]
        public ReturnAcceptCashierViewModel ViewModel
        {
            set { DataContext = value; }
            get { return DataContext as ReturnAcceptCashierViewModel; }
        }
    }
}