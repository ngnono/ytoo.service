using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Prism.Commands;
using System.Windows;
using OPCApp.TransManage.Models;
using Microsoft.Practices.Prism.Mvvm;


namespace OPCApp.TransManage.ViewModels
{
    class PrintInvoiceViewModel : BindableBase
    {

        public DelegateCommand CommandSearch { get; set; }
        public DelegateCommand CommandViewAndPrint { get; set; }
        public DelegateCommand CommandOnlyPrint { get; set; }
        public DelegateCommand CommandFinish { get; set; }

        public PrintInvoiceViewModel()
        {
            //初始化命令属性
            CommandSearch = new DelegateCommand(new Action(CommandSearchExecute));
            CommandViewAndPrint = new DelegateCommand(new Action(CommandViewAndPrintExecute));
            CommandOnlyPrint = new DelegateCommand(new Action(CommandOnlyPrintExecute));
            CommandFinish = new DelegateCommand(new Action(CommandFinishExecute));
        }

        public void CommandSearchExecute()
        {


        }
        public void CommandViewAndPrintExecute()
        {


        }
        public void CommandOnlyPrintExecute()
        {


        }
        public void CommandFinishExecute()
        {
          
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
