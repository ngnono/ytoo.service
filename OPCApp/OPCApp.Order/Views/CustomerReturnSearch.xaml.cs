using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace OPCApp.TransManage.Views
{
    /// <summary>
    ///     PrintInvoiceViewModel.xaml 的交互逻辑
    /// </summary>
    [Export("CustomerRuturnSearch", typeof(UserControl))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class CustomerRuturnSearch
    {
        public CustomerRuturnSearch()
        {
            InitializeComponent();
        }

        [Import("CustomerRuturnSearchViewModel")]
        public object ViewModel
        {
            set { DataContext = value; }
            get { return DataContext; }
        }
    }
}