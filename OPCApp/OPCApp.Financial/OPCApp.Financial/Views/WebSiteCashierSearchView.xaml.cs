using System.ComponentModel.Composition;
using System.Windows.Controls;
using OPCApp.Financial.ViewModels;

namespace OPCApp.Financial.Views
{
    /// <summary>
    ///     WebSiteCashierSearchView.xaml 的交互逻辑
    /// </summary>
    [Export("WebSiteCashierSearchView", typeof (UserControl))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class WebSiteCashierSearchView : UserControl
    {
        public WebSiteCashierSearchView()
        {
            InitializeComponent();
        }

        [Import("WebSiteCashierSearchViewModel")]
        public WebSiteCashierSearchViewModel ViewModel
        {
            set { DataContext = value; }
            get { return DataContext as WebSiteCashierSearchViewModel; }
        }
    }
}