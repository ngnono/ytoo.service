using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.DataService.Interface.Trans;
using OPCApp.DataService.IService;
using OPCApp.Domain.Customer;
using OPCAPP.Domain.Dto;
using OPCAPP.Domain.Enums;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;

namespace OPCApp.Customer.ViewModels
{
    [Export("CustomerSelfNetRuturnGoods", typeof(CustomerSelfNetRuturnGoods))]
    public class CustomerSelfNetRuturnGoods : BindableBase
    {
        private List<Order> _orderList;
        private List<OrderItem> _orderitemList;

        public CustomerSelfNetRuturnGoods()
        {
            CommandCustomerReturnGoodsSave = new DelegateCommand(CustomerReturnGoodsSave);
            CommandReturnGoodsSearch = new DelegateCommand(ReturnGoodsSearch);
            CommandGetDown = new DelegateCommand(GetOrderDetailByOrderNo);
            CommandSetOrderRemark = new DelegateCommand(SetOrderRemark);
            ClearOrInitData();
            InitCombo();
        }
        public IList<KeyValue> BrandList { get; set; }


        public IList<KeyValue> PaymentTypeList { get; set; }
        public void InitCombo()
        {
            // OderStatusList=new 
            BrandList = AppEx.Container.GetInstance<ICommonInfo>().GetBrandList();
           
            PaymentTypeList = AppEx.Container.GetInstance<ICommonInfo>().GetPayMethod();
        }
        public void SetOrderRemark()
        {
            //被选择的对象
            string id = Order.OrderNo;
            var remarkWin = AppEx.Container.GetInstance<IRemark>();
            remarkWin.ShowRemarkWin(id, EnumSetRemarkType.SetOrderRemark); //3填写的是订单
        }
        public DelegateCommand CommandSetOrderRemark { get; set; }
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
        private List<int> _returnList;

        public List<int> ReturnCountList
        {
            get { return _returnList; }
            set { SetProperty(ref _returnList, value); }
        }

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
            var selectOrder = OrderItemList.Where(e => e.IsSelected).ToList();
            if (selectOrder.Count==0)
            {
                MessageBox.Show("请选择销售单明细", "提示");
                return;
            }
            RmaPost.ReturnProducts =
                selectOrder.Select(e => new KeyValuePair<int, int>(e.StockId, e.NeedReturnCount)).ToList();
            bool bFlag = AppEx.Container.GetInstance<ICustomerReturnGoods>().CustomerReturnGoodsSave(RmaPost);
            MessageBox.Show(bFlag ? "客服退货成功" : "客服退货失败", "提示");
        }
    }
}