﻿using System.Collections.Generic;
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
using System.Collections.ObjectModel;
using Intime.OPC.Infrastructure.Mvvm.Utility;

namespace Intime.OPC.Modules.Logistics.ViewModels
{
    [Export("StoreOutViewModel", typeof (StoreOutViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class StoreOutViewModel : PrintInvoiceViewModel
    {
        private QuerySalesOrderByComposition _queryCriteriaForDeliveryOrder;
        private QuerySalesOrderByComposition _queryCriteriaForExpressReceipt;
        private QuerySalesOrderByComposition _queryCriteriaForHandOver;
        private IList<Order> _orders;
        private IList<OPC_Sale> _salesOrdersForDelivery;
        private IList<OPC_Sale> _salesOrdersForExpress;
        private ObservableCollection<OPC_ShippingSale> _deliveryOrders;
        private OPC_ShippingSale _selectedDeliveryOrder;
        private ShippingSaleCreateDto _shippingSaleCreateDto;

        public StoreOutViewModel()
        {
            QueryCriteriaForDeliveryOrder = new QuerySalesOrderByComposition { Status = EnumSaleOrderStatus.ShipInStorage };
            QueryCriteriaForExpressReceipt = new QuerySalesOrderByComposition { Status = EnumSaleOrderStatus.PrintInvoice };
            QueryCriteriaForHandOver = new QuerySalesOrderByComposition();

            //初始化命令属性
            SearchSaleStatus = EnumSearchSaleStatus.StoreOutDataBaseSearchStatus;

            QuerySalesOrderForDeliveryCommand = new AsyncDelegateCommand(OnSalesOrderForDeliveryQuery, MvvmUtility.OnException);
            QuerySalesOrderForExpressCommand = new AsyncDelegateCommand(OnSalesOrderForExpressQuery, MvvmUtility.OnException);
            CreateDeliveryOrderCommand = new AsyncDelegateCommand(OnDeliveryOrderCreate, MvvmUtility.OnException);
            PrintExpressReceiptCommand = new DelegateCommand(OnExpressReceiptPrint);
            PrintExpressCommand = new DelegateCommand(PrintExpress);
            RemarkOrderCommand = new DelegateCommand(SetOrderRemark);
            SearchOrderBySaleCommand = new AsyncDelegateCommand(SearchOrderBySale, MvvmUtility.OnException);
            RemarkShippingCommand = new DelegateCommand(SetShippingRemark);
            CreateExpressReceiptCommand = new DelegateCommand(OnExpressReceiptCreate);
            PreviewCommand = new DelegateCommand(PrintView);
            SearchExpressCommand = new DelegateCommand(GetShipSaleList);
            GetDownShipCommand = new DelegateCommand(GetDownShip);
            ShipSaleHandOverCommand = new DelegateCommand(ShippSaleHandOver);
            ShipViaList = AppEx.Container.GetInstance<ICommonInfo>().GetShipViaList();
            ShippingSaleCreateDto = new ShippingSaleCreateDto();
            PreviewDeliveryOrderCommand = new DelegateCommand(OnDeliveryOrderPreview);
            PrintDeliveryOrderCommand = new DelegateCommand(OnDeliveryOrderPrint);
            SelectAllSalesOrderForDeliveryCommand = new DelegateCommand(OnAllSalesOrderForDeliverySelect);
            LoadMoreSalesOrdersForDeliveryCommand = new AsyncDelegateCommand(OnMoreSalesOrderForDeliveryLoad, MvvmUtility.OnException);
        }

        #region Commands

        public ICommand SelectAllSalesOrderForDeliveryCommand { get; set; }
        public ICommand QuerySalesOrderForExpressCommand { get; set; }
        public ICommand QuerySalesOrderForDeliveryCommand { get; set; }
        public ICommand PrintDeliveryOrderCommand { get; set; }
        public ICommand PreviewDeliveryOrderCommand { get; set; }
        public ICommand ShipSaleHandOverCommand { get; set; }
        public ICommand CreateExpressReceiptCommand { get; set; }
        public ICommand CreateDeliveryOrderCommand { get; set; }
        public ICommand PrintExpressCommand { get; set; }
        public ICommand SearchExpressCommand { get; set; }
        public ICommand RemarkOrderCommand { get; set; }
        public ICommand SelectionChangedCommand { get; set; }
        public ICommand SearchOrderBySaleCommand { get; set; }
        public ICommand RemarkShippingCommand { get; set; }
        public ICommand GetListShipSaleCommand { get; set; }
        public ICommand PrintExpressReceiptCommand { get; set; }
        public ICommand PreviewCommand { get; set; }
        public ICommand GetDownShipCommand { get; set; }
        public ICommand LoadMoreSalesOrdersForDeliveryCommand { get; set; }

        #endregion

        #region Properties

        [Import]
        public IService<OPC_Sale> SalesOrderService { get; set; }

        [Import]
        public IService<OPC_ShippingSale> DeliveryOrderService { get; set; }

        [Import]
        public IService<Order> OrderService { get; set; }

        [Import]
        public IService<OPC_SaleDetail> SalesOrderDetailService { get; set; }

        public QuerySalesOrderByComposition QueryCriteriaForDeliveryOrder
        {
            get { return _queryCriteriaForDeliveryOrder; }
            set { SetProperty(ref _queryCriteriaForDeliveryOrder, value); }
        }

        public QuerySalesOrderByComposition QueryCriteriaForExpressReceipt
        {
            get { return _queryCriteriaForExpressReceipt; }
            set { SetProperty(ref _queryCriteriaForExpressReceipt, value); }
        }

        public QuerySalesOrderByComposition QueryCriteriaForHandOver
        {
            get { return _queryCriteriaForHandOver; }
            set { SetProperty(ref _queryCriteriaForHandOver, value); }
        }

        public OPC_ShippingSale SelectedDeliveryOrder
        {
            get { return _selectedDeliveryOrder; }
            set { SetProperty(ref _selectedDeliveryOrder, value); }
        }

        public ObservableCollection<OPC_ShippingSale> DeliveryOrders
        {
            get { return _deliveryOrders; }
            set { SetProperty(ref _deliveryOrders, value); }
        }

        public IList<Order> Orders
        {
            get { return _orders; }
            set { SetProperty(ref _orders, value); }
        }

        public IList<OPC_Sale> SalesOrdersForDelivery
        {
            get { return _salesOrdersForDelivery; }
            set { SetProperty(ref _salesOrdersForDelivery, value); }
        }

        public IList<OPC_Sale> SalesOrdersForExpress
        {
            get { return _salesOrdersForExpress; }
            set { SetProperty(ref _salesOrdersForExpress, value); }
        }

        public ShipVia ShipVia { get; set; }

        public List<ShipVia> ShipViaList { get; set; }

        public ShippingSaleCreateDto ShippingSaleCreateDto
        {
            get { return _shippingSaleCreateDto; }
            set { SetProperty(ref _shippingSaleCreateDto, value); }
        }

        public OPC_Sale SelectedSalesOrder { get; set; }

        public int IsTabIndex { get; set; }

        #endregion

        #region Command Handlers

        private void OnAllSalesOrderForDeliverySelect()
        {
            if (SalesOrdersForDelivery == null || !SalesOrdersForDelivery.Any()) return;

            SalesOrdersForDelivery.ForEach(salesOrder => salesOrder.IsSelected = true);
        }

        private void OnSalesOrderForDeliveryQuery()
        {
            _queryCriteriaForDeliveryOrder.PageIndex = 1;
            _queryCriteriaForDeliveryOrder.PageSize = 100;

            var result = SalesOrderService.Query(_queryCriteriaForDeliveryOrder);
            SalesOrdersForDelivery = result.Data;
            if (result.TotalCount == 0)
            {
                MessageBox.Show("没有符合条件的销售单","提示", MessageBoxButton.OK,MessageBoxImage.Information);
            }
        }

        private void OnMoreSalesOrderForDeliveryLoad()
        {
            _queryCriteriaForDeliveryOrder.PageIndex ++;

            var result = SalesOrderService.Query(_queryCriteriaForDeliveryOrder);
            SalesOrdersForDelivery = result.Data;
        }

        private void OnSalesOrderForExpressQuery()
        {
            var result = SalesOrderService.Query(_queryCriteriaForExpressReceipt);
            SalesOrdersForExpress = result.Data;
            if (result.TotalCount == 0)
            {
                MessageBox.Show("没有符合条件的销售单","提示", MessageBoxButton.OK,MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// 生成发货单
        /// </summary>
        private void OnDeliveryOrderCreate()
        {
            if (SalesOrdersForDelivery == null || !SalesOrdersForDelivery.Any())
            {
                MessageBox.Show("请选择至少一个销售单", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedSalesOrdersForDelivery = SalesOrdersForDelivery.Where(salesOrder => salesOrder.IsSelected);

            if (selectedSalesOrdersForDelivery.Distinct(salesOrder => salesOrder.Order.OrderNo).Count() > 1)
            {
                MessageBox.Show("所选择的销售单必须来自同一订单，否则无法生成发货单", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (selectedSalesOrdersForDelivery.Distinct(salesOrder => string.Format("{0} {1} {2}", salesOrder.Order.CustomerName, 
                salesOrder.Order.CustomerAddress, salesOrder.Order.CustomerPhone)).Count() > 1)
            {
                MessageBox.Show("所选择的销售单收货信息不一致，无法生成发货单", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var firstSalesOrder = selectedSalesOrdersForDelivery.First();

            var deliveryOrder = new OPC_ShippingSale
            {
                OrderNo = firstSalesOrder.Order.OrderNo,
                CustomerName = firstSalesOrder.Order.CustomerName,
                CustomerAddress = firstSalesOrder.Order.CustomerAddress,
                CustomerPhone = firstSalesOrder.Order.CustomerPhone,
                ShippingZipCode = firstSalesOrder.Order.PostCode
            };

            deliveryOrder = DeliveryOrderService.Create(deliveryOrder);
            selectedSalesOrdersForDelivery.ForEach(salesOrder => 
            {
                salesOrder.DeliveryOrder = deliveryOrder;
                SalesOrderService.Update(salesOrder);

                SalesOrdersForDelivery.Remove(so => so.Id == salesOrder.Id);
            });

            DeliveryOrders.Insert(0, deliveryOrder);
        }

        private void ShippSaleHandOver()
        {
            if (DeliveryOrders == null || DeliveryOrders.Count == 0)
            {
                MessageBox.Show("请选择快递单", "提示");
                return;
            }
            List<string> goodsOutCodes = DeliveryOrders.Where(e => e.IsSelected).Select(e => e.GoodsOutCode).ToList();
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
            var deliveryOrder = AppEx.Container.GetInstance<ITransService>().GetListShipSaleBySale(saleOrderNo);
            DeliveryOrders = new ObservableCollection<OPC_ShippingSale>(deliveryOrder);
        }

        #region 打印快递单

        /// <summary>
        /// 打印快递单
        /// </summary>
        private void OnExpressReceiptPrint()
        {
            if (SelectedDeliveryOrder == null)
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
                ExpressFee = SelectedDeliveryOrder.ExpressFee.ToString("f2")
            };
            pr.PrintExpress(rdlcName, printModel,true);
        }

        /// <summary>
        /// 打印快递单预览
        /// </summary>
        private void PrintView()
        {
            if (SelectedDeliveryOrder == null)
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
                ExpressFee = SelectedDeliveryOrder.ExpressFee.ToString("f2")
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
            if (SelectedDeliveryOrder == null)
            {
                MessageBox.Show("请选择快递单", "提示");
                return;
            }
            string id = SelectedDeliveryOrder.ExpressCode;
            var remarkWin = AppEx.Container.GetInstance<IRemark>();
            //填写的是快递单
            remarkWin.ShowRemarkWin(id, EnumSetRemarkType.SetShipSaleRemark); 
        }

        /// <summary>
        /// 生成快递单
        /// </summary>
        private void OnExpressReceiptCreate()
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
                DeliveryOrders = new ObservableCollection<OPC_ShippingSale>();
                GetShipSaleList();
            }
        }

        /// <summary>
        /// 查询
        /// </summary>
        private void SearchOrderBySale()
        {
            if (SelectedSalesOrder == null)
            {
                Orders = new List<Order>();
                return;
            }
            PageResult<Order> re = AppEx.Container.GetInstance<ITransService>().SearchOrderBySale(SelectedSalesOrder.OrderNo);
            Orders = re == null ? new List<Order>() : re.Result.ToList();
        }

        private void SetOrderRemark()
        {
            //被选择的对象
            string id = SelectedSalesOrder.SaleOrderNo;
            var remarkWin = AppEx.Container.GetInstance<IRemark>();
            remarkWin.ShowRemarkWin(id, EnumSetRemarkType.SetOrderRemark); //3填写的是订单
        }

        public override void RefreshOther(OPC_Sale sale)
        {
            PageResult<Order> re = AppEx.Container.GetInstance<ITransService>().SearchOrderBySale(sale.OrderNo);
            Orders = re == null ? new List<Order>() : re.Result.ToList();
            if (SearchSaleStatus == EnumSearchSaleStatus.PrintExpressSearchStatus) return;
            GetListShipSaleBySale(sale.SaleOrderNo);
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
            DeliveryOrders = new ObservableCollection<OPC_ShippingSale>();
            SaleList = new List<OPC_Sale>();
            Orders = new List<Order>();
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
        /// 查询快递单
        /// </summary>
        private void PrintExpress()
        {
            if (SelectedDeliveryOrder == null)
            {
                MessageBox.Show("请勾选要打印的发货单", "提示");
                return;
            }
            var ts = AppEx.Container.GetInstance<ITransService>();
            bool bFalg = ts.SetStatusPrintExpress(SelectedDeliveryOrder.GoodsOutCode);
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
            DeliveryOrders = new ObservableCollection<OPC_ShippingSale>(re.Result.ToList());
            if (DeliveryOrders != null && DeliveryOrders.Count > 0)
            {
                SearchRaDoc(DeliveryOrders[0]);
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
            if (DeliveryOrders == null) return;
            OPC_ShippingSale saleCur = DeliveryOrders.FirstOrDefault(n => n.IsSelected);
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
            PrintDeliveryOrder(false);
        }

        private void OnDeliveryOrderPreview()
        {
            PrintDeliveryOrder(true);
        }

        private void PrintDeliveryOrder(bool preview)
        {
            if (SelectedDeliveryOrder == null)
            {
                MessageBox.Show("请选择一张发货单", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            const string ReportName = "Print//ReportFHD.rdlc";
            IPrint reporter = new PrintWin();

            var order = SelectedDeliveryOrder.SalesOrders.First().Order;
            var salesOrderDetails = SalesOrderDetailService.Query(new QuerySalesOrderDetailBySalesOrderNo { SalesOrderNo = SelectedDeliveryOrder.OrderNo });

            reporter.PrintDeliveryOrder(ReportName, order, SelectedDeliveryOrder.SalesOrders[0], salesOrderDetails.Data, !preview);
        }

        #endregion

        #endregion
    }
}