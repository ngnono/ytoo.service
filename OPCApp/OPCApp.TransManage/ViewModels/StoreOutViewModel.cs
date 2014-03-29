using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Commands;
using OPCApp.DataService.Interface.Trans;
using OPCAPP.Domain.Enums;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;
using OPCApp.TransManage.IService;

namespace OPCApp.TransManage.ViewModels
{
    [Export("StoreOutViewModel", typeof (StoreOutViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class StoreOutViewModel : PrintInvoiceViewModel
    {
        private IEnumerable<Order> _orderList;
        public IEnumerable<Order> OrderList
        {
            get { return _orderList; }
            set { SetProperty(ref _orderList, value); }
        }

        public StoreOutViewModel()
        {
            //初始化命令属性
            SearchSaleStatus = EnumSearchSaleStatus.StoreOutDataBaseSearchStatus;
            //初始化命令属性
            CommandPrintInvoice = new DelegateCommand(PrintInvoice);
            CommandPrintExpress = new DelegateCommand(PrintExpress);
            // CommandSelectionChanged = new DelegateCommand<int?>(SelectionChanged);
            CommandSetOrderRemark = new DelegateCommand(SetOrderRemark);
            CommandSearchOrder = new DelegateCommand(SearchOrder);
        }
        public DelegateCommand CommandPrintInvoice { get; set; }
        public DelegateCommand CommandPrintExpress { get; set; }
        public DelegateCommand CommandSetOrderRemark { get; set; }
        public DelegateCommand<int?> CommandSelectionChanged { get; set; }
        public DelegateCommand CommandSearchOrder { get; set; }
        public int IsTabIndex { get; set; }

        public void SearchOrder()
        {

            PageResult<Order> re = AppEx.Container.GetInstance<ITransService>().SearchOrderBySale(Invoice4Get.OrderNo);
            OrderList = re.Result;
            if (OrderList != null) SaleList = new List<OPC_Sale>();
        }

        public void SetOrderRemark()
        {
            //被选择的对象
            string id = Invoice4Remark.SaleOrderNo;
            var remarkWin = AppEx.Container.GetInstance<IRemark>();
            remarkWin.ShowRemarkWin(id, 3);//3填写的是订单
        }
        //有问题 所以改为下面这种方式
        //public void SelectionChanged(int? i)
        //{

        //}
        public void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                var tabControl = sender as TabControl;
                int i = tabControl.SelectedIndex;
                switch (i)
                {
                    case 1:
                        SearchSaleStatus = EnumSearchSaleStatus.PrintInvoiceSearchStatus;
                        break;
                    case 2:
                        SearchSaleStatus = EnumSearchSaleStatus.PrintExpressSearchStatus;
                        break;
                    default:
                        SearchSaleStatus = EnumSearchSaleStatus.StoreOutDataBaseSearchStatus;
                        break;
                        ;
                }
                Refresh();
            }
        }

        public void PrintInvoice()
        {
            if (SaleList == null || SaleList.Count() == 0)
            {
                MessageBox.Show("请勾选要打应销售单", "提示");
               return;
            }
            List<string> selectSaleIds = SaleList.Where(n => n.IsSelected).Select(e => e.SaleOrderNo).ToList();
            var ts = AppEx.Container.GetInstance<ITransService>();
            bool bFalg = ts.SetStatusPrintInvoice(selectSaleIds);
            MessageBox.Show(bFalg ? "打印发货单成功" : "打印发货单失败", "提示");
            Refresh();
        }

        public void PrintExpress()
        {
            if (SaleList == null || SaleList.Count() == 0)
            {
                MessageBox.Show("请勾选要打印的销售单", "提示");
                return;
            }
            List<string> selectSaleIds = SaleList.Where(n => n.IsSelected).Select(e => e.SaleOrderNo).ToList();
            var ts = AppEx.Container.GetInstance<ITransService>();
            bool bFalg = ts.SetStatusPrintExpress(selectSaleIds);
            MessageBox.Show(bFalg ? "打印快递单成功" : "打印快递单失败", "提示");
            Refresh();
        }
    }
}