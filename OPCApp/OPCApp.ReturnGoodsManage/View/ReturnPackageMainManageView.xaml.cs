using System.ComponentModel.Composition;
using System.Windows.Controls;

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
    }
}