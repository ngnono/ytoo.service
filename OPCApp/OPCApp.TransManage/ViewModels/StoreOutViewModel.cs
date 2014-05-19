using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Commands;
using OPCApp.DataService.Interface.Trans;
using OPCApp.DataService.IService;
using OPCApp.Domain.Dto;
using OPCApp.Domain.Enums;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;
using Intime.OPC.Modules.Logistics.Print;
using Intime.OPC.Infrastructure.Mvvm;
using Intime.OPC.Modules.Logistics.Criteria;
using Intime.OPC.Infrastructure.Service;

namespace Intime.OPC.Modules.Logistics.ViewModels
{
    [Export("StoreOutViewModel", typeof (StoreOutViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class StoreOutViewModel : PrintInvoiceViewModel
    {
        private QuerySalesOrder _queryCriteria;
        private IList<Order> _orders;
        private IList<OPC_Sale> _salesOrders;
        private IList<OPC_ShippingSale> _shipList;
        private OPC_ShippingSale _shipSale;
        private ShippingSaleCreateDto _shippingSaleCreateDto;

        public StoreOutViewModel()
        {
            //初始化命令属性
            SearchSaleStatus = EnumSearchSaleStatus.StoreOutDataBaseSearchStatus;

            QuerySalesOrderCommand = new AsyncDelegateCommand(OnSalesOrderQuery);
            PrintInvoiceCommand = new DelegateCommand(PrintInvoice);
            OnlyPrintCommand = new DelegateCommand(OnlyPrint);
            PrintExpressCommand = new DelegateCommand(PrintExpress);
            RemarkOrderCommand = new DelegateCommand(SetOrderRemark);
            SearchOrderBySaleCommand = new AsyncDelegateCommand(SearchOrderBySale);
            RemarkShippingCommand = new DelegateCommand(SetShippingRemark);
            SaveShipCommand = new DelegateCommand(SaveShipSale);
            PreviewCommand = new DelegateCommand(PrintView);
            SearchExpressCommand = new DelegateCommand(GetShipSaleList);
            GetDownShipCommand = new DelegateCommand(GetDownShip);
            ShipSaleHandOverCommand = new DelegateCommand(ShippSaleHandOver);
            ShipViaList = AppEx.Container.GetInstance<ICommonInfo>().GetShipViaList();
            ShippingSaleCreateDto = new ShippingSaleCreateDto();
            PreviewDeliveryOrderCommand = new DelegateCommand(OnDeliveryOrderPreview);
            PrintDeliveryOrderCommand = new DelegateCommand(OnDeliveryOrderPrint);
        }

        #region Commands

        public ICommand QuerySalesOrderCommand { get; set; }
        public ICommand PrintDeliveryOrderCommand { get; set; }
        public ICommand PreviewDeliveryOrderCommand { get; set; }
        public ICommand ShipSaleHandOverCommand { get; set; }
        public ICommand SaveShipCommand { get; set; }
        public ICommand PrintInvoiceCommand { get; set; }
        public ICommand PrintExpressCommand { get; set; }
        public ICommand SearchExpressCommand { get; set; }
        public ICommand RemarkOrderCommand { get; set; }
        public ICommand SelectionChangedCommand { get; set; }
        public ICommand SearchOrderBySaleCommand { get; set; }
        public ICommand RemarkShippingCommand { get; set; }
        public ICommand GetListShipSaleCommand { get; set; }
        public ICommand OnlyPrintCommand { get; set; }
        public ICommand PreviewCommand { get; set; }
        public ICommand GetDownShipCommand { get; set; }

        #endregion

        #region Properties

        [Import]
        public IService<OPC_Sale> SalesOrderService { get; set; }

        public QuerySalesOrder QueryCriteria
        {
            get { return _queryCriteria; }
            set { SetProperty(ref _queryCriteria, value); }
        }

        public OPC_ShippingSale ShipSaleSelected
        {
            get { return _shipSale; }
            set { SetProperty(ref _shipSale, value); }
        }

        public IList<OPC_ShippingSale> ShipSaleList
        {
            get { return _shipList; }
            set { SetProperty(ref _shipList, value); }
        }

        public IList<Order> Orders
        {
            get { return _orders; }
            set { SetProperty(ref _orders, value); }
        }

        public IList<OPC_Sale> SalesOrders
        {
            get { return _salesOrders; }
            set { SetProperty(ref _salesOrders, value); }
        }

        public ShipVia ShipVia { get; set; }

        public List<ShipVia> ShipViaList { get; set; }

        public ShippingSaleCreateDto ShippingSaleCreateDto
        {
            get { return _shippingSaleCreateDto; }
            set { SetProperty(ref _shippingSaleCreateDto, value); }
        }

        public int IsTabIndex { get; set; }

        #endregion

        #region Command Handlers

        private void OnSalesOrderQuery()
        {
            var result = SalesOrderService.Query(_queryCriteria);
            SalesOrders = result.Data;
            if (result.TotalCount == 0)
            {
                MessageBox.Show("没有符合条件的销售单","提示", MessageBoxButton.OK,MessageBoxImage.Information);
            }
        }

        private void ShippSaleHandOver()
        {
            if (ShipSaleList == null || ShipSaleList.Count == 0)
            {
                MessageBox.Show("请选择快递单", "提示");
                return;
            }
            List<string> goodsOutCodes = ShipSaleList.Where(e => e.IsSelected).Select(e => e.GoodsOutCode).ToList();
            if (goodsOutCodes.Count == 0)
            {
                MessageBox.Show("请选择快递单", "提示");
                return;
            }
            bool isSuccess = AppEx.Container.GetInstance<ITransService>().SetSaleOrderShipped(goodsOutCodes);
            if (isSuccess)
            {
                MessageBox.Show("完成快递发货交接成功", "提示");
            }
            ClearList();
        }

        private void GetListShipSaleBySale(string saleOrderNo)
        {
            ShipSaleList = AppEx.Container.GetInstance<ITransService>().GetListShipSaleBySale(saleOrderNo);
            ;
        }

        #region 打印快递单

        /// <summary>
        /// 打印
        /// </summary>
        private void OnlyPrint()
        {
            if (ShipSaleSelected == null)
            {
                MessageBox.Show("请选择快递单", "提示");
                return;
            }
            IPrint pr = new PrintWin();
            var rdlcName = "Print//ReportForSF.rdlc";

            var printModel = new PrintExpressModel()
            {
                CustomerAddress = Orders[0].CustomerAddress,
                CustomerName = Orders[0].CustomerName,
                CustomerPhone = Orders[0].CustomerPhone,
                ExpressFee = ShipSaleSelected.ExpressFee.ToString("f2")
            };
            pr.PrintExpress(rdlcName, printModel,true);
        }

        /// <summary>
        /// 打印预览
        /// </summary>
        private void PrintView()
        {
            if (ShipSaleSelected == null)
            {
                MessageBox.Show("请选择快递单", "提示");
                return;
            }
            IPrint pr = new PrintWin();
            var rdlcName = "Print//ReportForSF.rdlc";

            var printModel = new PrintExpressModel()
            {
                CustomerAddress = Orders[0].CustomerAddress,
                CustomerName = Orders[0].CustomerName,
                CustomerPhone = Orders[0].CustomerPhone,
                ExpressFee = ShipSaleSelected.ExpressFee.ToString("f2")
            };
            pr.PrintExpress(rdlcName, printModel);
        }

        #endregion

        /// <summary>
        /// 发货单备注
        /// </summary>
        private void SetShippingRemark()
        {
            //被选择的对象
            if (ShipSaleSelected == null)
            {
                MessageBox.Show("请选择快递单", "提示");
                return;
            }
            string id = ShipSaleSelected.ExpressCode;
            var remarkWin = AppEx.Container.GetInstance<IRemark>();
            remarkWin.ShowRemarkWin(id, EnumSetRemarkType.SetShipSaleRemark); //填写的是快递单
        }

        /// <summary>
        /// 生成发货单
        /// </summary>
        private void SaveShipSale()
        {
            if (SaleList == null) return;
            List<OPC_Sale> sale = SaleList.Where(e => e.IsSelected).ToList();
            if (sale.Count == 0)
            {
                MessageBox.Show("请勾选销售单", "提示");
                return;
            }
            _shippingSaleCreateDto.SaleOrderIDs = sale.Select(e => e.SaleOrderNo).ToList();
            _shippingSaleCreateDto.OrderNo = sale[0].OrderNo;
            _shippingSaleCreateDto.ShipViaID = ShipVia.Id;
            _shippingSaleCreateDto.ShipViaName = ShipVia.Name;
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

        /// <summary>
        /// 查询
        /// </summary>
        private void SearchOrderBySale()
        {
            //OPC_Sale sale = SaleList.FirstOrDefault(e => e.IsSelected);
            if (SaleSelected == null)
            {
                Orders = new List<Order>();
                return;
            }
            PageResult<Order> re = AppEx.Container.GetInstance<ITransService>().SearchOrderBySale(SaleSelected.OrderNo);
            Orders = re == null ? new List<Order>() : re.Result.ToList();
        }

        private void SetOrderRemark()
        {
            //被选择的对象
            string id = SaleSelected.SaleOrderNo;
            var remarkWin = AppEx.Container.GetInstance<IRemark>();
            remarkWin.ShowRemarkWin(id, EnumSetRemarkType.SetOrderRemark); //3填写的是订单
        }

        public override void RefreshOther(OPC_Sale sale)
        {
            PageResult<Order> re = AppEx.Container.GetInstance<ITransService>().SearchOrderBySale(sale.OrderNo);
            Orders = re == null ? new List<Order>() : re.Result.ToList();
            if (SearchSaleStatus == EnumSearchSaleStatus.PrintExpressSearchStatus) return;
            GetListShipSaleBySale(sale.SaleOrderNo);
            ;
        }

        public override void ClearOtherList()
        {
            ClearList();
        }

        /// <summary>
        /// 清空所有默认列表值
        /// </summary>
        private void ClearList()
        {
            ShipSaleList = new List<OPC_ShippingSale>();
            SaleList = new List<OPC_Sale>();
            Orders = new List<Order>();
           // InvoiceDetail4List = new List<OPC_SaleDetail>();
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

        /// <summary>
        /// 打印发货单完成
        /// </summary>
        private void PrintInvoice()
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

        /// <summary>
        /// 查询快递单
        /// </summary>
        private void PrintExpress()
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
                ClearList();
                Refresh();
            }
        }

        /// <summary>
        /// 查询快递单
        /// </summary>
        private void GetShipSaleList()
        {
            string filter = string.Format("startdate={0}&enddate={1}&orderno={2}",
                Invoice4Get.StartSellDate.ToShortDateString(),
                Invoice4Get.EndSellDate.ToShortDateString(),
                Invoice4Get.OrderNo);
            PageResult<OPC_ShippingSale> re = AppEx.Container.GetInstance<ITransService>().GetListShip(filter);
            if (re == null || re.Result == null || re.Result.ToList().Count == 0) return;
            ShipSaleList = re.Result.ToList();
            if (ShipSaleList != null && ShipSaleList.Count > 0)
            {
                SearchRaDoc(ShipSaleList[0]);
            }
        }

        /// <summary>
        /// 查询快递单关联单据
        /// </summary>
        /// <param name="opcShippingSale"></param>
        private void SearchRaDoc(OPC_ShippingSale opcShippingSale)
        {
            if (opcShippingSale == null) return;
            //SaleList = AppEx.Container.GetInstance<ITransService>().SelectSaleByShip(opcShippingSale.GoodsOutCode);
            SaleList = AppEx.Container.GetInstance<ITransService>().QuerySaleOrderByShippingId(opcShippingSale.Id);
            if (SaleList != null && SaleList.Any())
            {
                OPC_Sale sale = SaleList.FirstOrDefault();
                PageResult<Order> re1 = AppEx.Container.GetInstance<ITransService>().SearchOrderBySale(sale.OrderNo);
                Orders = re1 == null ? new List<Order>() : re1.Result.ToList();
                //InvoiceDetail4List =
                //    AppEx.Container.GetInstance<ITransService>().SelectSaleDetail(sale.SaleOrderNo).Result.ToList();
            }
        }

        private void GetDownShip()
        {
            if (ShipSaleList == null) return;
            OPC_ShippingSale saleCur = ShipSaleList.FirstOrDefault(n => n.IsSelected);
            if (saleCur == null)
            {
                SaleList = new List<OPC_Sale>();
                Orders = new List<Order>();
                //InvoiceDetail4List = new List<OPC_SaleDetail>();
                return;
            }
            SearchRaDoc(saleCur);
        }

        #region 打印发货单

        private void OnDeliveryOrderPrint()
        {
            FHDPrintViewCommon(true);
        }

        private void FHDPrintViewCommon(bool bPrint)
        {
            if (SaleList == null)
            {
                MessageBox.Show("请选择销售单", "提示");
                return;
            }
            IPrint pr = new PrintWin();
            var rdlcName = "Print//ReportFHD.rdlc";

            var saleSelecteds = SaleList.Where(e => e.IsSelected).ToList();
            if (saleSelecteds.Count != 1)
            {
                MessageBox.Show("请选择一张单据进行打印", "提示");
                return;
            }
            var listOpcSaleDetails = new List<OPC_SaleDetail>();
            if (SaleSelected != null)
            {
                listOpcSaleDetails =
                    AppEx.Container.GetInstance<ITransService>()
                        .SelectSaleDetail(SaleSelected.SaleOrderNo)
                        .Result.ToList();
                PageResult<Order> re = AppEx.Container.GetInstance<ITransService>().SearchOrderBySale(SaleSelected.OrderNo);

                pr.PrintFHD(rdlcName, re.Result.FirstOrDefault(), saleSelecteds[0], listOpcSaleDetails, bPrint);
            }

        }

        private void OnDeliveryOrderPreview()
        {
            FHDPrintViewCommon(false);
        }

        #endregion

        #endregion
    }
}