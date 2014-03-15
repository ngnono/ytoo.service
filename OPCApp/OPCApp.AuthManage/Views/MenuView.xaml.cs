using System.ComponentModel.Composition;
using System.Windows.Controls;
using OPCApp.AuthManage.ViewModels;

namespace OPCApp.AuthManage.Views
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
