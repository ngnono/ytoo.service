using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.DataService.Interface.Customer;
using OPCApp.DataService.Interface.Trans;
using OPCApp.DataService.IService;
using OPCApp.Domain.Dto;
using OPCApp.Domain.Enums;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;

namespace OPCApp.Customer.ViewModels
{

    [Export(typeof(CustomerStockoutRemindNotReplenishViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CustomerStockoutRemindNotReplenishViewModel : BindableBase
    {
        public CustomerStockoutRemindNotReplenishViewModel()
        {
            CommandGetOrder = new DelegateCommand(GetOrder);
            CommandGetSaleByOrderId = new DelegateCommand(GetSaleByOrderId);
            CommandGetSaleDetailBySaleId = new DelegateCommand(GetSaleDetailBySaleId);
            CommandCannotReplenish=new DelegateCommand(CannotReplenish);
            InitCombo();
            _orderGet = new OrderGet();
        }

       

        public DelegateCommand CommandSetRemark { get; set; }

        public IList<KeyValue> StoreList { get; set; }
        public IList<KeyValue> PaymentTypeList { get; set; }
        public IList<KeyValue> BrandList { get; set; }

        public IList<KeyValue> OrderStatusList { get; set; }


        public IList<KeyValue> OutGoodsTypeList { get; set; }

        public List<ShipVia> ShipViaList { get; set; }

        public DelegateCommand CommandGetOrder { get; set; }
        public DelegateCommand CommandGetSaleByOrderId { get; set; }
        public DelegateCommand CommandGetSaleDetailBySaleId { get; set; }

        public DelegateCommand CommandGetShipping { get; set; }
        public DelegateCommand CommandGetOrderByShippingId { get; set; }
        public DelegateCommand CommandGetSaleByOrderNoShipping { get; set; }
        public DelegateCommand CommandCannotReplenish { get; set; }

        #region Tab1页签

        //Tab1选中的Order中的数据集
        private OrderGet _orderGet;
        private List<Order> _orderList;
        private List<OPC_SaleDetail> _saleDetailList;
        private List<OPC_Sale> _saleList;
        private Order _selectOrder;

        //Tab1选中的Sale中的数据集
        private OPC_Sale _selectSale;

        public Order SelectOrder
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

        public List<Order> OrderList
        {
            get { return _orderList; }
            set { SetProperty(ref _orderList, value); }
        }

        //Tab1中Grid数据集2

        public List<OPC_Sale> SaleList
        {
            get { return _saleList; }
            set { SetProperty(ref _saleList, value); }
        }

        //Tab1中Grid数据集3

        public List<OPC_SaleDetail> SaleDetailList
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
        private void CannotReplenish()
        {
            if (SaleList == null && !SaleList.Any())
            {
                MessageBox.Show("请选择销售单", "提示");
                return;
            }
            var saleListSelected = SaleList.Where(e => e.IsSelected).ToList();
            var falg = AppEx.Container.GetInstance<ICustomerInquiriesService>().SetCannotReplenish(saleListSelected.Select(e=>e.SaleOrderNo).ToList());
            MessageBox.Show(falg?"设置取销售单成功":"设置取消销售单失败","提示");
            if (falg)
            {
                SaleList = new List<OPC_Sale>();
                SaleDetailList = new List<OPC_SaleDetail>();
                GetSaleByOrderId();
            }
        }

        public void GetOrder()
        {
            OrderList = new List<Order>();
            if (OrderGet.PaymentType == "-1")
                OrderGet.PaymentType = "";
            if (OrderGet.OutGoodsType == "-1")
                OrderGet.OutGoodsType = "";


            string orderfilter =
                string.Format(
                    "orderNo={0}&orderSource={1}&startCreateDate={2}&endCreateDate={3}&storeId={4}&BrandId={5}&status={6}&paymentType={7}&outGoodsType={8}&shippingContactPhone={9}&expressDeliveryCode={10}&expressDeliveryCompany={11}",
                    OrderGet.OrderNo, OrderGet.OrderSource, OrderGet.StartCreateDate.ToShortDateString(),
                    OrderGet.EndCreateDate.ToShortDateString(),
                    string.IsNullOrEmpty(OrderGet.StoreId) ? "-1" : OrderGet.StoreId,
                    string.IsNullOrEmpty(OrderGet.BrandId) ? "-1" : OrderGet.BrandId, OrderGet.Status,
                    OrderGet.PaymentType, OrderGet.OutGoodsType,
                    OrderGet.ShippingContactPhone, OrderGet.ExpressDeliveryCode, OrderGet.ExpressDeliveryCompany);

            var re = AppEx.Container.GetInstance<ICustomerInquiriesService>().GetOrderStockout(orderfilter).Result;
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
                string orderNo = string.Format("orderID={0}&pageIndex={1}&pageSize={2}", SelectOrder.OrderNo, 1, 30);
                //这个工作状态
                SaleList = AppEx.Container.GetInstance<ICustomerInquiriesService>().GetSaleByOrderNo(orderNo).Result.ToList();
                if (SaleList != null && SaleList.Any())
                {
                    OPC_Sale sale = SaleList.ToList()[0];
                    SaleDetailList =
                        AppEx.Container.GetInstance<ITransService>().SelectSaleDetail(sale.SaleOrderNo).Result.ToList();
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
            SaleDetailList = AppEx.Container.GetInstance<ITransService>().SelectSaleDetail(saleOrderNo).Result.ToList();
        }

        #endregion


        public void SetRemarkOrder()
        {
            //被选择的对象
            string id = SelectOrder.OrderNo;
            var remarkWin = AppEx.Container.GetInstance<IRemark>();
            remarkWin.ShowRemarkWin(id, EnumSetRemarkType.SetOrderRemark);
        }


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
    }
}

