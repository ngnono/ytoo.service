﻿using System;
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

        //选择上面行数据时赋值的数据集
        private OPC_Sale invoice4remark = new OPC_Sale();
        public OPC_Sale Invoice4Remark
        {
            get { return this.invoice4remark; }
            set { SetProperty(ref this.invoice4remark, value); }

        }
        //销售单明细Grid数据集
        private IEnumerable<OPC_SaleDetail> invoicedetail4list;
        public IEnumerable<OPC_SaleDetail> InvoiceDetail4List
        {

            get { return this.invoicedetail4list; }
            set { SetProperty(ref this.invoicedetail4list, value); }
        }

        public DelegateCommand CommandSearch        { get; set; }
        public DelegateCommand CommandViewAndPrint  { get; set; }
        public DelegateCommand CommandOnlyPrint     { get; set; }
        public DelegateCommand CommandFinish        { get; set; }
        public DelegateCommand CommandSetRemark     { get; set; }
        public DelegateCommand CommandGetDown       { get; set; }
        public PrintInvoiceViewModel()
        {
            //初始化命令属性
            this.CommandSearch = new DelegateCommand(this.CommandSearchExecute);
            this.CommandViewAndPrint = new DelegateCommand(this.CommandViewAndPrintExecute);
            this.CommandOnlyPrint = new DelegateCommand(this.CommandOnlyPrintExecute);
            this.CommandFinish = new DelegateCommand(this.CommandFinishExecute);
            this.CommandSetRemark = new DelegateCommand(this.CommandRemarkExecute);
            this.CommandGetDown = new DelegateCommand(this.CommandGetDownExecute);
            this.Invoice4Get = new Invoice4Get();
        }

        //调用接口打开填写Remark的窗口
        //[Import("IRemark",typeof(IRemark))]
       // public
       
        public void CommandRemarkExecute()
        {
            
            //被选择的对象
            int id = Invoice4Remark.Id;
             IRemark remarkWin=AppEx.Container.GetInstance<IRemark>();
            remarkWin.ShowRemarkWin(id, 1);
        }

        public void CommandSearchExecute()
        {

            string salesfilter = string.Format("startdate={0}&enddate={1}&orderno={2}&saleorderno={3}", Invoice4Get.StartSellDate.ToShortDateString(), Invoice4Get.EndSellDate.ToShortDateString(), Invoice4Get.OrderNo, Invoice4Get.SaleOrderNo);
            PageResult<OPC_Sale> re = AppEx.Container.GetInstance<ITransService>().Search(salesfilter);
            Invoice4List = re.Result;

        }

        public void CommandGetDownExecute()
        {
            var selectSaleIds = Invoice4List.Where(n => n.IsSelected == true).Select(e=>e.Id).ToList();
            string saleIds = selectSaleIds.Aggregate("", (current, key) => current + key+',');
            //这个工作状态
            InvoiceDetail4List = AppEx.Container.GetInstance<ITransService>().SelectSaleDetail(saleIds.Substring(0,saleIds.Length-1)).Result;
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
       

        
        
    }
}
