using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.DataService.Interface.Customer;
using OPCApp.DataService.Interface.Trans;
using OPCApp.DataService.IService;
using OPCAPP.Domain.Dto;
using OPCAPP.Domain.Enums;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;

namespace OPCApp.TransManage.ViewModels
{
    [Export("CustomerInquiriesViewModel", typeof (CustomerInquiriesViewModel))]
    public class CustomerInquiriesViewModel : BindableBase
    {
        // public List<object> OderStatusList { get; set; }

        public CustomerInquiriesViewModel()
        {
            //Tab 订单查询         CommandGetSaleByOrderId
            CommandGetOrder = new DelegateCommand(GetOrder);
            CommandGetSaleByOrderId = new DelegateCommand(GetSaleByOrderId);
            CommandGetSaleDetailBySaleId = new DelegateCommand(GetSaleDetailBySaleId);

            //Tab 发货信息
            CommandGetShipping = new DelegateCommand(GetShipping);
            CommandGetOrderByShippingId = new DelegateCommand(GetOrderByShippingId);
            CommandGetSaleByOrderNoShipping = new DelegateCommand(GetSaleByOrderNoShipping);
            CommandSetRemark = new DelegateCommand(SetRemarkOrder);
            CommandSetShipSaleRemark = new DelegateCommand(ShipSaleRemark);
            InitCombo();
            _orderGet = new OrderGet();
            ShippingGet = new ShippingGet();
        }

        public void SetRemarkOrder()
        {
            //被选择的对象
            string id = SelectOrder.OrderNo;
            var remarkWin = AppEx.Container.GetInstance<IRemark>();
            remarkWin.ShowRemarkWin(id, EnumSetRemarkType.SetOrderRemark);
        }

        public DelegateCommand CommandSetShipSaleRemark { get; set; }

        public void ShipSaleRemark()
        {
            //被选择的对象
            string id = SelectShipping.ExpressCode;
            var remarkWin = AppEx.Container.GetInstance<IRemark>();
            remarkWin.ShowRemarkWin(id, EnumSetRemarkType.SetShipSaleRemark);
        }
        public DelegateCommand CommandSetRemark { get; set; }

        #region Tab1页签

        //Tab1选中的Order中的数据集
        private OrderGet _orderGet;
        private IEnumerable<OPCApp.Domain.Models.Order> _orderList;
        private IEnumerable<OPC_SaleDetail> _saleDetailList;
        private IEnumerable<OPC_Sale> _saleList;
        private OPCApp.Domain.Models.Order _selectOrder;

        //Tab1选中的Sale中的数据集
        private OPC_Sale _selectSale;

        public OPCApp.Domain.Models.Order SelectOrder
        {
            get { return _selectOrder; }
            set { SetProperty(ref _selectOrder, value); }
        }

        public OPC_Sale SelectSale
        {
            get { return _selectSale; }
            set { SetProperty(ref _selectSale, value); }
        }

        //Tab1中Grid数据集1

        public IEnumerable<OPCApp.Domain.Models.Order> OrderList
        {
            get { return _orderList; }
            set { SetProperty(ref _orderList, value); }
        }

        //Tab1中Grid数据集2

        public IEnumerable<OPC_Sale> SaleList
        {
            get { return _saleList; }
            set { SetProperty(ref _saleList, value); }
        }

        //Tab1中Grid数据集3

        public IEnumerable<OPC_SaleDetail> SaleDetailList
        {
            get { return _saleDetailList; }
            set { SetProperty(ref _saleDetailList, value); }
        }

        //界面查询条件

        public OrderGet OrderGet
        {
            get { return _orderGet; }
            set { SetProperty(ref _orderGet, value); }
        }


        public void GetOrder()
        {
            OrderList = new List<Order>();
            if (OrderGet.PaymentType=="-1")
                OrderGet.PaymentType="";
            if (OrderGet.OutGoodsType == "-1")
                OrderGet.OutGoodsType = "";
            

            string orderfilter =
                string.Format(
                    "orderNo={0}&orderSource={1}&startCreateDate={2}&endCreateDate={3}&storeId={4}&BrandId={5}&status={6}&paymentType={7}&outGoodsType={8}&shippingContactPhone={9}&expressDeliveryCode={10}&expressDeliveryCompany={11}",
                    OrderGet.OrderNo, OrderGet.OrderSource, OrderGet.StartCreateDate.ToShortDateString() , OrderGet.EndCreateDate.ToShortDateString() ,
                    string.IsNullOrEmpty(OrderGet.StoreId) ? "-1" : OrderGet.StoreId, string.IsNullOrEmpty(OrderGet.BrandId) ? "-1" : OrderGet.BrandId, OrderGet.Status ,OrderGet.PaymentType,OrderGet.OutGoodsType,
                    OrderGet.ShippingContactPhone, OrderGet.ExpressDeliveryCode, OrderGet.ExpressDeliveryCompany);
            
            var re=AppEx.Container.GetInstance<ICustomerInquiriesService>().GetOrder(orderfilter).Result;
            if (re != null)
            {
                OrderList = re.ToList();
            }
        }

        public void GetSaleByOrderId()
        {
            try
            {
                if (SelectOrder == null || string.IsNullOrEmpty(SelectOrder.Id.ToString()))
                {
                    return;
                }
                string orderNo = string.Format("orderID={0}&pageIndex={1}&pageSize={2}", SelectOrder.OrderNo,1,30);
                //这个工作状态
                SaleList = AppEx.Container.GetInstance<ICustomerInquiriesService>().GetSaleByOrderNo(orderNo).Result;
                if (SaleList != null && SaleList.Count() > 0)
                {
                    OPC_Sale sale = SaleList.ToList()[0];
                    SaleDetailList =
                        AppEx.Container.GetInstance<ITransService>().SelectSaleDetail(sale.SaleOrderNo).Result;
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void GetSaleDetailBySaleId()
        {
            if (SelectSale == null || string.IsNullOrEmpty(SelectSale.Id.ToString()))
            {
                return;
            }
            string saleOrderNo = SelectSale.SaleOrderNo;
            //这个工作状态
            SaleDetailList = AppEx.Container.GetInstance<ITransService>().SelectSaleDetail(saleOrderNo).Result;
        }

        #endregion

        #region Tab2页签

        //Tab 发货查询 选中的Shipping中的数据集
        private IEnumerable<Order> orderListShipping;
        private IEnumerable<OPC_Sale> saleListShipping;
        private Order selectOrderShipping;
        private OPC_ShippingSale selectShipping;
        private ShippingGet shippingGet;
        private IEnumerable<OPC_ShippingSale> shippingList;

        public OPC_ShippingSale SelectShipping
        {
            get { return selectShipping; }
            set { SetProperty(ref selectShipping, value); }
        }

        //Tab 发货查询  选中的Order中的数据集

        public Order SelectOrderShipping
        {
            get { return selectOrderShipping; }
            set { SetProperty(ref selectOrderShipping, value); }
        }


        //Tab 发货查询 中Grid数据集shipping

        public IEnumerable<OPC_ShippingSale> ShippingList
        {
            get { return shippingList; }
            set { SetProperty(ref shippingList, value); }
        }

        //Tab  发货查询 中Grid数据集order

        public IEnumerable<Order> OrderListShipping
        {
            get { return orderListShipping; }
            set { SetProperty(ref orderListShipping, value); }
        }

        //Tab  发货查询 中Grid数据集sale

        public IEnumerable<OPC_Sale> SaleListShipping
        {
            get { return saleListShipping; }
            set { SetProperty(ref saleListShipping, value); }
        }

        //Tab  发货查询  界面查询条件

        public ShippingGet ShippingGet
        {
            get { return shippingGet; }
            set { SetProperty(ref shippingGet, value); }
        }


        public void GetShipping()
        {
            string shippingfilter =
                string.Format(
                    "OrderNo={0}&ExpressNo={1}&StartGoodsOutDate={2}&EndGoodsOutDate={3}&OutGoodsCode={4}&StoreId={5}&ShippingStatus={6}&CustomerPhone={7}&BrandId={8}&pageIndex={9}&pageSize={10}",
                    ShippingGet.OrderNo, ShippingGet.ExpressNo, ShippingGet.StartGoodsOutDate.ToShortDateString(),
                    ShippingGet.EndGoodsOutDate.ToShortDateString(), ShippingGet.OutGoodsCode, string.IsNullOrEmpty(ShippingGet.StoreId) ? "-1" : ShippingGet.StoreId,
                    string.IsNullOrEmpty(ShippingGet.ShippingStatus) ? "-1" : ShippingGet.ShippingStatus, ShippingGet.CustomerPhone, string.IsNullOrEmpty(ShippingGet.BrandId) ? "-1" : ShippingGet.BrandId, 1, 100);
            try
            {
                ShippingList = AppEx.Container.GetInstance<ICustomerInquiriesService>().GetShipping(shippingfilter).Result;
            }
            catch (Exception ex)
            {
                
            }
        }

        public void GetOrderByShippingId()
        {
            try
            {
                if (SelectShipping == null )
                {
                    return;
                }
                //这个工作状态
                OrderListShipping =
                    AppEx.Container.GetInstance<ICustomerInquiriesService>().GetOrderByShippingId(string.Format("shippingNo={0}", SelectShipping.ExpressCode)).Result;
            }
            catch (Exception ex)
            {
            }
        }

        public void GetSaleByOrderNoShipping()
        {
            if (SelectOrderShipping == null)
            {
                return;
            }
            string filter = string.Format("orderID={0}&pageIndex={1}&pageSize={2}", SelectOrderShipping.OrderNo, 1, 300);
            SaleListShipping = AppEx.Container.GetInstance<ICustomerInquiriesService>().GetSaleByOrderNo(filter).Result;
        }

        #endregion

        public IList<KeyValue> StoreList { get; set; }

        public IList<KeyValue> BrandList { get; set; }

        public IList<KeyValue> OrderStatusList { get; set; }

        public IList<KeyValue> PaymentTypeList { get; set; }

        public IList<KeyValue> OutGoodsTypeList { get; set; }

        public List<ShipVia> ShipViaList { get; set; }

        public DelegateCommand CommandGetOrder { get; set; }
        public DelegateCommand CommandGetSaleByOrderId { get; set; }
        public DelegateCommand CommandGetSaleDetailBySaleId { get; set; }

        public DelegateCommand CommandGetShipping { get; set; }
        public DelegateCommand CommandGetOrderByShippingId { get; set; }
        public DelegateCommand CommandGetSaleByOrderNoShipping { get; set; }

        public void InitCombo()
        {
            // OderStatusList=new 
            StoreList = AppEx.Container.GetInstance<ICommonInfo>().GetStoreList();
            BrandList = AppEx.Container.GetInstance<ICommonInfo>().GetBrandList();
            OrderStatusList = AppEx.Container.GetInstance<ICommonInfo>().GetOrderStatus();
            PaymentTypeList = AppEx.Container.GetInstance<ICommonInfo>().GetPayMethod();
            OutGoodsTypeList = AppEx.Container.GetInstance<ICommonInfo>().GetOutGoodsMehtod();
            ShipViaList = AppEx.Container.GetInstance<ICommonInfo>().GetShipViaList();
        }

        /*方式不可取 等待修改*/

        public void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                var tabControl = sender as TabControl;
                int i = tabControl.SelectedIndex;
                switch (i)
                {
                    case 1:
                        GetShipping();
                        break;
                    case 2:

                        break;
                    default:
                        GetOrder();
                        break;
                        ;
                }
            }
            if (e.Source is DataGrid)
            {
                var dg = e.Source as DataGrid;
                switch (dg.Name)
                {
                    case "OrderDataGrid":
                        GetSaleByOrderId();
                        break;
                    case "SaleDataGrid":
                        GetSaleDetailBySaleId();
                        break;
                    case "ShipDataGrid":
                        GetOrderByShippingId();
                        break;
                    case "OrderDataGrid1":
                        GetSaleByOrderNoShipping();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}