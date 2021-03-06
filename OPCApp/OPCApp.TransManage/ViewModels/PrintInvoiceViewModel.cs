﻿using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.DataService.Interface.Trans;
using OPCApp.DataService.IService;
using OPCApp.Domain.Enums;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;
using OPCApp.TransManage.Models;
using OPCApp.TransManage.Print;

namespace OPCApp.TransManage.ViewModels
{
    [Export("PrintInvoiceViewModel", typeof (PrintInvoiceViewModel))]
    public class PrintInvoiceViewModel : BindableBase
    {
        private OPC_Sale _opcSale;
        private Invoice4Get invoice4Get;
        private List<OPC_SaleDetail> invoiceDetail4List;
        private IEnumerable<OPC_Sale> saleList;

        public PrintInvoiceViewModel()
        {
            EnumSetRemarkType = EnumSetRemarkType.SetSaleRemark;
            //初始化命令属性
            CommandSearch = new DelegateCommand(CommandSearchExecute);
            CommandViewAndPrint = new DelegateCommand(CommandViewAndPrintExecute);
            CommandOnlyPrint = new DelegateCommand(CommandOnlyPrintExecute);
            CommandFinish = new DelegateCommand(CommandFinishExecute);
            CommandSetRemark = new DelegateCommand(CommandRemarkExecute);
            CommandGetDown = new DelegateCommand(CommandGetDownExecute);
            CommandDbClick = new DelegateCommand(CommandDbClickExecute);
            Invoice4Get = new Invoice4Get();
            SearchSaleStatus = EnumSearchSaleStatus.CompletePrintSearchStatus;
        }

        public EnumSearchSaleStatus SearchSaleStatus { get; set; }
        //Grid数据集

        public IEnumerable<OPC_Sale> SaleList
        {
            get { return saleList; }
            set { SetProperty(ref saleList, value); }
        }

        //界面查询条件

        public Invoice4Get Invoice4Get
        {
            get { return invoice4Get; }
            set { SetProperty(ref invoice4Get, value); }
        }

        //选择上面行数据时赋值的数据集

        public OPC_Sale SaleSelected
        {
            get { return _opcSale; }
            set { SetProperty(ref _opcSale, value); }
        }

        //销售单明细Grid数据集

        public List<OPC_SaleDetail> InvoiceDetail4List
        {
            get { return invoiceDetail4List; }
            set { SetProperty(ref invoiceDetail4List, value); }
        }

        public DelegateCommand CommandSearch { get; set; }
        public DelegateCommand CommandViewAndPrint { get; set; }
        public DelegateCommand CommandOnlyPrint { get; set; }
        public DelegateCommand CommandFinish { get; set; }
        public DelegateCommand CommandSetRemark { get; set; }
        public DelegateCommand CommandGetDown { get; set; }
        public DelegateCommand CommandDbClick { get; set; }
        public EnumSetRemarkType EnumSetRemarkType { get; set; }

        private void CommandDbClickExecute()
        {
            if (SaleSelected != null)
            {
                InvoiceDetail4List =
                    AppEx.Container.GetInstance<ITransService>()
                        .SelectSaleDetail(SaleSelected.SaleOrderNo)
                        .Result.ToList();
            }
        }

        //调用接口打开填写Remark的窗口
        public void CommandRemarkExecute()
        {
            //被选择的对象
            string id = SaleSelected.SaleOrderNo;
            var remarkWin = AppEx.Container.GetInstance<IRemark>();
            remarkWin.ShowRemarkWin(id, EnumSetRemarkType.SetSaleRemark);
        }

        public void CommandSearchExecute()
        {
            Refresh();
        }

        public virtual string GetFilter()
        {
            return string.Format("startdate={0}&enddate={1}&orderno={2}&saleorderno={3}&pageIndex={4}&pageSize={5}",
                Invoice4Get.StartSellDate.ToShortDateString(),
                Invoice4Get.EndSellDate.ToShortDateString(),
                Invoice4Get.OrderNo, Invoice4Get.SaleOrderNo, 1, 50);
        }

        protected void Refresh()
        {
            PageResult<OPC_Sale> re = AppEx.Container.GetInstance<ITransService>().Search(GetFilter(), SearchSaleStatus);
            SaleList = re.Result;
            if (InvoiceDetail4List != null) InvoiceDetail4List = new List<OPC_SaleDetail>();
        }

        public virtual void ClearOtherList()
        {
        }

        public virtual void RefreshOther(OPC_Sale SaleOrderNo)
        {
        }

        public void CommandGetDownExecute()
        {
            //if (SaleList == null)return;
            // OPC_Sale saleCur = //SaleList.Where(n => n.IsSelected).FirstOrDefault();
            if (SaleSelected == null)
            {
                if (invoiceDetail4List == null) return;
                invoiceDetail4List.ToList().Clear();
                ClearOtherList();
                return;
            }
            InvoiceDetail4List =
                AppEx.Container.GetInstance<ITransService>().SelectSaleDetail(SaleSelected.SaleOrderNo).Result.ToList();
            RefreshOther(SaleSelected);
        }

        public void CommandViewAndPrintExecute()
        {
            IPrint pr = new PrintWin();
            string xsdName = "InvoiceDataSet";
            string rdlcName = "Print//PrintInvoice.rdlc";

            var invoiceModel = new PrintModel();
            var salelist = new List<OPC_Sale>();
            salelist.Add(SaleSelected);
            invoiceModel.SaleDT = salelist;
            var orderlist = new List<Order>();
            var Order = new Order();
            orderlist.Add(Order);
            invoiceModel.OrderDT = orderlist;
            invoiceModel.SaleDetailDT = InvoiceDetail4List;
            pr.Print(xsdName, rdlcName, invoiceModel, true);
        }

        /*打印销售单*/

        public void CommandOnlyPrintExecute()
        {
            if (SaleList == null || !SaleList.Any())
            {
                MessageBox.Show("请勾选要打印的销售单", "提示");
                return;
            }
            List<string> selectSaleIds = SaleList.Where(n => n.IsSelected).Select(e => e.SaleOrderNo).ToList();
            var iTransService = AppEx.Container.GetInstance<ITransService>();
            bool bFalg = iTransService.ExecutePrintSale(selectSaleIds);
            MessageBox.Show(bFalg ? "打印成功" : "打印失败", "提示");
        }

        /*完成销售单打印*/

        public void CommandFinishExecute()
        {
            if (SaleList == null || !SaleList.Any())
            {
                MessageBox.Show("请勾选要设置打印完成状态的销售单", "提示");
                return;
            }
            List<string> selectSaleIds = SaleList.Where(n => n.IsSelected).Select(e => e.SaleOrderNo).ToList();
            var iTransService = AppEx.Container.GetInstance<ITransService>();
            bool bFalg = iTransService.SetStatusAffirmPrintSaleFinish(selectSaleIds);
            MessageBox.Show(bFalg ? "设置打印销售单完成成功" : "设置打印销售单失败", "提示");
            Refresh();
        }
    }
}