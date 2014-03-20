using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Prism.Commands;
using System.Windows;
using OPCApp.TransManage.Models;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.Common;
using OPCApp.TransManage.IService;
using OPCApp.TransManage.Views;
using OPCApp.DataService.Interface.Trans;
using OPCApp.DataService.Impl.Trans;
using Intime.OPC.Domain.Models;
using OPCApp.Infrastructure;
using System.ComponentModel.Composition;


namespace OPCApp.TransManage.ViewModels
{
    [Export("PrintInvoiceViewModel", typeof(PrintInvoiceViewModel))]
    public class PrintInvoiceViewModel : BindableBase
    {
        //Grid数据集
        private IEnumerable<OPC_Sale> invoice4list;
        public IEnumerable<OPC_Sale> Invoice4List
        {

            get { return this.invoice4list; }
            set { SetProperty(ref this.invoice4list, value); }
        }
        //界面查询条件
        private Invoice4Get invoice4get;
        public Invoice4Get Invoice4Get
        {
            get { return this.invoice4get; }
            set { SetProperty(ref this.invoice4get, value); }
        }

        public DelegateCommand CommandSearch { get; set; }
        public DelegateCommand CommandViewAndPrint { get; set; }
        public DelegateCommand CommandOnlyPrint { get; set; }
        public DelegateCommand CommandFinish { get; set; }
        public DelegateCommand CommandRemark { get; set; }
        public DelegateCommand CommandGetDown { get; set; }
        public PrintInvoiceViewModel()
        {
            //初始化命令属性
            CommandSearch = new DelegateCommand(new Action(CommandSearchExecute));
            CommandViewAndPrint = new DelegateCommand(new Action(CommandViewAndPrintExecute));
            CommandOnlyPrint = new DelegateCommand(new Action(CommandOnlyPrintExecute));
            CommandFinish = new DelegateCommand(new Action(CommandFinishExecute));
            CommandRemark = new DelegateCommand(new Action(CommandRemarkExecute));
            CommandGetDown = new DelegateCommand(new Action(CommandGetDownExecute));
            invoice4get = new Invoice4Get();
        }


        public void CommandRemarkExecute()
        {
            IRemark remark = new RemarkWin();
            remark.ShowRemarkWin();
        }
        public void CommandSearchExecute()
        {
            var dic = new Dictionary<string, object>();

            dic.Add("startdate", Invoice4Get.StartSellDate.ToShortDateString());
            dic.Add("enddate", Invoice4Get.EndSellDate.ToShortDateString());
            dic.Add("orderno", Invoice4Get.OrderNo);
            dic.Add("saleorderno", Invoice4Get.SaleOrderNo);
            PageResult<OPC_Sale> re = AppEx.Container.GetInstance<ITransService>().Search(dic);
            Invoice4List = re.Result;

        }

        //销售单明细Grid数据集
        private IEnumerable<OPC_SaleDetail> invoicedetail4list;
        public IEnumerable<OPC_SaleDetail> InvoiceDetail4List
        {

            get { return this.invoicedetail4list; }
            set { SetProperty(ref this.invoicedetail4list, value); }
        }

        public void CommandGetDownExecute()
        {
            var selectdata = Invoice4List.Where(n => n.IsSelected == true).Select(e => e.Id).ToList();
            string passData = "";
            foreach (int id in selectdata)
            {
                passData += id.ToString() + ",";
            }
            if (!string.IsNullOrEmpty(passData))
            {
                var dic = new Dictionary<string, object>();
                dic.Add("ids", passData);
                InvoiceDetail4List= AppEx.Container.GetInstance<ITransService>().SelectSaleDetail(dic).Result;
            }
        }

        public void CommandViewAndPrintExecute()
        {
            //var selectdata = Invoice4List.Where(n => n.IsSelected == true).Select(e=>e.ID).ToList();
            //List<InvoiceDetail4Get> detail4get;
            //foreach (Invoice data in selectdata)
            //{
            //    detail4get
            //    detail4get.Add(data.ID);
            //}


        }
        public void CommandOnlyPrintExecute()
        {


        }
        public void CommandFinishExecute()
        {
            ITransService ITrans = new TransService();
            var dic = new Dictionary<string, string>();
            dic.Add("id", "1");
            dic.Add("status", "55");
            ITrans.Finish(dic);
        }
        //选择上面行数据时赋值的数据集
        private Invoice invoice4sel;
        public Invoice Invoice4Sel
        {
            get { return this.invoice4sel; }
            set { SetProperty(ref this.invoice4sel, value); }

        }

        
        
    }
}
