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
using OPCApp.Order.ViewModels;

namespace OPCApp.Order.Views
{
    //[Export("OrderInfoSearch2")]
    //[PartCreationPolicy(CreationPolicy.NonShared)]
    /// <summary>
    /// OrderInfoSearch.xaml 的交互逻辑
    /// </summary>
    public partial class OrderInfoSearch : UserControl
    {
        public OrderInfoSearch()
        {
            InitializeComponent();
        }

        //[Import]
        public OrderInfoSearchViewModel  ViewModel
        {
            set { this.DataContext = value; }
        }
    }


}
