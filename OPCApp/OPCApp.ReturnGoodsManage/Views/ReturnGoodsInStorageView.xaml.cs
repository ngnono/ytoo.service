using System.ComponentModel.Composition;
using System.Windows.Controls;
using OPCApp.ReturnGoodsManage.ViewModel;

namespace OPCApp.ReturnGoodsManage.Views
{
    /// <summary>
    ///     ReturnGoodsInStorageView.xaml 的交互逻辑
    /// </summary>
    [Export("ReturnGoodsInStorageView", typeof (UserControl))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ReturnGoodsInStorageView : UserControl
    {
        public ReturnGoodsInStorageView()
        {
            InitializeComponent();
        }

        [Import(typeof (ReturnGoodsInStorageViewModel))]
        public ReturnGoodsInStorageViewModel ViewModel
        {
            set { DataContext = value; }
            get { return DataContext as ReturnGoodsInStorageViewModel; }
        }
    }
}