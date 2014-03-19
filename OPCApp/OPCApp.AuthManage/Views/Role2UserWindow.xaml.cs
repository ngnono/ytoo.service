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
    public partial class Role2UserListWindow : UserControl
    {
        [Import("Role2UserViewModel")] 
        public Role2UserWindowViewModel ViewModel
        {
            get { return this.DataContext as Role2UserWindowViewModel; }
            set { this.DataContext = value; }
        }
        public Role2UserListWindow()
        {
            InitializeComponent();
        }

       
    }
}
