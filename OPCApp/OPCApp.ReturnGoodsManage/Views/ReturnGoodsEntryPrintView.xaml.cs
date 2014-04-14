using System.ComponentModel.Composition;
using System.Windows.Controls;
using OPCApp.ReturnGoodsManage.ViewModel;

namespace OPCApp.ReturnGoodsManage.Views
{
    /// <summary>
    ///     ReturnGoodsEntryPrintView.xaml 的交互逻辑
    /// </summary>
    [Export("ReturnGoodsEntryPrintView", typeof (UserControl))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ReturnGoodsEntryPrintView : UserControl
    {
        public ReturnGoodsEntryPrintView()
        {
            InitializeComponent();
        }

        [Import(typeof (ReturnGoodsEntryPrintViewModel))]
        public ReturnGoodsEntryPrintViewModel ViewModel
        {
            set { DataContext = value; }
            get { return DataContext as ReturnGoodsEntryPrintViewModel; }
        }
    }
}