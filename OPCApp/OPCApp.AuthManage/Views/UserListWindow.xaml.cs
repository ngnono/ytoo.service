using System.ComponentModel.Composition;
using System.Windows.Controls;
using CustomControlLibrary;
using OPCApp.DataService.Interface;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;
using OPCApp.Infrastructure.Mvvm.View;

namespace OPCApp.AuthManage.Views
{
    /// <summary>
    ///     RoleWindow.xaml 的交互逻辑
    /// </summary>
    [Export("UserListWindow", typeof (IBaseView))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class UserListWindow : UserControl, IBaseView
    {
        public UserListWindow()
        {
            InitializeComponent();
        }
        public void Query(int size, int pageIndex)
        {
           // PageResult<OPC_AuthUser> pageResult = AppEx.Container.GetInstance<IAuthenticateService>().Search(null);

        }
        private void dataPager_PageChanged(object sender, PageChangedEventArgs args)
        {
            Query(args.PageSize, args.PageIndex);
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