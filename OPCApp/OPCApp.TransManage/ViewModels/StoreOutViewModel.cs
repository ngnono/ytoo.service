using System;
using System.ComponentModel.Composition;
using System.Linq;
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
            this.SearchSaleStatus = EnumSearchSaleStatus.StoreInDataBaseSearchStatus;
            //初始化命令属性
            CommandPrintInvoice = new DelegateCommand(this.PrintInvoice);
            CommandPrintExpress = new DelegateCommand(this.PrintExpress);
            CommandSelectionChanged = new DelegateCommand<int?>(this.SelectionChanged);
        }

         public void SelectionChanged(int? i)
         {
             switch (i)
             {
                 case 1:
                     this.SearchSaleStatus = EnumSearchSaleStatus.PrintInvoiceSearchStatus;
                     break;
                 case 2:
                     this.SearchSaleStatus = EnumSearchSaleStatus.PrintExpressSearchStatus;
                     break;
                 default:
                     this.SearchSaleStatus = EnumSearchSaleStatus.StoreInDataBaseSearchStatus;
                     break;;
             }
         }

         public void PrintInvoice()
        {
            var selectSaleIds = this.SaleList.Where(n => n.IsSelected == true).Select(e => e.SaleOrderNo).ToList();
            ITransService ts = AppEx.Container.GetInstance<ITransService>();
            ts.SetStatusPrintInvoice(selectSaleIds);

        }
        public void PrintExpress()
        {
            var selectSaleIds = this.SaleList.Where(n => n.IsSelected == true).Select(e => e.SaleOrderNo).ToList();
            ITransService ts = AppEx.Container.GetInstance<ITransService>();
            ts.SetStatusPrintExpress(selectSaleIds);

        }
    }
}
