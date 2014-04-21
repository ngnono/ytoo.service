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
using OPCApp.Main.ViewModels;

namespace OPCApp.Main
{
    /// <summary>
    /// Config.xaml 的交互逻辑
    /// </summary>
    public partial class Config
    {
        public ConfigViewModel ViewModel
        {
            get { return DataContext as ConfigViewModel; }
            set { DataContext = value; }
        }

        [ImportingConstructor]
        public Config(ConfigViewModel configView)
        {
            InitializeComponent();
            ViewModel = configView;
            ViewModel.OKCommand=new DelegateCommand(SaveConfig);
            ViewModel.CancelCommand=new DelegateCommand(Cancel);
        }

        private void Cancel()
        {
            this.DialogResult = false;
            this.Close();
        }

        private void SaveConfig()
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
