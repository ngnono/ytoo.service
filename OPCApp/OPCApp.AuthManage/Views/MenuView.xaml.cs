using OPCApp.AuthManage.ViewModels;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OPCApp.AuthManage
{
    /// <summary>
    /// MenuView.xaml 的交互逻辑
    /// </summary>
    [Export("MenuView")]
    public partial class MenuView : UserControl
    {
   
        public MenuView()
        {
            InitializeComponent();
           // this.DataContext = mvm;
            //this.InitMenu();
            
        }
        [Import(typeof(MenuViewModel))]
        public MenuViewModel ViewMode
        {
            get {
                return this.DataContext as MenuViewModel;
            }
            set
            {
            this.DataContext = value;
        } }
        public void InitMenu()
        {
            //Expander ex = new Expander();
            //ex.VerticalAlignment = VerticalAlignment.Stretch;
            //ex.Header = "权限管理";
            //this.NavigationItemsControl.Items.Add(ex); 
        }
    }
}
