using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.DataService.Interface.Trans;
using OPCApp.Domain.Customer;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;

namespace OPCApp.Customer.ViewModels
{
    [Export("CustomerRuturnGoodsViewModel", typeof (CustomerRuturnGoodsViewModel))]
    public class CustomerRuturnGoodsViewModel : BindableBase
    {
        private List<Order> _orderList;
        private List<OrderItem> _orderitemList;

        public CustomerRuturnGoodsViewModel()
        {
            CommandCustomerReturnGoodsSave = new DelegateCommand(CustomerReturnGoodsSave);
            CommandReturnGoodsSearch = new DelegateCommand(ReturnGoodsSearch);
            CommandGetDown = new DelegateCommand(GetOrderDetailByOrderNo);
            ClearOrInitData();
        }

        public DelegateCommand CommandReturnGoodsSearch { get; set; }
        public DelegateCommand CommandCustomerReturnGoodsSave { get; set; }
        public DelegateCommand CommandGetDown { get; set; }
        public RMAPost RmaPost { get; set; }

        public List<Order> OrderList
        {
            get { return _orderList; }
            set { SetProperty(ref _orderList, value); }
        }

        private Order _order;
        public Order Order
        {
            get { return _order; }
            set { SetProperty(ref _order, value); }
        }
        public List<OrderItem> OrderItemList
        {
            get { return _orderitemList; }
            set { SetProperty(ref _orderitemList, value); }
        }

        public ReturnGoodsGet ReturnGoodsGet { get; set; }

        private void ClearOrInitData()
        {
            OrderItemList = new List<OrderItem>();
            OrderList = new List<Order>();
        }
        /*点击销售单显示明细*/
        private void GetOrderDetailByOrderNo()
        {
            if (Order != null)
            {
                this.OrderList.Clear();
                IList<Order> list = AppEx.Container.GetInstance<ICustomerReturnGoods>()
                    .ReturnGoodsSearch(ReturnGoodsGet);
            }

        }
        /*退货单查询*/
        private void ReturnGoodsSearch()
        {
            OrderList.Clear();
            IList<Order> list = AppEx.Container.GetInstance<ICustomerReturnGoods>().ReturnGoodsSearch(ReturnGoodsGet);
            if (OrderList != null)
            {
                ClearOrInitData();
            }
            else
            {
                OrderList = list.ToList();
            }
        }
        /*客服退货*/
        private void CustomerReturnGoodsSave()
        {

            bool bFlag = AppEx.Container.GetInstance<ICustomerReturnGoods>().CustomerReturnGoodsSave(RmaPost);
            MessageBox.Show(bFlag ? "客服退货成功" : "客服退货失败", "提示");
        }
    }
}