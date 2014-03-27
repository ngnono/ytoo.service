using System.ComponentModel.Composition;
using System.Windows;
using OPCApp.Infrastructure;
using OPCApp.Infrastructure.Interfaces;
using OPCApp.Infrastructure.Mvvm.View;

namespace OPCApp.Main
{
    /// <summary>
    ///     Login.xaml 的交互逻辑
    /// </summary>
    [Export("loginView", typeof (IBaseView))]
    public partial class LoginView : Window, IBaseView
    {
        public LoginView()
        {
            InitializeComponent();
        }

        public void CloseView()
        {
            DialogResult = true;
            Close();
        }

        public void Cancel()
        {
            DialogResult = false;
            Close();
        }

        public void Login()
        {
            var loginmanager = AppEx.Container.GetInstance<ILoginManager>();
            ILoginModel result = loginmanager.Login("", "");
        }
    }
}