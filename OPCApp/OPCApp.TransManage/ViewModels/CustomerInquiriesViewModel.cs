using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;
using Intime.OPC.Domain.Models;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.DataService.Interface.Trans;
using OPCApp.Infrastructure;

namespace OPCApp.TransManage.ViewModels
{
    [Export("CustomerInquiriesViewModel", typeof (CustomerInquiriesViewModel))]
    public class CustomerInquiriesViewModel : BindableBase
    {
        public CustomerInquiriesViewModel()
        {
            //Tab 订单查询
            CommandGetOrder = new DelegateCommand(GetOrder);
            CommandGetSaleByOrderId = new DelegateCommand(GetSaleByOrderId);
            CommandGetSaleDetailBySaleId = new DelegateCommand(GetSaleDetailBySaleId);

            //Tab 发货信息
            CommandGetShipping = new DelegateCommand(GetShipping);
            CommandGetOrderByShippingId = new DelegateCommand(GetOrderByShippingId);
            CommandGetSaleByOrderNoShipping = new DelegateCommand(GetSaleByOrderNoShipping);
            orderGet = new OrderGet();
        }

        #region Tab1页签

        //Tab1选中的Order中的数据集
        private OrderGet orderGet;
        private IEnumerable<Order> orderList;
        private IEnumerable<OPC_SaleDetail> saleDetailList;
        private IEnumerable<OPC_Sale> saleList;
        private Order selectOrder;

        //Tab1选中的Sale中的数据集
        private OPC_Sale selectSale;

        public Order SelectOrder
        {
            get { return selectOrder; }
            set { SetProperty(ref selectOrder, value); }
        }

        public OPC_Sale SelectSale
        {
            get { return selectSale; }
            set { SetProperty(ref selectSale, value); }
        }

        //Tab1中Grid数据集1

        public IEnumerable<Order> OrderList
        {
            get { return orderList; }
            set { SetProperty(ref orderList, value); }
        }

        //Tab1中Grid数据集2

        public IEnumerable<OPC_Sale> SaleList
        {
            get { return saleList; }
            set { SetProperty(ref saleList, value); }
        }

        //Tab1中Grid数据集3

        public IEnumerable<OPC_SaleDetail> SaleDetailList
        {
            get { return saleDetailList; }
            set { SetProperty(ref saleDetailList, value); }
        }

        //界面查询条件

        public OrderGet OrderGet
        {
            get { return orderGet; }
            set { SetProperty(ref orderGet, value); }
        }


        public void GetOrder()
        {
            string orderfilter =
                string.Format(
                    "orderNo={0}&orderSource={1}&startCreateDate={2}&endCreateDate={3}&storeId={4}&BrandId={5}&status={6}&paymentType={7}&outGoodsType={8}&shippingContactPhone={9}&expressDeliveryCode={10}&expressDeliveryCompany={11}",
                    OrderGet.OrderNo, OrderGet.OrderSource, OrderGet.StartCreateDate, OrderGet.EndCreateDate,
                    OrderGet.StoreId, OrderGet.BrandId, OrderGet.Status, OrderGet.PaymentType, OrderGet.OutGoodsType,
                    OrderGet.ShippingContactPhone, OrderGet.ExpressDeliveryCode, OrderGet.ExpressDeliveryCompany);

            OrderList = AppEx.Container.GetInstance<ICustomerInquiriesService>().GetOrder(orderfilter).Result;
        }

        public void GetSaleByOrderId()
        {
            if (string.IsNullOrEmpty(SelectOrder.Id.ToString()))
            {
                return;
            }
            string orderNo = string.Format("orderID={0}", SelectOrder.OrderNo);
            //这个工作状态
            SaleList = AppEx.Container.GetInstance<ICustomerInquiriesService>().GetSaleByOrderNo(orderNo).Result;
            if (SaleList != null)
            {
                var sale = SaleList.ToList()[0];
                SaleDetailList = AppEx.Container.GetInstance<ITransService>().SelectSaleDetail(sale.SaleOrderNo).Result;
            }
        }

        public void GetSaleDetailBySaleId()
        {
            if (string.IsNullOrEmpty(SelectSale.Id.ToString()))
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
                    "OrderNo={0}&ExpressNo={1}&StartGoodsOutDate={2}&EndGoodsOutDate={3}&OutGoodsCode={4}&SectionId={5}&ShippingStatus={6}&CustomerPhone={7}&BrandId={8}",
                    ShippingGet.OrderNo, ShippingGet.ExpressNo, ShippingGet.StartGoodsOutDate.ToShortDateString(),
                    ShippingGet.EndGoodsOutDate.ToShortDateString(), ShippingGet.OutGoodsCode, ShippingGet.SectionId,
                    ShippingGet.ShippingStatus, ShippingGet.CustomerPhone, ShippingGet.BrandId);

            ShippingList = AppEx.Container.GetInstance<ICustomerInquiriesService>().GetShipping(shippingfilter).Result;
        }

        public void GetOrderByShippingId()
        {
            if (string.IsNullOrEmpty(SelectShipping.Id.ToString()))
            {
                return;
            }
            string shippingId = SelectShipping.Id.ToString();
            //这个工作状态
            OrderListShipping =
                AppEx.Container.GetInstance<ICustomerInquiriesService>().GetOrderByShippingId(shippingId).Result;
        }

        public void GetSaleByOrderNoShipping()
        {
            if (string.IsNullOrEmpty(SelectOrderShipping.Id.ToString()))
            {
                return;
            }
            string OrderNo = SelectOrderShipping.OrderNo;
            //这个工作状态
            SaleListShipping = AppEx.Container.GetInstance<ICustomerInquiriesService>().GetSaleByOrderNo(OrderNo).Result;
        }

        #endregion

        public DelegateCommand CommandGetOrder { get; set; }
        public DelegateCommand CommandGetSaleByOrderId { get; set; }
        public DelegateCommand CommandGetSaleDetailBySaleId { get; set; }

        public DelegateCommand CommandGetShipping { get; set; }
        public DelegateCommand CommandGetOrderByShippingId { get; set; }
        public DelegateCommand CommandGetSaleByOrderNoShipping { get; set; }

        public void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                var tabControl = sender as TabControl;
                int i = tabControl.SelectedIndex;
                switch (i)
                {
                    case 1:
                        GetOrder();
                        break;
                    case 2:

                        break;
                    default:

                        break;
                        ;
                }
            }
        }
    }
}