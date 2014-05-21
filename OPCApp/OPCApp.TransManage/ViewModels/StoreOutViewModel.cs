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
using System.Collections.ObjectModel;
using Intime.OPC.Infrastructure.Mvvm.Utility;
using OPCApp.Domain;
using Intime.OPC.Modules.Logistics.Services;
using Intime.OPC.Modules.Logistics.Enums;

namespace Intime.OPC.Modules.Logistics.ViewModels
{
    [Export("StoreOutViewModel", typeof (StoreOutViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class StoreOutViewModel : ValidatableBindableBase
    {
        private QuerySalesOrderByComposition _queryCriteriaForDeliveryOrder;
        private QueryDeliveryOrderByComposition _queryCriteriaForExpressReceipt;
        private QueryDeliveryOrderByComposition _queryCriteriaForHandOver;

        private OPC_ShippingSale _selectedDeliveryOrder;
        private ObservableCollection<OPC_ShippingSale> _deliveryOrders;

        private ObservableCollection<OPC_Sale> _salesOrdersForDelivery;
        private ObservableCollection<OPC_ShippingSale> _deliveryOrdersForExpress;
        private IList<Order> _orderOfExpressReceipt;
        private OPC_ShippingSale _selectedDeliveryOrderForExpress;
        private ShippingSaleCreateDto _shippingSaleCreateDto;

        private IList<Order> _orderOfHandOver;
        private OPC_ShippingSale _selectedDeliveryOrderForHandOver;
        private ObservableCollection<OPC_ShippingSale> _deliveryOrdersForHandOver;

        public StoreOutViewModel()
        {
            DeliveryOrders = new ObservableCollection<OPC_ShippingSale>();

            QueryCriteriaForDeliveryOrder = new QuerySalesOrderByComposition { Status = EnumSaleOrderStatus.ShipInStorage };
            QueryCriteriaForExpressReceipt = new QueryDeliveryOrderByComposition { Status = EnumSaleOrderStatus.PrintInvoice };
            QueryCriteriaForHandOver = new QueryDeliveryOrderByComposition { Status = EnumSaleOrderStatus.PrintExpress };

            QuerySalesOrderForDeliveryCommand = new AsyncDelegateCommand(OnSalesOrderForDeliveryQuery, MvvmUtility.OnException);
            QueryDeliveryOrderForExpressCommand = new AsyncDelegateCommand(OnDeliveryOrderForExpressQuery, MvvmUtility.OnException);
            QueryDeliveryOrderForHandOverCommand = new AsyncDelegateCommand(OnDeliveryOrderForHandOverQuery, MvvmUtility.OnException);

            CreateDeliveryOrderCommand = new AsyncDelegateCommand(OnDeliveryOrderCreate, MvvmUtility.OnException);
            CreateExpressReceiptCommand = new AsyncDelegateCommand(OnExpressReceiptCreate, MvvmUtility.OnException);

            RemarkOrderCommand = new DelegateCommand(SetOrderRemark);
            RemarkShippingCommand = new DelegateCommand(SetShippingRemark);
            
            CompleteHandOverCommand = new DelegateCommand(OnHandOverComplete);
            ShippingViaList = AppEx.Container.GetInstance<ICommonInfo>().GetShipViaList();
            ShippingSaleCreateDto = new ShippingSaleCreateDto();

            SelectAllSalesOrderForDeliveryCommand = new DelegateCommand<bool?>(OnAllSalesOrderForDeliverySelect);
            LoadOrderOfExpressReceiptCommand = new DelegateCommand<OPC_ShippingSale>(OnOrderOfExpressReceiptLoad);
            LoadOrderOfHandOverCommand = new DelegateCommand<OPC_ShippingSale>(OnOrderOfHandOverLoad);

            PrintExpressReceiptCommand = new DelegateCommand<OPC_ShippingSale>(OnExpressReceiptPrint);
            PreviewExpressReceiptCommand = new DelegateCommand<OPC_ShippingSale>(OnExpressReceiptPreview);
            PreviewDeliveryOrderCommand = new DelegateCommand<OPC_ShippingSale>(OnDeliveryOrderPreview);
            PrintDeliveryOrderCommand = new DelegateCommand<OPC_ShippingSale>(OnDeliveryOrderPrint);
        }

        #region Commands

        public ICommand SelectAllSalesOrderForDeliveryCommand { get; set; }
        
        public ICommand QuerySalesOrderForDeliveryCommand { get; set; }
        public ICommand QueryDeliveryOrderForExpressCommand { get; set; }
        public ICommand QueryDeliveryOrderForHandOverCommand { get; set; }

        public ICommand PrintDeliveryOrderCommand { get; set; }
        public ICommand PreviewDeliveryOrderCommand { get; set; }
        public ICommand PreviewExpressReceiptCommand { get; set; }
        public ICommand PrintExpressReceiptCommand { get; set; }
        public ICommand CompleteHandOverCommand { get; set; }
        public ICommand CreateExpressReceiptCommand { get; set; }
        public ICommand CreateDeliveryOrderCommand { get; set; }
        
        public ICommand RemarkOrderCommand { get; set; }
        public ICommand RemarkShippingCommand { get; set; }
        public ICommand GetListShipSaleCommand { get; set; }
        public ICommand LoadOrderOfExpressReceiptCommand { get; set; }
        public ICommand LoadOrderOfHandOverCommand { get; set; }

        #endregion

        #region Properties

        [Import]
        public IService<OPC_Sale> SalesOrderService { get; set; }

        [Import]
        public IDeliveryOrderService DeliveryOrderService { get; set; }

        [Import]
        public IService<Order> OrderService { get; set; }

        [Import]
        public IService<OPC_SaleDetail> SalesOrderDetailService { get; set; }

        public QuerySalesOrderByComposition QueryCriteriaForDeliveryOrder
        {
            get { return _queryCriteriaForDeliveryOrder; }
            set { SetProperty(ref _queryCriteriaForDeliveryOrder, value); }
        }

        public QueryDeliveryOrderByComposition QueryCriteriaForExpressReceipt
        {
            get { return _queryCriteriaForExpressReceipt; }
            set { SetProperty(ref _queryCriteriaForExpressReceipt, value); }
        }

        public QueryDeliveryOrderByComposition QueryCriteriaForHandOver
        {
            get { return _queryCriteriaForHandOver; }
            set { SetProperty(ref _queryCriteriaForHandOver, value); }
        }

        #region Delivery properties

        public ObservableCollection<OPC_ShippingSale> DeliveryOrders
        {
            get { return _deliveryOrders; }
            set { SetProperty(ref _deliveryOrders, value); }
        }

        public ObservableCollection<OPC_Sale> SalesOrdersForDelivery
        {
            get { return _salesOrdersForDelivery; }
            set { SetProperty(ref _salesOrdersForDelivery, value); }
        }

        public OPC_ShippingSale SelectedDeliveryOrder
        {
            get { return _selectedDeliveryOrder; }
            set { SetProperty(ref _selectedDeliveryOrder, value); }
        }

        public OPC_Sale SelectedSalesOrder { get; set; }

        #endregion

        #region Express properties

        public ObservableCollection<OPC_ShippingSale> DeliveryOrdersForExpress
        {
            get { return _deliveryOrdersForExpress; }
            set { SetProperty(ref _deliveryOrdersForExpress, value); }
        }

        public OPC_ShippingSale SelectedDeliveryOrderForExpress
        {
            get { return _selectedDeliveryOrderForExpress; }
            set { SetProperty(ref _selectedDeliveryOrderForExpress, value); }
        }

        public IList<Order> OrderOfExpressReceipt
        {
            get { return _orderOfExpressReceipt; }
            set { SetProperty(ref _orderOfExpressReceipt, value); }
        }

        public List<ShipVia> ShippingViaList { get; set; }

        public ShippingSaleCreateDto ShippingSaleCreateDto
        {
            get { return _shippingSaleCreateDto; }
            set { SetProperty(ref _shippingSaleCreateDto, value); }
        }

        #endregion

        #region HandOver properties

        public OPC_ShippingSale SelectedDeliveryOrderForHandOver
        {
            get { return _selectedDeliveryOrderForHandOver; }
            set { SetProperty(ref _selectedDeliveryOrderForHandOver, value); }
        }

        public ObservableCollection<OPC_ShippingSale> DeliveryOrdersForHandOver
        {
            get { return _deliveryOrdersForHandOver; }
            set { SetProperty(ref _deliveryOrdersForHandOver, value); }
        }

        public IList<Order> OrderOfHandOver
        {
            get { return _orderOfHandOver; }
            set { SetProperty(ref _orderOfHandOver, value); }
        }

        #endregion

        public int IsTabIndex { get; set; }

        #endregion

        #region Command Handlers

        /// <summary>
        /// 全选所有销售单
        /// </summary>
        /// <param name="isSelected"></param>
        private void OnAllSalesOrderForDeliverySelect(bool? isSelected)
        {
            if (SalesOrdersForDelivery == null || !SalesOrdersForDelivery.Any()) return;

            SalesOrdersForDelivery.ForEach(salesOrder => salesOrder.IsSelected = isSelected.Value);
        }

        /// <summary>
        /// 查询需要生成发货单的销售单
        /// </summary>
        private void OnSalesOrderForDeliveryQuery()
        {
            var salesOrders = SalesOrderService.QueryAll(_queryCriteriaForDeliveryOrder);
            SalesOrdersForDelivery = salesOrders.ToObservableCollection();
            if (salesOrders.Count == 0)
            {
                MessageBox.Show("没有符合条件的销售单", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void OnOrderOfExpressReceiptLoad(OPC_ShippingSale deliveryOrder)
        {
            OrderOfExpressReceipt = deliveryOrder == null ? null : new List<Order> { deliveryOrder.SalesOrders.First().Order };
        }

        private void OnOrderOfHandOverLoad(OPC_ShippingSale deliveryOrder)
        {
            OrderOfHandOver = deliveryOrder == null ? null : new List<Order> { deliveryOrder.SalesOrders.First().Order };
        }

        /// <summary>
        /// 查询需要生成快递单的发货单
        /// </summary>
        private void OnDeliveryOrderForExpressQuery()
        {
            var deliveryOrders = DeliveryOrderService.QueryAll(_queryCriteriaForExpressReceipt);
            DeliveryOrdersForExpress = deliveryOrders.ToObservableCollection();
            if (deliveryOrders.Count == 0)
            {
                MessageBox.Show("没有符合条件的发货单", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
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
            if (!selectedSalesOrdersForDelivery.Any())
            {
                MessageBox.Show("请选择至少一个销售单", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
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
            });

            SalesOrdersForDelivery.SafelyRemove(salesOrder => salesOrder.IsSelected);

            DeliveryOrders.SafelyInsert(0, deliveryOrder);
        }

        /// <summary>
        /// 生成快递单
        /// </summary>
        private void OnExpressReceiptCreate()
        {
            if (SelectedDeliveryOrderForExpress == null)
            {
                MessageBox.Show("请选择一张发货单", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ShippingSaleCreateDto.ValidateProperties();
            if (ShippingSaleCreateDto.HasErrors) return;

            DeliveryOrderService.Update<ShippingSaleCreateDto>(SelectedDeliveryOrderForExpress, ShippingSaleCreateDto);

            var selectedDeliveryOrderID = SelectedDeliveryOrderForExpress.Id;
            DeliveryOrdersForExpress.SafelyRemove(deliveryOrder => deliveryOrder.Id == selectedDeliveryOrderID);

            ShippingSaleCreateDto.ShippingCode = null;
        }

        /// <summary>
        /// 完成发货交接
        /// </summary>
        private void OnHandOverComplete()
        {
            if (SelectedDeliveryOrderForHandOver == null)
            {
                MessageBox.Show("请选择一张快递单", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DeliveryOrderService.CompleteHandOver(SelectedDeliveryOrderForHandOver);
            var selectedDeliveryOrderID = SelectedDeliveryOrderForHandOver.Id;
            DeliveryOrdersForHandOver.SafelyRemove(deliveryOrder => deliveryOrder.Id == selectedDeliveryOrderID);
        }

        private void GetListShipSaleBySale(string saleOrderNo)
        {
            var deliveryOrder = AppEx.Container.GetInstance<ITransService>().GetListShipSaleBySale(saleOrderNo);
            DeliveryOrders = new ObservableCollection<OPC_ShippingSale>(deliveryOrder);
        }

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

        private void SetOrderRemark()
        {
            //被选择的对象
            string id = SelectedSalesOrder.SaleOrderNo;
            var remarkWin = AppEx.Container.GetInstance<IRemark>();
            remarkWin.ShowRemarkWin(id, EnumSetRemarkType.SetOrderRemark); //3填写的是订单
        }

        /// <summary>
        /// 查询快递单
        /// </summary>
        private void OnDeliveryOrderForHandOverQuery()
        {
            var deliveryOrders = DeliveryOrderService.QueryAll(_queryCriteriaForHandOver);
            DeliveryOrdersForHandOver = deliveryOrders.ToObservableCollection();
            if (deliveryOrders.Count == 0)
            {
                MessageBox.Show("没有符合条件的快递单", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        #region 打印发货单

        private void OnDeliveryOrderPrint(OPC_ShippingSale deliveryOrder)
        {
            PrintDeliveryOrder(deliveryOrder, false);
        }

        private void OnDeliveryOrderPreview(OPC_ShippingSale deliveryOrder)
        {
            PrintDeliveryOrder(deliveryOrder, true);
        }

        private void PrintDeliveryOrder(OPC_ShippingSale deliveryOrder, bool preview)
        {
            const string ReportName = "Print//ReportFHD.rdlc";

            if (deliveryOrder == null)
            {
                MessageBox.Show("请选择一张发货单", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            IPrint reporter = new PrintWin();
            var order = deliveryOrder.SalesOrders.First().Order;
            var salesOrders = deliveryOrder.SalesOrders;
            var salesOrderDetails = SalesOrderDetailService.Query(new QuerySalesOrderDetailBySalesOrderNo { SalesOrderNo = deliveryOrder.SaleOrderNo });

            reporter.PrintDeliveryOrder(ReportName, order, salesOrders, salesOrderDetails.Data, !preview);

            if (!preview) DeliveryOrderService.Print(deliveryOrder, ReceiptType.DeliveryOrder);
        }

        #endregion

        #region 打印快递单

        /// <summary>
        /// 打印预览快递单
        /// </summary>
        private void OnExpressReceiptPreview(OPC_ShippingSale deliveryOrder)
        {
            PrintExpressReceipt(deliveryOrder, true);
        }

        /// <summary>
        /// 打印快递单
        /// </summary>
        private void OnExpressReceiptPrint(OPC_ShippingSale deliveryOrder)
        {
            PrintExpressReceipt(deliveryOrder, false);
        }

        /// <summary>
        /// 打印快递单
        /// </summary>
        /// <param name="deliveryOrder"></param>
        /// <param name="preview"></param>
        private void PrintExpressReceipt(OPC_ShippingSale deliveryOrder, bool preview)
        {
            const string ReportName = "Print//ReportForSF.rdlc";

            if (deliveryOrder == null)
            {
                MessageBox.Show("请选择一张快递单", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            IPrint reporter = new PrintWin();
            var order = deliveryOrder.SalesOrders.First().Order;
            var expressReceiptPrintModel = new PrintExpressModel()
            {
                CustomerAddress = order.CustomerAddress,
                CustomerName = order.CustomerName,
                CustomerPhone = order.CustomerPhone,
                ExpressFee = deliveryOrder.ExpressFee.ToString("f2")
            };

            reporter.PrintExpress(ReportName, expressReceiptPrintModel, !preview);

            if (!preview) DeliveryOrderService.Print(deliveryOrder, ReceiptType.ExpressReceipt);
        }

        #endregion

        #endregion
    }
}