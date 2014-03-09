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
using MahApps.Metro;
using MahApps.Metro.Controls;
using OPCApp.AuthManage.ViewModels;

namespace OPCApp.AuthManage.Views
{
    /// <summary>
    /// UserAddWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UserAddWindow : MetroWindow
    {
        public UserAddWindowViewModel userAddWin = new UserAddWindowViewModel();
        public UserAddWindow()
        {
            InitializeComponent();
            userAddWin.SubmitCommand = new Microsoft.Practices.Prism.Commands.DelegateCommand(new Action(Submit));
            userAddWin.ResetCommand = new Microsoft.Practices.Prism.Commands.DelegateCommand(new Action(Cancel));
            this.DataContext = userAddWin;

        }
        public void Submit() 
        {
            this.DialogResult = true;
            this.Close();
        }
        public void Cancel() 
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
