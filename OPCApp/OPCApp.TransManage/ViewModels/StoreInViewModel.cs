using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Intime.OPC.Domain.Models;
using Microsoft.Practices.Prism.Commands;
using System.Windows;
using OPCApp.DataService.Interface.Trans;
using OPCAPP.Domain.Enums;
using OPCApp.Infrastructure;
using OPCApp.TransManage.Models;
using Microsoft.Practices.Prism.Mvvm;


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
            CommandSoldOut = new DelegateCommand(new Action(CommandSoldOutExecute));
            CommandStoreInSure = new DelegateCommand(new Action(CommandStoreInSureExecute));
        }
  
      
        public void CommandSoldOutExecute()
        {
            var selectSaleIds = this.SaleList.Where(n => n.IsSelected == true).Select(e => e.SaleOrderNo).ToList();
            ITransService ts = AppEx.Container.GetInstance<ITransService>();
            ts.SetStatusSoldOut(selectSaleIds);

        }
  
        public void CommandStoreInSureExecute()
        {
            var selectSaleIds = this.SaleList.Where(n => n.IsSelected == true).Select(e => e.SaleOrderNo).ToList();
            ITransService ts = AppEx.Container.GetInstance<ITransService>();
            ts.SetStatusStoreInSure(selectSaleIds);

        }
     
    }
}
