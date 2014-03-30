using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
        private List<Order> _orderList;
        private List<OPC_ShippingSale> _shipList;
        private OPC_ShippingSale _shipSale;
        private ShippingSaleCreateDto shippingSaleCreateDto;

        public StoreOutViewModel()
        {
            //初始化命令属性
            SearchSaleStatus = EnumSearchSaleStatus.StoreOutDataBaseSearchStatus;
            //初始化命令属性
            //tab1 打印发货单
            CommandPrintInvoice = new DelegateCommand(PrintInvoice);
            CommandOnlyPrint = new DelegateCommand(OnlyPrint);
            CommandPrintExpress = new DelegateCommand(PrintExpress);
            CommandSetOrderRemark = new DelegateCommand(SetOrderRemark);
            CommandSearchOrderBySale = new DelegateCommand(SearchOrderBySale);
            CommandSetShippingRemark = new DelegateCommand(SetShippingRemark);
            CommandSaveShip = new DelegateCommand(SaveShipSale);
            CommandPrintView = new DelegateCommand(PrintView);
            CommandSearchExpress = new DelegateCommand(GetShipSaleList);
            CommandGetDownShip = new DelegateCommand(GetDownShip);
            CommandShippSaleHandOver = new DelegateCommand(ShippSaleHandOver);
            ShipViaList = AppEx.Container.GetInstance<ICommonInfo>().GetShipViaList();
            ShippingSaleCreateDto = new ShippingSaleCreateDto();
           
        
        }

        private void ShippSaleHandOver()
        {
            if (ShipSaleList == null) return;
            var shipSale = ShipSaleList.Where(e => e.IsSelected).ToList();
            if (shipSale.Count==0)
            {
                MessageBox.Show("请选择快递单", "提示");
                return;
            }
            var shipNum = shipSale.Select(e => e.SaleOrderNo).ToList();
            var isSuccess = AppEx.Container.GetInstance<ITransService>().SetSaleOrderShipped(shipNum);
            if (isSuccess)
            {
                MessageBox.Show("完成快递发货交接成功", "提示");
                return;
            }
            else
            {
                this.ClearList();

            }

        }

        public OPC_ShippingSale ShipSaleSelected
        {
            get { return _shipSale; }
            set { SetProperty(ref _shipSale, value); }
        }

        public List<OPC_ShippingSale> ShipSaleList
        {
            get { return _shipList; }
            set { SetProperty(ref _shipList, value); }
        }

        public List<Order> OrderList
        {
            get { return _orderList; }
            set { SetProperty(ref _orderList, value); }
        }

        public ShipVia ShipVia { get; set; }
        public List<ShipVia> ShipViaList { get; set; }
        public DelegateCommand CommandShippSaleHandOver { get; set; }
        public DelegateCommand CommandSaveShip { get; set; }
        public DelegateCommand CommandPrintInvoice { get; set; }
        public DelegateCommand CommandPrintExpress { get; set; }
        public DelegateCommand CommandSearchExpress { get; set; }
        public DelegateCommand CommandSetOrderRemark { get; set; }
        public DelegateCommand<int?> CommandSelectionChanged { get; set; }
        public DelegateCommand CommandSearchOrderBySale { get; set; }
        public DelegateCommand CommandSetShippingRemark { get; set; }
        public DelegateCommand CommandGetListShipSale { get; set; }
        public DelegateCommand CommandOnlyPrint { get; set; }/*打印待加的命令*/
        public DelegateCommand CommandPrintView { get; set; }
        public DelegateCommand CommandGetDownShip { get; set; }


        public ShippingSaleCreateDto ShippingSaleCreateDto
        {
            get { return shippingSaleCreateDto; }
            set { SetProperty(ref shippingSaleCreateDto, value); }
        }

        public int IsTabIndex { get; set; }

        public void GetListShipSaleBySale(string saleOrderNo)
        {
            ShipSaleList = AppEx.Container.GetInstance<ITransService>().GetListShipSaleBySale(saleOrderNo);
            ;
        }

        /*打印*/

        public void OnlyPrint()
        {
//李写
        }

        /*打印预览*/

        private void PrintView()
        {
        }

        //发货单备注
        private void SetShippingRemark()
        {
            //被选择的对象
            string id = SaleSelected.SaleOrderNo;
            var remarkWin = AppEx.Container.GetInstance<IRemark>();
            remarkWin.ShowRemarkWin(id, EnumSetRemarkType.SetShipSaleRemark); //4填写的是订单
        }

        /*生成*/
        //发货单
        public void SaveShipSale()
        {
            if (SaleList == null) return;
            List<OPC_Sale> sale = SaleList.Where(e => e.IsSelected).ToList();
            if (sale.Count == 0)
            {
                MessageBox.Show("请勾选销售单", "提示");
                return;
            }
            shippingSaleCreateDto.SaleOrderIDs = sale.Select(e => e.SaleOrderNo).ToList();
            shippingSaleCreateDto.ShipViaID = ShipVia.Id;
            shippingSaleCreateDto.ShipViaName = ShipVia.Name;
            bool isSuccess = AppEx.Container.GetInstance<ITransService>().SaveShip(ShippingSaleCreateDto);
            MessageBox.Show(isSuccess ? "生成发货单成功" : "生成发货单失败", "提示");
            if (isSuccess)
            {
                GetListShipSaleBySale(sale[0].SaleOrderNo);
            }
            else
            {
                ShipSaleList = new List<OPC_ShippingSale>();
                GetShipSaleList();
            }
        }


        public void SearchOrderBySale()
        {
            //OPC_Sale sale = SaleList.FirstOrDefault(e => e.IsSelected);
            if (SaleSelected == null)
            {
                this.OrderList = new List<Order>();
                return;
            }
            PageResult<Order> re = AppEx.Container.GetInstance<ITransService>().SearchOrderBySale(SaleSelected.OrderNo);
            OrderList = re == null ? new List<Order>() : re.Result.ToList();
        }

        public void SetOrderRemark()
        {
            //被选择的对象
            string id = SaleSelected.SaleOrderNo;
            var remarkWin = AppEx.Container.GetInstance<IRemark>();
            remarkWin.ShowRemarkWin(id, EnumSetRemarkType.SetOrderRemark); //3填写的是订单
        }

        public override void RefreshOther(OPC_Sale sale)
        {
            PageResult<Order> re = AppEx.Container.GetInstance<ITransService>().SearchOrderBySale(sale.OrderNo);
            OrderList = re == null ? new List<Order>() : re.Result.ToList();
            if (SearchSaleStatus == EnumSearchSaleStatus.PrintExpressSearchStatus) return;
            GetListShipSaleBySale(sale.SaleOrderNo);
            ;
        }

        public override void ClearOtherList()
        {
           this.ClearList();
        }

        //清空所有默认列表值
        public void ClearList()
        {
            ShipSaleList = new List<OPC_ShippingSale>();
            SaleList = new List<OPC_Sale>();
            OrderList = new List<Order>();
            InvoiceDetail4List = new List<OPC_SaleDetail>();
        }

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
                        ClearList();
                        Refresh();
                        break;
                    case 2:
                        SearchSaleStatus = EnumSearchSaleStatus.PrintExpressSearchStatus;

                        ClearList();
                        GetShipSaleList();
                        break;
                    default:
                        ClearList();
                        SearchSaleStatus = EnumSearchSaleStatus.StoreOutDataBaseSearchStatus;
                        Refresh();
                        break;
                        ;
                }
            }
        }

        //打印发货单
        public void PrintInvoice()
        {
            if (SaleList == null || !SaleList.Any())
            {
                MessageBox.Show("请勾选要打应销售单", "提示");
                return;
            }
            List<string> selectSaleIds = SaleList.Where(n => n.IsSelected).Select(e => e.SaleOrderNo).ToList();
            var ts = AppEx.Container.GetInstance<ITransService>();
            bool bFalg = ts.SetStatusPrintInvoice(selectSaleIds);
            MessageBox.Show(bFalg ? "打印发货单成功" : "打印发货单失败", "提示");
            ClearList();
            Refresh();
        }

        public void PrintExpress()
        {
            if (ShipSaleSelected == null)
            {
                MessageBox.Show("请勾选要打印的发货单", "提示");
                return;
            }
            var ts = AppEx.Container.GetInstance<ITransService>();
            bool bFalg = ts.SetStatusPrintExpress(ShipSaleSelected.GoodsOutCode);
            MessageBox.Show(bFalg ? "打印快递单成功" : "打印快递单失败", "提示");
         
            if (bFalg)
            {  
                this.ClearList();
                Refresh();
               
            }
        }

        /*查询快递单*/

        public void GetShipSaleList()
        {
            string filter = string.Format("startdate={0}&enddate={1}&shippingCode={2}",
                Invoice4Get.StartSellDate.ToShortDateString(),
                Invoice4Get.EndSellDate.ToShortDateString(),
                Invoice4Get.OrderNo);
            PageResult<OPC_ShippingSale> re = AppEx.Container.GetInstance<ITransService>().GetListShip(filter);
            ShipSaleList = re.Result.ToList();
            OrderList = new List<Order>();
            SaleList = new List<OPC_Sale>();
            InvoiceDetail4List = new List<OPC_SaleDetail>();
        }

        public void GetDownShip()
        {
            if (ShipSaleList == null) return;
            OPC_ShippingSale saleCur = ShipSaleList.FirstOrDefault(n => n.IsSelected);
            if (saleCur == null)
            {
                SaleList = new List<OPC_Sale>();
                InvoiceDetail4List = new List<OPC_SaleDetail>();
                OrderList = new List<Order>();
                return;
            }
            SaleList = AppEx.Container.GetInstance<ITransService>().SelectSaleByShip(saleCur.GoodsOutCode);
        }

    }
}