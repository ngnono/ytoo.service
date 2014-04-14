using System.ComponentModel.Composition;
using OPCApp.Infrastructure.Mvvm.View;

namespace OPCApp.BaseInfoManage.Views
{
    /// <summary>
    ///     StoreManage.xaml 的交互逻辑
    /// </summary>
    [Export("StoreManageWindow", typeof (IBaseView))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class StoreManage
    {
        public StoreManage()
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