using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Intime.OPC.Domain.Models;
using Microsoft.Practices.Prism.Commands;
using System.Windows;
using OPCApp.DataService.Interface.Trans;
using OPCApp.Infrastructure;
using OPCApp.TransManage.Models;
using Microsoft.Practices.Prism.Mvvm;


namespace OPCApp.TransManage.ViewModels
{
    [Export("StoreInViewModel", typeof(StoreInViewModel))]
  public  class StoreInViewModel : BindableBase
    {

        public DelegateCommand CommandSearch { get; set; }
        public DelegateCommand CommandSoldOut { get; set; }
        public DelegateCommand CommandStoreInSure { get; set; }
        public DelegateCommand CommandBack { get; set; }

        public StoreInViewModel()
        {
            //初始化命令属性
            CommandSearch = new DelegateCommand(new Action(CommandSearchExecute));
            CommandSoldOut = new DelegateCommand(new Action(CommandSoldOutExecute));
            CommandStoreInSure = new DelegateCommand(new Action(CommandStoreInSureExecute));
            
        }

        public void CommandBackExecute()
        {

        }
        public void CommandSearchExecute()
        {

        }
        public void CommandSoldOutExecute()
        {


        }
        private IEnumerable<OPC_Sale> invoice4list;
        public IEnumerable<OPC_Sale> Invoice4List
        {

            get { return this.invoice4list; }
            set { SetProperty(ref this.invoice4list, value); }
        }
        public void CommandStoreInSureExecute()
        {
            ITransService ts = AppEx.Container.GetInstance<ITransService>();
           // ts.StoreInSure("1");

        }
        //选择上面行数据时赋值的数据集
        private Invoice invoice4sel;
        public Invoice Invoice4Sel
        {
            get { return this.invoice4sel; }
            set { SetProperty(ref this.invoice4sel, value); }

        }

        //Grid数据集
        private List<Invoice> invoice4grid;
        public List<Invoice> InvoiceInfo4Grid
        {

            get { return this.invoice4grid; }
            set { SetProperty(ref this.invoice4grid, value); }
        }
        
    }
}
