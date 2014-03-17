using System.ComponentModel.Composition;
using System.Windows.Controls;
using OPCApp.Infrastructure.Mvvm.View;

namespace OPCApp.AuthManage.Views
{
    /// <summary>
    /// RoleWindow.xaml 的交互逻辑
    /// </summary>
    ///  
    [Export("UserListWindow", typeof(IBaseView))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class UserListWindow : UserControl,IBaseView
    {
        public UserListWindow()
        {
            InitializeComponent();
        }



        public void CloseView()
        {
        }

        public void Cancel()
        {
        }

        public bool? ShowDialog()
        {
            return false;
        }
    }
}
