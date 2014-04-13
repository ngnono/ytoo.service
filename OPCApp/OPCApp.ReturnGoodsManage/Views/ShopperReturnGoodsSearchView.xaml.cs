using OPCApp.ReturnGoodsManage.ViewModel;
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

namespace OPCApp.ReturnGoodsManage.Views
{
    /// <summary>
    /// ShopperReturnGoodsSearchView.xaml 的交互逻辑
    /// </summary>
    [Export("ShopperReturnGoodsSearchView", typeof(UserControl))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ShopperReturnGoodsSearchView : UserControl
    {
        public ShopperReturnGoodsSearchView()
        {
            InitializeComponent();
        }
      [Import(typeof(ShopperReturnGoodsSearchViewModel))]
        public ShopperReturnGoodsSearchViewModel ViewModel
        {
            set { DataContext = value; }
            get { return DataContext as ShopperReturnGoodsSearchViewModel; }
        }
    }
}
