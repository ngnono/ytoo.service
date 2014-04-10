using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.DataService.Interface.Customer;
using OPCApp.DataService.Interface.Trans;
using OPCApp.DataService.IService;
using OPCApp.Domain.Customer;
using OPCAPP.Domain.Dto;
using OPCAPP.Domain.Enums;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;

namespace OPCApp.Customer.ViewModels
{
    [Export("CustomerReturnGoodsViewModel", typeof(CustomerReturnGoodsViewModel))]
    public class CustomerReturnGoodsViewModel : BindableBase
    {
        private List<OPC_SaleRMA> _orderList;
        private List<OrderItem> _orderitemList;

        public CustomerReturnGoodsViewModel()
        {
            CommandCustomerReturnGoodsSave = new DelegateCommand(CustomerReturnGoodsSave);
            CommandReturnGoodsSearch = new DelegateCommand(ReturnGoodsSearch);
            CommandGetDown = new DelegateCommand(GetOrderDetailByOrderNo);
            CommandSetOrderRemark = new DelegateCommand(SetOrderRemark);
            CommandComboSelect = new DelegateCommand(ComboSelect);
            ReturnGoodsGet = new ReturnGoodsGet();
            ClearOrInitData();
            InitCombo();
            RmaPost = new RMAPost();
        }

        public void ComboSelect()
        {
            ReturnCountList = new List<int>() {1,2,4};
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
            string id = SaleRma.OrderNo;
            var remarkWin = AppEx.Container.GetInstance<IRemark>();
            remarkWin.ShowRemarkWin(id, EnumSetRemarkType.SetSaleRMARemark); //4填写的是退货单
        }

        public DelegateCommand CommandComboSelect { get; set; }
        public DelegateCommand CommandSetOrderRemark { get; set; }
        public DelegateCommand CommandReturnGoodsSearch { get; set; }
        public DelegateCommand CommandCustomerReturnGoodsSave { get; set; }
        public DelegateCommand CommandGetDown { get; set; }
        public RMAPost RmaPost { get; set; }

        public List<OPC_SaleRMA> SaleRmaList
        {
            get { return _orderList; }
            set { SetProperty(ref _orderList, value); }
        }

        private OPC_SaleRMA _opcSaleRma;
        public OPC_SaleRMA SaleRma
        {
            get { return _opcSaleRma; }
            set { SetProperty(ref _opcSaleRma, value); }
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

        private OrderItem _orderItem;
        public OrderItem OrderItem
        {
            get { return _orderItem; }
            set { SetProperty(ref _orderItem, value); }
        }
        private void ClearOrInitData()
        {
            OrderItemList = new List<OrderItem>();
            SaleRmaList = new List<OPC_SaleRMA>();
        }
        /*点击销售单显示明细*/
        private void GetOrderDetailByOrderNo()
        {
            if (SaleRma != null)
            {
                this.OrderItemList.Clear();
                IList<OrderItem> list = AppEx.Container.GetInstance<ICustomerReturnGoods>()
                    .GetOrderDetailByOrderNo(SaleRma.OrderNo);
                OrderItemList = list.ToList();
            }

        }
        /*退货单查询*/
        private void ReturnGoodsSearch()
        {
            SaleRmaList.Clear();
            var list = AppEx.Container.GetInstance<ICustomerReturnGoods>().ReturnGoodsSearch(ReturnGoodsGet);
            if (list == null)
            {
                ClearOrInitData();
            }
            else
            {
                SaleRmaList = list.ToList();
            }
        }
        /*客服退货*/
        private void CustomerReturnGoodsSave()
        {
            var selectOrder = OrderItemList.Where(e => e.IsSelected).ToList();
            if (SaleRma==null)
            {
                MessageBox.Show("请选择订单", "提示");
                return;
            }
            if (selectOrder.Count == 0)
            {
                MessageBox.Show("请选择销售单明细", "提示");
                return;
            }
            var list =
                selectOrder.Select<OrderItem, KeyValuePair<int, int>>(
                    e => new KeyValuePair<int, int>(e.Id, e.NeedReturnCount)).ToList<KeyValuePair<int, int>>();
            RmaPost.OrderNo = SaleRma.OrderNo;
            RmaPost.ReturnProducts = list;
            bool bFlag = AppEx.Container.GetInstance<ICustomerReturnGoods>().CustomerReturnGoodsSave(RmaPost);
            MessageBox.Show(bFlag ? "客服退货成功" : "客服退货失败", "提示");
        }

    }
}