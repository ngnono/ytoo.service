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

        #region Tab1页签
        public DelegateCommand CommandGetOrder { get; set; }
        public DelegateCommand CommandGetSaleByOrderId { get; set; }
        public DelegateCommand CommandGetSaleDetailBySaleId { get; set; }

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

        public CustomerInquiriesViewModel()
        {
            //初始化命令属性
            CommandGetOrder = new DelegateCommand(GetOrder);

            CommandGetSaleByOrderId = new DelegateCommand(GetSaleByOrderId);
            CommandGetSaleDetailBySaleId = new DelegateCommand(GetSaleDetailBySaleId);
        }



        public void GetOrder()
        {
            var orderfilter = string.Format("orderNo={0}&orderSource={1}&startCreateDate={2}&endCreateDate={3}&storeId={4}&BrandId={5}&status={6}&paymentType={7}&outGoodsType={8}&shippingContactPhone={9}&expressDeliveryCode={10}&expressDeliveryCompany={11}", OrderGet.OrderNo, OrderGet.OrderSource, OrderGet.StartCreateDate.ToShortDateString(), OrderGet.EndCreateDate.ToShortDateString(), OrderGet.StoreId, OrderGet.BrandId, OrderGet.Status, OrderGet.PaymentType, OrderGet.OutGoodsType, OrderGet.ShippingContactPhone, OrderGet.ExpressDeliveryCode, OrderGet.ExpressDeliveryCompany);

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
            SaleList = AppEx.Container.GetInstance<ICustomerInquiriesService>().GetSaleByOrderId(orderId).Result;

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
        public DelegateCommand CommandGetShipping { get; set; }
        public DelegateCommand CommandGetOrderByShippingId { get; set; }
        public DelegateCommand CommandGetSaleByOrderIdShipping { get; set; }

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
        public IEnumerable<OPC_Sale> SaleDetailList
        {

            get { return this.saleListShipping; }
            set { SetProperty(ref this.saleListShipping, value); }
        }

        //Tab  发货查询  界面查询条件
        private OrderGet shippingGet;
        public OrderGet ShippingGet
        {
            get { return this.shippingGet; }
            set { SetProperty(ref this.shippingGet, value); }
        }

        public CustomerInquiriesViewModel()
        {
            //初始化命令属性
            CommandGetShipping = new DelegateCommand(GetShipping);

            CommandGetOrderByShippingId = new DelegateCommand(GetOrderByShippingId);
            CommandGetSaleByOrderIdShipping = new DelegateCommand(GetSaleByOrderIdShipping);
        }



        public void GetShipping()
        {
            var orderfilter = string.Format("orderNo={0}&orderSource={1}&startCreateDate={2}&endCreateDate={3}&storeId={4}&BrandId={5}&status={6}&paymentType={7}&outGoodsType={8}&shippingContactPhone={9}&expressDeliveryCode={10}&expressDeliveryCompany={11}", OrderGet.OrderNo, OrderGet.OrderSource, OrderGet.StartCreateDate.ToShortDateString(), OrderGet.EndCreateDate.ToShortDateString(), OrderGet.StoreId, OrderGet.BrandId, OrderGet.Status, OrderGet.PaymentType, OrderGet.OutGoodsType, OrderGet.ShippingContactPhone, OrderGet.ExpressDeliveryCode, OrderGet.ExpressDeliveryCompany);

            OrderList = AppEx.Container.GetInstance<ICustomerInquiriesService>().GetOrder(orderfilter).Result;

        }
        public void GetOrderByShippingId()
        {
            if (string.IsNullOrEmpty(SelectOrder.Id.ToString()))
            {
                return;
            }
            string orderId = SelectOrder.Id.ToString();
            //这个工作状态
            SaleList = AppEx.Container.GetInstance<ICustomerInquiriesService>().GetSaleByOrderId(orderId).Result;

        }

        public void GetSaleByOrderIdShipping()
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

    }
}
