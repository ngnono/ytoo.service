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
using OPCApp.Infrastructure;
using OPCApp.Infrastructure.Interfaces;
using OPCApp.Infrastructure.Mvvm.View;

namespace OPCApp.Main
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    [Export("loginView",typeof(IBaseView))]
    public partial class LoginView : Window,IBaseView
    {
        public LoginView()
        {
            InitializeComponent();
        }

        public void CloseView()
        {
            DialogResult = true;
            this.Close();
        }

        public void Cancel()
        {
            DialogResult = false;
            this.Close();
        }

        public void Login()
        {
            var loginmanager = AppEx.Container.GetInstance<ILoginManager>();
            var result = loginmanager.Login("", "");
        }

    }
}
