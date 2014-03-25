using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using OPCAPP.Domain.Enums;
using OPCApp.Infrastructure;
using OPCApp.Domain;
using Intime.OPC.Domain.Models;
using System.Collections.Generic;
using OPCApp.DataService.Interface.Trans;


namespace OPCApp.TransManage.ViewModels
{
     [Export("CustomerInquiriesViewModel", typeof(CustomerInquiriesViewModel))]
   public class CustomerInquiriesViewModel:BindableBase
    {//1
        public DelegateCommand CommandGetOrder { get; set; }
        public DelegateCommand CommandGetSaleByOrderId { get; set; }
        public DelegateCommand CommandGetSaleDetailBySaleId { get; set; }

        //选中的Order中的数据集
        private Order selectOrder;
        public Order SelectOrder
        {

            get { return this.selectOrder; }
            set { SetProperty(ref this.selectOrder, value); }
        }
        //选中的Sale中的数据集
        private OPC_Sale selectSale;
        public OPC_Sale SelectSale
        {

            get { return this.selectSale; }
            set { SetProperty(ref this.selectSale, value); }
        }

        //Grid数据集1
        private IEnumerable<Order> orderList;
        public IEnumerable<Order> OrderList
        {

            get { return this.orderList; }
            set { SetProperty(ref this.orderList, value); }
        }
        //Grid数据集2
        private IEnumerable<OPC_Sale> saleList;
        public IEnumerable<OPC_Sale> SaleList
        {

            get { return this.saleList; }
            set { SetProperty(ref this.saleList, value); }
        }

        //Grid数据集3
        private IEnumerable<OPC_SaleDetail> saleDetailList;
        public IEnumerable<OPC_SaleDetail> SaleDetailList
        {

            get { return this.saleDetailList; }
            set { SetProperty(ref this.saleDetailList, value); }
        }

        //界面查询条件
        private OrderGet order4Get;
        public OrderGet Order4Get
        {
            get { return this.order4Get; }
            set { SetProperty(ref this.order4Get, value); }
        }

         public CustomerInquiriesViewModel()
        {
            //初始化命令属性
            CommandGetOrder = new DelegateCommand(GetOrder);

            CommandGetSaleByOrderId = new DelegateCommand(GetSaleByOrderId);
            CommandGetSaleDetailBySaleId = new DelegateCommand(GetSaleDetailBySaleId);
        }

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

        public void GetOrder()
        {
            var orderfilter = string.Format("orderNo={0}&orderSource={1}&startCreateDate={2}&endCreateDate={3}&storeId={4}&BrandId={5}&status={6}&paymentType={7}&outGoodsType={8}&shippingContactPhone={9}&expressDeliveryCode={10}&expressDeliveryCompany={11}", Order4Get.OrderNo, Order4Get.OrderSource, Order4Get.StartCreateDate.ToShortDateString(), Order4Get.EndCreateDate.ToShortDateString(), Order4Get.StoreId, Order4Get.BrandId, Order4Get.Status, Order4Get.PaymentType, Order4Get.OutGoodsType, Order4Get.ShippingContactPhone, Order4Get.ExpressDeliveryCode, Order4Get.ExpressDeliveryCompany);

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
    }
}
