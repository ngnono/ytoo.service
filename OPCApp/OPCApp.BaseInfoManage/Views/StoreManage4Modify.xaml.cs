using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Practices.Prism.Commands;
using OPCApp.BaseInfoManage.ViewModels;

namespace OPCApp.BaseInfoManage.Views
{
    /// <summary>
    /// StoreManage.xaml 的交互逻辑
    /// </summary>
    public partial class StoreManage4Modify
    {
        
        public StoreManage4ModifyViewModel sv = new StoreManage4ModifyViewModel();
        public StoreManage4Modify()
        {
            InitializeComponent();
         
            sv.CommandSave = new DelegateCommand(new Action(this.CommandSaveExecute));
            sv.CommandBack = new DelegateCommand(new Action(this.CommandBackExecute));
            this.DataContext = sv;
        }

        public void CommandBackExecute()
        {
            DialogResult = false;
            this.Close();
        }

        private void CommandSaveExecute()
        {
            DialogResult = true;
            this.Close();
        }
        
    }
}
