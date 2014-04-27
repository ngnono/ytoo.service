using System.ComponentModel.Composition;
using System.Windows.Controls;
using OPCApp.ReturnGoodsManage.ViewModel;

namespace OPCApp.ReturnGoodsManage.Views
{
    /// <summary>
    ///     CompletedReturnGoodsSearchView.xaml 的交互逻辑
    /// </summary>
    [Export("CompletedReturnGoodsSearchView", typeof (UserControl))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class CompletedReturnGoodsSearchView : UserControl
    {
        public CompletedReturnGoodsSearchView()
        {
            InitializeComponent();
        }

        [Import(typeof (CompletedReturnGoodsSearchViewModel))]
        public CompletedReturnGoodsSearchViewModel ViewModel
        {
            set { DataContext = value; }
            get { return DataContext as CompletedReturnGoodsSearchViewModel; }
        }
    }
}