using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
    /// 
    [Export(typeof(IRemark))]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public partial class RemarkWin : IRemark
    {
        
        public RemarkViewModel ViewModel
        {
            set
            {
                this.DataContext = value;
            }
            get
            {
                return this.DataContext as RemarkViewModel;
            }
        }
        [ImportingConstructor]
        public RemarkWin(RemarkViewModel viewModel)
        {
            InitializeComponent();
            this.ViewModel = viewModel;
            ViewModel.CommandSave = new DelegateCommand(new Action(this.CommandSaveExecute));
            ViewModel.CommandBack = new DelegateCommand(new Action(this.CommandBackExecute));
            ViewModel.Remark.Content = "";
        }

        public void ShowRemarkWin(string id,int type)
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ViewModel.OpenWinSearch(id, type);
            if (this.ShowDialog() == true)
            {
                ViewModel.SaveRemark(id,type);
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
