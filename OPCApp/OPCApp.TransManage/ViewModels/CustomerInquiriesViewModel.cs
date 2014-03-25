using System.ComponentModel.Composition;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.Infrastructure;
using Intime.OPC.Domain.Models;
using System.Collections.Generic;
using OPCApp.DataService.Interface.Trans;


namespace OPCApp.TransManage.ViewModels
{
    [Export("CustomerInquiriesViewModel", typeof(CustomerInquiriesViewModel))]
    public class CustomerInquiriesViewModel : BindableBase
    {

        public void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                TabControl tabControl = sender as TabControl;
                int i = tabControl.SelectedIndex;
                switch (i)
                {
                    case 1:
                        GetOrder();
                        break;
                    case 2:

                        break;
                    default:

                        break; ;
                }

            }

        }

        public DelegateCommand CommandGetOrder { get; set; }
        public DelegateCommand CommandGetSaleByOrderId { get; set; }
        public DelegateCommand CommandGetSaleDetailBySaleId { get; set; }

        public DelegateCommand CommandGetShipping { get; set; }
        public DelegateCommand CommandGetOrderByShippingId { get; set; }
        public DelegateCommand CommandGetSaleByOrderNoShipping { get; set; }

        public CustomerInquiriesViewModel()
        {
            //Tab 订单查询
            this.CommandGetOrder = new DelegateCommand(this.GetOrder);
            this.CommandGetSaleByOrderId = new DelegateCommand(this.GetSaleByOrderId);
            this.CommandGetSaleDetailBySaleId = new DelegateCommand(this.GetSaleDetailBySaleId);

            //Tab 发货信息
            this.CommandGetShipping = new DelegateCommand(this.GetShipping);
            this.CommandGetOrderByShippingId = new DelegateCommand(this.GetOrderByShippingId);
            this.CommandGetSaleByOrderNoShipping = new DelegateCommand(this.GetSaleByOrderNoShipping);
            orderGet = new OrderGet();
        }
        #region Tab1页签
       

        //Tab1选中的Order中的数据集
        private Order selectOrder;
        public Order SelectOrder
        {

            get { return this.selectOrder; }
            set { SetProperty(ref this.selectOrder, value); }
        }
        //Tab1选中的Sale中的数据集
        private OPC_Sale selectSale;
        public OPC_Sale SelectSale
        {

            get { return this.selectSale; }
            set { SetProperty(ref this.selectSale, value); }
        }

        //Tab1中Grid数据集1
        private IEnumerable<Order> orderList;
        public IEnumerable<Order> OrderList
        {

            get { return this.orderList; }
            set { SetProperty(ref this.orderList, value); }
        }
        //Tab1中Grid数据集2
        private IEnumerable<OPC_Sale> saleList;
        public IEnumerable<OPC_Sale> SaleList
        {

            get { return this.saleList; }
            set { SetProperty(ref this.saleList, value); }
        }

        //Tab1中Grid数据集3
        private IEnumerable<OPC_SaleDetail> saleDetailList;
        public IEnumerable<OPC_SaleDetail> SaleDetailList
        {

            get { return this.saleDetailList; }
            set { SetProperty(ref this.saleDetailList, value); }
        }

        //界面查询条件
        private OrderGet orderGet;
        public OrderGet OrderGet
        {
            get { return this.orderGet; }
            set { SetProperty(ref this.orderGet, value); }
        }

        



        public void GetOrder()
        {
            var orderfilter = string.Format("orderNo={0}&orderSource={1}&startCreateDate={2}&endCreateDate={3}&storeId={4}&BrandId={5}&status={6}&paymentType={7}&outGoodsType={8}&shippingContactPhone={9}&expressDeliveryCode={10}&expressDeliveryCompany={11}", OrderGet.OrderNo, OrderGet.OrderSource, OrderGet.StartCreateDate, OrderGet.EndCreateDate, OrderGet.StoreId, OrderGet.BrandId, OrderGet.Status, OrderGet.PaymentType, OrderGet.OutGoodsType, OrderGet.ShippingContactPhone, OrderGet.ExpressDeliveryCode, OrderGet.ExpressDeliveryCompany);

            OrderList = AppEx.Container.GetInstance<ICustomerInquiriesService>().GetOrder(orderfilter).Result;

        }
        public void GetSaleByOrderId()
        {
            if (string.IsNullOrEmpty(SelectOrder.Id.ToString()))
            {
                return;
            }
            string orderId = SelectOrder.Id.ToString();
            //这个工作状态
            SaleList = AppEx.Container.GetInstance<ICustomerInquiriesService>().GetSaleByOrderNo(orderId).Result;

        }

        public void GetSaleDetailBySaleId()
        {
            if (string.IsNullOrEmpty(SelectSale.Id.ToString()))
            {
                return;
            }
            string saleOrderNo = SelectSale.SaleOrderNo.ToString();
            //这个工作状态
            SaleDetailList = AppEx.Container.GetInstance<ITransService>().SelectSaleDetail(saleOrderNo).Result;
        }
        #endregion

        #region Tab2页签
        

        //Tab 发货查询 选中的Shipping中的数据集
        private OPC_ShippingSale selectShipping;
        public OPC_ShippingSale SelectShipping
        {

            get { return this.selectShipping; }
            set { SetProperty(ref this.selectShipping, value); }
        }

        //Tab 发货查询  选中的Order中的数据集
        private Order selectOrderShipping;
        public Order SelectOrderShipping
        {

            get { return this.selectOrderShipping; }
            set { SetProperty(ref this.selectOrderShipping, value); }
        }


        //Tab 发货查询 中Grid数据集shipping
        private IEnumerable<OPC_ShippingSale> shippingList;
        public IEnumerable<OPC_ShippingSale> ShippingList
        {

            get { return this.shippingList; }
            set { SetProperty(ref this.shippingList, value); }
        }
        //Tab  发货查询 中Grid数据集order
        private IEnumerable<Order> orderListShipping;
        public IEnumerable<Order> OrderListShipping
        {

            get { return this.orderListShipping; }
            set { SetProperty(ref this.orderListShipping, value); }
        }

        //Tab  发货查询 中Grid数据集sale
        private IEnumerable<OPC_Sale> saleListShipping;
        public IEnumerable<OPC_Sale> SaleListShipping
        {

            get { return this.saleListShipping; }
            set { SetProperty(ref this.saleListShipping, value); }
        }

        //Tab  发货查询  界面查询条件
        private ShippingGet shippingGet;
        public ShippingGet ShippingGet
        {
            get { return this.shippingGet; }
            set { SetProperty(ref this.shippingGet, value); }
        }




        public void GetShipping()
        {
            var shippingfilter = string.Format("OrderNo={0}&ExpressNo={1}&StartGoodsOutDate={2}&EndGoodsOutDate={3}&OutGoodsCode={4}&SectionId={5}&ShippingStatus={6}&CustomerPhone={7}&BrandId={8}", ShippingGet.OrderNo, ShippingGet.ExpressNo, ShippingGet.StartGoodsOutDate.ToShortDateString(), ShippingGet.EndGoodsOutDate.ToShortDateString(), ShippingGet.OutGoodsCode, ShippingGet.SectionId, ShippingGet.ShippingStatus, ShippingGet.CustomerPhone, ShippingGet.BrandId);

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
            OrderListShipping = AppEx.Container.GetInstance<ICustomerInquiriesService>().GetOrderByShippingId(shippingId).Result;

        }

        public void GetSaleByOrderNoShipping()
        {
            if (string.IsNullOrEmpty(SelectOrderShipping.Id.ToString()))
            {
                return;
            }
            string OrderNo = SelectOrderShipping.OrderNo.ToString();
            //这个工作状态
            SaleListShipping = AppEx.Container.GetInstance<ICustomerInquiriesService>().GetSaleByOrderNo(OrderNo).Result;
        }
        #endregion

    }
}
