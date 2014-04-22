using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using OPCApp.Infrastructure;

namespace OPCApp.Main
{
    /// <summary>
    ///     Login.xaml 的交互逻辑
    /// </summary>
    [Export]
    public partial class Login : Window
    {
        // [Import(typeof (ILoginManager))] public ILoginManager LoginManager;

        public Login()
        {
            InitializeComponent();
            _init();
        }


        public void _init()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }


        private void login_Click(object sender, RoutedEventArgs e)
        {
            Logon();
        }

        private void Logon()
        {
            string name = logonName.Text;
            string pwd = logonPwd.Password;
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("用户名为空", "提示");
                return;
            }
            if (string.IsNullOrEmpty(pwd))
            {
                MessageBox.Show("用户名为空", "提示");
                return;
            }
            bool bl = false;
            try
            {
                bl = AppEx.Login(name, pwd);
            }
            catch
            {

                var config = AppEx.Container.GetInstance<Config>();
                if (config.ShowDialog() == true)
                {
                    config.WirteConfig();
                }
            }
            if (!bl)
            {
                if (AppEx.LoginModel !=null && AppEx.LoginModel.ErrorCode >= 400)
                {
                    try
                    {
                        var config = AppEx.Container.GetInstance<Config>();
                        if (config.ShowDialog() == true)
                        {
                            config.WirteConfig();
                            //重启应用

                        }
                    }
                    catch(Exception Ex)
                    {
                        MessageBox.Show("修改配置文件，保存失败，请把配置文件只读属性去掉！");
                    }
                    return;
                }
                MessageBox.Show("用户名或者密码错误", "提示");
                return;
            }

            DialogResult = true;
        }


        private void tips_close_MouseEnter(object sender, MouseEventArgs e)
        {
            var lb_1 = (Label) sender;
            try
            {
                var ib1 = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), "/Image/cancel_1.png")));

                ib1.Stretch = Stretch.Fill;


                lb_1.Background = ib1;
            }
            catch (Exception ef)
            {
                MessageBox.Show("出现错误！：" + ef);
            }
        }

        private void tips_close_MouseLeave(object sender, MouseEventArgs e)
        {
            var lb_1 = (Label) sender;
            try
            {
                var ib1 = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), "/Image/cancel.png")));

                ib1.Stretch = Stretch.Fill;


                lb_1.Background = ib1;
            }
            catch (Exception ef)
            {
                MessageBox.Show("出现错误！：" + ef);
            }
        }

        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btn_login_MouseEnter(object sender, MouseEventArgs e)
        {
            // Button btn_login = (Button)sender;
            // Label lb1 = (Label)btn_login.Template.FindName("tips_for_login", btn_login);
            //lb1.Foreground = new SolidColorBrush(Colors.Red);
        }

        private void btn_login_MouseLeave(object sender, MouseEventArgs e)
        {
            var btn_login = (Button) sender;
            var lb1 = (Label) btn_login.Template.FindName("tips_for_login", btn_login);
            lb1.Foreground = new SolidColorBrush(Colors.White);
        }


        private void logonPwd_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Logon();
            }
        }
    }
}