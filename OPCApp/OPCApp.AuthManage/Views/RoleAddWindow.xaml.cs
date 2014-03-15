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
using  OPCApp.Infrastructure.Mvvm.View;
namespace OPCApp.AuthManage.Views
{
    /// <summary>
    /// UserAddWindow.xaml 的交互逻辑
    /// </summary>
    [Export("RoleAddWindow", typeof(IBaseView))]
    public partial class RoleAddWindow : MetroWindow,IBaseView
    {
        public RoleAddWindow()
        {
            InitializeComponent();
        }

        public void Cancel()
        {
            this.DialogResult = false;
            this.Close();
        }
    
        public void CloseView()
        {
            this.DialogResult = true;
            this.Close();
        }
}
}
