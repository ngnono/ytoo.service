using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using OPCApp.DataService.Interface.Trans;
using OPCAPP.Domain.Enums;
using OPCApp.Infrastructure;


namespace OPCApp.TransManage.ViewModels
{
    [Export("StoreInViewModel", typeof(StoreInViewModel))]
  public  class StoreInViewModel :PrintInvoiceViewModel
    {


        public DelegateCommand CommandSoldOut { get; set; }
        public DelegateCommand CommandStoreInSure { get; set; }

        public StoreInViewModel():base()
        {
            this.SearchSaleStatus=EnumSearchSaleStatus.StoreInDataBaseSearchStatus;
            //初始化命令属性
            CommandSoldOut = new DelegateCommand(CommandSoldOutExecute);
            CommandStoreInSure = new DelegateCommand(CommandStoreInSureExecute);
        }
  
      
        public void CommandSoldOutExecute()
        {
            var selectSaleIds = this.SaleList.Where(n => n.IsSelected).Select(e => e.SaleOrderNo).ToList();
            ITransService ts = AppEx.Container.GetInstance<ITransService>();
            bool  bFalg= ts.SetStatusSoldOut(selectSaleIds);
            MessageBox.Show(bFalg ? "设置缺货成功" : "设置缺货失败", "提示");
            this.Refresh();

        }
  
        public void CommandStoreInSureExecute()
        {
            var selectSaleIds = this.SaleList.Where(n => n.IsSelected).Select(e => e.SaleOrderNo).ToList();
            ITransService ts = AppEx.Container.GetInstance<ITransService>();
            bool bFalg = ts.SetStatusStoreInSure(selectSaleIds);
            MessageBox.Show(bFalg ? "销售单入库成功" : "销售单入库失败", "提示");
            this.Refresh();

        }
     
    }
}
