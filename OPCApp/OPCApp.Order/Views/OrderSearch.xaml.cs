using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace OPCApp.Order.Views
{
    /// <summary>
    ///     OrderSearch.xaml 的交互逻辑
    /// </summary>
    [Export("OrderSearch1")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class OrderSearchView : UserControl
    {
        public OrderSearchView()
        {
            InitializeComponent();
        }
    }
}