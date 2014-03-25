using System.ComponentModel.Composition;
using MahApps.Metro.Controls;
using OPCApp.Infrastructure.Mvvm.View;

namespace OPCApp.BaseInfoManage.Views
{
    /// <summary>
    /// StoreManage.xaml 的交互逻辑
    /// </summary>
    [Export("StoreView", typeof(IBaseView))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class StoreAddWindow : MetroWindow, IBaseView
    {

        public StoreAddWindow()
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
