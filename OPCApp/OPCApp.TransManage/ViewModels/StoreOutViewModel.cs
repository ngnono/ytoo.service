using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Microsoft.Practices.Prism.Commands;
using OPCApp.DataService.Interface.Trans;
using OPCApp.Domain.Dto;
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
        private List<OPC_ShippingSale> _shipList;
        public List<OPC_ShippingSale> ShipSaleList
        {
            get { return _shipList; }
            set { SetProperty(ref _shipList, value); }
        }
        private List<Order> _orderList;
        public List<Order> OrderList
        {
            get { return _orderList; }
            set { SetProperty(ref _orderList, value); }
        }

        public ShipVia ShipVia { get; set; }
        public List<ShipVia> ShipViaList { get; set; }

        public StoreOutViewModel()
        {
            //初始化命令属性
            SearchSaleStatus = EnumSearchSaleStatus.StoreOutDataBaseSearchStatus;
            //初始化命令属性
            CommandPrintInvoice = new DelegateCommand(PrintInvoice);
            CommandPrintExpress = new DelegateCommand(PrintExpress);
            CommandSetOrderRemark = new DelegateCommand(SetOrderRemark);
            CommandSearchOrderBySale = new DelegateCommand(SearchOrderBySale);
            CommandSetShippingRemark = new DelegateCommand(SetShippingRemark);
            CommandSaveShip = new DelegateCommand(SaveShip);
            ShipViaList= AppEx.Container.GetInstance<ICommonInfo>().GetShipViaList();
            ShippingSaleCreateDto = new ShippingSaleCreateDto();
            ShipVia = new ShipVia();
        }

      
        //界面查询条件
        private ShippingSaleCreateDto shippingSaleCreateDto;

        public ShippingSaleCreateDto ShippingSaleCreateDto
        {
            get { return shippingSaleCreateDto; }
            set { SetProperty(ref shippingSaleCreateDto, value); }
        }

        //发货单备注
        private void SetShippingRemark()
        {
            //被选择的对象
            string id = Invoice4Remark.SaleOrderNo;
            var remarkWin = AppEx.Container.GetInstance<IRemark>();
            remarkWin.ShowRemarkWin(id, EnumSetRemarkType.SetShipSaleRemark);//4填写的是订单
        }

        /*生成*/
        //发货单
        public void SaveShip()
        {
            var sale = SaleList.Where(e => e.IsSelected).ToList();
            if (sale == null||sale.Count==0)
            {
                MessageBox.Show("请勾选销售单", "提示");
                return;
            }
            shippingSaleCreateDto.SaleOrderIDs = sale.Select(e => e.SaleOrderNo).ToList();
            shippingSaleCreateDto.ShipViaID = ShipVia.Id;
            shippingSaleCreateDto.ShipViaName = ShipVia.Name;
            var isSuccess=AppEx.Container.GetInstance<ITransService>().SaveShip(ShippingSaleCreateDto);
            MessageBox.Show(isSuccess ? "生成发货单成功" : "生成发货单失败", "提示");
        }

        public DelegateCommand CommandSaveShip { get; set; }
        public DelegateCommand CommandPrintInvoice { get; set; }
        public DelegateCommand CommandPrintExpress { get; set; }
        public DelegateCommand CommandSetOrderRemark { get; set; }
        public DelegateCommand<int?> CommandSelectionChanged { get; set; }
        public DelegateCommand CommandSearchOrderBySale { get; set; }
        public DelegateCommand CommandSetShippingRemark { get; set; }
        public int IsTabIndex { get; set; }

        public void SearchOrderBySale()
        {
            var sale = SaleList.FirstOrDefault(e => e.IsSelected);
            if (sale == null) return;
            PageResult<Order> re = AppEx.Container.GetInstance<ITransService>().SearchOrderBySale(sale.OrderNo);
            OrderList =re==null?new List<Order>():re.Result.ToList();
        }

        public void SetOrderRemark()
        {
            //被选择的对象
            string id = Invoice4Remark.SaleOrderNo;
            var remarkWin = AppEx.Container.GetInstance<IRemark>();
            remarkWin.ShowRemarkWin(id,EnumSetRemarkType.SetOrderRemark);//3填写的是订单
        }

        public override void RefreshOther(OPC_Sale sale)
        {
            PageResult<Order> re = AppEx.Container.GetInstance<ITransService>().SearchOrderBySale(sale.OrderNo);
            OrderList = re == null ? new List<Order>() : re.Result.ToList();
        }
        public override void ClearOtherList()
        {
            OrderList = new List<Order>();
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
            OrderList = new List<Order>();
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