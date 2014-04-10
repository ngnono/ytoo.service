using System.ComponentModel.Composition;
using System.Windows.Controls;
using OPCApp.ReturnGoodsManage.ViewModel;

namespace OPCApp.ReturnGoodsManage.View
{
    /// <summary>
    ///     ReturnAcceptCashierView.xaml 的交互逻辑
    /// </summary>
    /// 
    [Export("ReturnPackageMainManageView", typeof(UserControl))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ReturnPackageMainManageView : UserControl
    {
        public ReturnPackageMainManageView()
        {
            InitializeComponent();
        }
        [Import("ReturnPackageManageViewModel")]
        public ReturnPackageManageViewModel ViewModel
        {
            set { DataContext = value; }
            get { return DataContext as ReturnPackageManageViewModel; }
        }
    }
}