using System.Windows.Controls;
using System;
using System.ComponentModel.Composition;
using OPCApp.Infrastructure.Mvvm.View;
namespace OPCApp.AuthManage.Views
{
    /// <summary>
    /// RoleWindow.xaml 的交互逻辑
    /// </summary>
    /// 
    [Export("RoleListWindow", typeof(IBaseView))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial  class RoleListWindow :UserControl,IBaseView
    {
        
        public RoleListWindow()
        {
            InitializeComponent();
        }

        public void CloseView()
        {
            throw new NotImplementedException();
        }

        public void Cancel()
        {
            throw new NotImplementedException();
        }


        public bool? ShowDialog()
        {
            throw new NotImplementedException();
        }
    }
}
