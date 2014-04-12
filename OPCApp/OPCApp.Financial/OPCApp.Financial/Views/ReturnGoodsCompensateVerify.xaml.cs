using OPCApp.Financial.ViewModels;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace OPCApp.Financial.Views
{
    /// <summary>
    ///     ReturnPackageManageView.xaml 的交互逻辑
    /// </summary>
    [Export("ReturnGoodsCompensateVerify", typeof(UserControl))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ReturnGoodsCompensateVerify : UserControl
    {

        public ReturnGoodsCompensateVerify()
        {
            InitializeComponent();

        }
        [Import("ReturnGoodsCompensateVerifyViewModel")]
        public OPCApp.Financial.ViewModels.ReturnGoodsPaymentVerifyViewModel ViewModel
        {
            set { DataContext = value; }
            get { return DataContext as ReturnGoodsPaymentVerifyViewModel; }
        }
    }
}