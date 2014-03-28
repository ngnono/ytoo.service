using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Intime.OPC.Domain.Models;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.DataService.Interface.Trans;
using OPCAPP.Domain.Enums;
using OPCApp.Infrastructure;
using OPCApp.TransManage.IService;
using OPCApp.TransManage.Models;

namespace OPCApp.TransManage.ViewModels
{
    [Export("PrintInvoiceViewModel", typeof (PrintInvoiceViewModel))]
    public class PrintInvoiceViewModel : BindableBase
    {
        //Grid数据集
        private Invoice4Get invoice4get;
        private OPC_Sale invoice4remark = new OPC_Sale();
        private List<OPC_SaleDetail> invoicedetail4list;
        private IEnumerable<OPC_Sale> saleList;

        public PrintInvoiceViewModel()
        {
            //初始化命令属性
            CommandSearch = new DelegateCommand(CommandSearchExecute);
            CommandViewAndPrint = new DelegateCommand(CommandViewAndPrintExecute);
            CommandOnlyPrint = new DelegateCommand(CommandOnlyPrintExecute);
            CommandFinish = new DelegateCommand(CommandFinishExecute);
            CommandSetRemark = new DelegateCommand(CommandRemarkExecute);
            CommandGetDown = new DelegateCommand(CommandGetDownExecute);
            Invoice4Get = new Invoice4Get();
            SearchSaleStatus = EnumSearchSaleStatus.CompletePrintSearchStatus;
        }

        public EnumSearchSaleStatus SearchSaleStatus { get; set; }

        public IEnumerable<OPC_Sale> SaleList
        {
            get { return saleList; }
            set { SetProperty(ref saleList, value); }
        }

        //界面查询条件

        public Invoice4Get Invoice4Get
        {
            get { return invoice4get; }
            set { SetProperty(ref invoice4get, value); }
        }

        //选择上面行数据时赋值的数据集

        public OPC_Sale Invoice4Remark
        {
            get { return invoice4remark; }
            set { SetProperty(ref invoice4remark, value); }
        }

        //销售单明细Grid数据集

        public List<OPC_SaleDetail> InvoiceDetail4List
        {
            get { return invoicedetail4list; }
            set { SetProperty(ref invoicedetail4list, value); }
        }

        public DelegateCommand CommandSearch { get; set; }
        public DelegateCommand CommandViewAndPrint { get; set; }
        public DelegateCommand CommandOnlyPrint { get; set; }
        public DelegateCommand CommandFinish { get; set; }
        public DelegateCommand CommandSetRemark { get; set; }
        public DelegateCommand CommandGetDown { get; set; }

        //调用接口打开填写Remark的窗口
        public void CommandRemarkExecute()
        {
            //被选择的对象
            string id = Invoice4Remark.SaleOrderNo;
            var remarkWin = AppEx.Container.GetInstance<IRemark>();
            remarkWin.ShowRemarkWin(id, 1);
        }

        public void CommandSearchExecute()
        {
            Refresh();
        }

        public void Refresh()
        {
            string salesfilter = string.Format("startdate={0}&enddate={1}&orderno={2}&saleorderno={3}",
                Invoice4Get.StartSellDate.ToShortDateString(), Invoice4Get.EndSellDate.ToShortDateString(),
                Invoice4Get.OrderNo, Invoice4Get.SaleOrderNo);
            PageResult<OPC_Sale> re = AppEx.Container.GetInstance<ITransService>().Search(salesfilter, SearchSaleStatus);
            SaleList = re.Result;
            if (InvoiceDetail4List != null) InvoiceDetail4List.Clear();
        }

        public void CommandGetDownExecute()
        {
            if(SaleList==null)return;
            OPC_Sale saleCur = SaleList.Where(n => n.IsSelected).FirstOrDefault();
            if (saleCur == null)
            {
                if (invoicedetail4list == null) return;
                invoicedetail4list.ToList().Clear();
                return;
            }
            //这个工作状态
            InvoiceDetail4List =
                AppEx.Container.GetInstance<ITransService>().SelectSaleDetail(saleCur.SaleOrderNo).Result.ToList();
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

        /*完成销售单打印*/
        public void CommandFinishExecute()
        {
            if (SaleList == null)
            {
                MessageBox.Show("请选择要打印的销售单", "提示");
                return;
            }
            List<string> selectSaleIds = SaleList.Where(n => n.IsSelected).Select(e => e.SaleOrderNo).ToList();
            var iTransService = AppEx.Container.GetInstance<ITransService>();
            bool bFalg = iTransService.SetStatusAffirmPrintSaleFinish(selectSaleIds);
            MessageBox.Show(bFalg ? "打印销售单完成" : "打印销售单失败", "提示");
            Refresh();
        }
    }
}