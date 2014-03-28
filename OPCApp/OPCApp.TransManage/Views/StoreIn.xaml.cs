using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace OPCApp.TransManage.Views
{
    /// <summary>
    ///     PrintInvoiceViewModel.xaml 的交互逻辑
    /// </summary>
    [Export("StoreIn", typeof (UserControl))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class StoreIn
    {
        public StoreIn()
        {
            InitializeComponent();
        }

        [Import("StoreInViewModel")]
        public object ViewModel
        {
            set { DataContext = value; }
            get { return DataContext; }
        }
    }
}