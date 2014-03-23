using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
using System.Windows.Shapes;
using MahApps.Metro;
using MahApps.Metro.Controls;
using OPCApp.AuthManage.ViewModels;

namespace OPCApp.AuthManage.Views
{
    /// <summary>
    /// Role2UserListWindow.xaml 的交互逻辑
    /// </summary>
    [Export("Role2MenuWindow", typeof(UserControl))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class Role2MenuWindow : UserControl
    {
        [Import("UsersViewModel")]
        public UsersWindowViewModel ViewModel
        {
            get { return this.DataContext as UsersWindowViewModel; }
            set { this.DataContext = value; }
        }
        public Role2MenuWindow()
        {
            InitializeComponent();
        }
       
    }
}
