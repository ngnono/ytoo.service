using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MahApps.Metro;
using OPCApp.TransManage.ViewModels;

namespace OPCApp.TransManage.Views
{
    /// <summary>
    /// PrintInvoiceViewModel.xaml 的交互逻辑
    /// </summary>
    /// 
    [Export("CustomerInquiries", typeof(UserControl))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class CustomerInquiries 
    {
        [Import("CustomerInquiriesViewModel")]
        public object ViewModel
        {
            set
            {
                this.DataContext = value;
            }
            get
            {
                return this.DataContext;
            }
        }
        public CustomerInquiries()
        {
            InitializeComponent();
        }
    }
}
