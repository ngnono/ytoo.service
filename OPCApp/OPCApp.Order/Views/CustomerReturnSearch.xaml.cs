using System.ComponentModel.Composition;
using System.Windows.Controls;
using OPCApp.Customer.ViewModels;

namespace OPCApp.TransManage.Views
{
    /// <summary>
    ///     PrintInvoiceViewModel.xaml 的交互逻辑
    /// </summary>
    [Export("CustomerReturnSearch", typeof(UserControl))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class CustomerReturnSearch
    {
        public CustomerReturnSearch()
        {
            InitializeComponent();
        }

        [Import("CustomerReturnSearchViewModel")]
        public CustomerReturnSearchViewModel ViewModel
        {
            set { DataContext = value; }
            get { return DataContext as CustomerReturnSearchViewModel; }
        }
    }
}