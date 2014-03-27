using System.ComponentModel.Composition;
using System.Windows.Controls;
using OPCApp.AuthManage.ViewModels;

namespace OPCApp.AuthManage.Views
{
    /// <summary>
    ///     Role2UserListWindow.xaml 的交互逻辑
    /// </summary>
    [Export("Role2MenuWindow", typeof (UserControl))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class Role2MenuWindow : UserControl
    {
        public Role2MenuWindow()
        {
            InitializeComponent();
        }

        [Import("UsersViewModel")]
        public UsersWindowViewModel ViewModel
        {
            get { return DataContext as UsersWindowViewModel; }
            set { DataContext = value; }
        }
    }
}