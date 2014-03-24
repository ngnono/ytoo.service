using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.DataService.Interface.Trans;
using OPCAPP.Domain.Enums;
using OPCApp.Infrastructure;


namespace OPCApp.TransManage.ViewModels
{
     [Export("StoreOutViewModel", typeof(StoreOutViewModel))]
   public class StoreOutViewModel :PrintInvoiceViewModel
    {
        public DelegateCommand CommandPrintInvoice { get; set; }
        public DelegateCommand CommandPrintExpress { get; set; }
         public DelegateCommand<int?> CommandSelectionChanged { get; set; }
         public int IsTabIndex { get; set; }

        public StoreOutViewModel()
            : base()
        {
            //初始化命令属性
            this.SearchSaleStatus = EnumSearchSaleStatus.StoreOutDataBaseSearchStatus;
            //初始化命令属性
            CommandPrintInvoice = new DelegateCommand(PrintInvoice);
            CommandPrintExpress = new DelegateCommand(PrintExpress);
           // CommandSelectionChanged = new DelegateCommand<int?>(SelectionChanged);
        }
         //有问题 所以改为下面这种方式
         //public void SelectionChanged(int? i)
         //{
            
         //}
         public void SelectionChanged(object sender, SelectionChangedEventArgs e)
         {
             if (e.Source is TabControl)
             {
                 TabControl tabControl = sender as TabControl;
                 int i = tabControl.SelectedIndex;
                 switch (i)
                 {
                     case 1:
                         this.SearchSaleStatus = EnumSearchSaleStatus.PrintInvoiceSearchStatus;
                         break;
                     case 2:
                         this.SearchSaleStatus = EnumSearchSaleStatus.PrintExpressSearchStatus;
                         break;
                     default:
                         this.SearchSaleStatus = EnumSearchSaleStatus.StoreOutDataBaseSearchStatus;
                         break; ;
                 }
                 this.Refresh();
             }
           
         }
         public void PrintInvoice()
        {
            var selectSaleIds = this.SaleList.Where(n => n.IsSelected).Select(e => e.SaleOrderNo).ToList();
            ITransService ts = AppEx.Container.GetInstance<ITransService>();
            bool bFalg= ts.SetStatusPrintInvoice(selectSaleIds);
            MessageBox.Show(bFalg ? "打印发货单成功" : "打印发货单失败", "提示");
             this.Refresh();

        }
        public void PrintExpress()
        {
            var selectSaleIds = this.SaleList.Where(n => n.IsSelected).Select(e => e.SaleOrderNo).ToList();
            ITransService ts = AppEx.Container.GetInstance<ITransService>();
            bool bFalg= ts.SetStatusPrintExpress(selectSaleIds);
            MessageBox.Show(bFalg ? "打印快递单成功" : "打印快递单失败", "提示");
            this.Refresh();

        }
    }
}
