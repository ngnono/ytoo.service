using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OPCApp.BaseInfoManage.ViewModels;
using System.ComponentModel.Composition;

namespace OPCApp.BaseInfoManage.Views
{
    /// <summary>
    /// StoreManage.xaml 的交互逻辑
    /// </summary>

    [Export("StoreManage")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class StoreManage
    {
        public StoreManage()
        {
            InitializeComponent();
            this.DataContext = new StoreManageViewModel();
        }
    }
}
