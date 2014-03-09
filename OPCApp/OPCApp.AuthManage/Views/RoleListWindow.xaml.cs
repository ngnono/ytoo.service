using System.Windows;
using System.Windows.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
namespace OPCApp.AuthManage.Views
{
    /// <summary>
    /// RoleWindow.xaml 的交互逻辑
    /// </summary>
    /// 
    [Export("RoleListWindow")]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial  class RoleListWindow :UserControl
    {
        public RoleListWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RoleAddWindow roleWin = new RoleAddWindow();
            roleWin.ShowDialog();
        }
    }
}
