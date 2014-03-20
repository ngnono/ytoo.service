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
using System.Windows.Shapes;
using Microsoft.Practices.Prism.Commands;
using OPCApp.TransManage.IService;
using OPCApp.TransManage.ViewModels;

namespace OPCApp.TransManage.Views
{
    /// <summary>
    /// RemarkWin.xaml 的交互逻辑
    /// 封装的录入备注的接口
    /// </summary>
    public partial class RemarkWin : IRemark
    {
        public RemarkViewModel sv = new RemarkViewModel();
        public RemarkWin()
        {
            InitializeComponent();
            sv.CommandSave = new DelegateCommand(new Action(this.CommandSaveExecute));
            sv.CommandBack = new DelegateCommand(new Action(this.CommandBackExecute));
            this.DataContext = sv;
        }

        public void ShowRemarkWin()
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            if (this.ShowDialog() == true)
            {
                this.sv.SaveRemark();
                
            }
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
