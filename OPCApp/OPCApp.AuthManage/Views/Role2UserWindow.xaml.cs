using System.ComponentModel.Composition;
using System.Windows.Controls;
using OPCApp.AuthManage.ViewModels;

namespace OPCApp.AuthManage.Views
{
    /// <summary>
    /// Role2UserListWindow.xaml 的交互逻辑
    /// </summary>
    [Export("Role2UserWindow", typeof(UserControl))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
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
