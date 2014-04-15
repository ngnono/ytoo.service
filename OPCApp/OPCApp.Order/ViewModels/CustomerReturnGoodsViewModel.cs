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
using OPCApp.Domain.Dto;
using OPCApp.Domain.Enums;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;

namespace OPCApp.Customer.ViewModels
{
    [Export("CustomerReturnGoodsViewModel", typeof(CustomerReturnGoodsViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CustomerReturnGoodsViewModel : BindableBase
    {
        private OPC_SaleRMA _opcSaleRma;
        private OrderItem _orderItem;
        private List<OPC_SaleRMA> _orderList;
        private List<OrderItem> _orderitemList;
        private List<int> _returnList;
        private RMAPost rmaPost;

        public CustomerReturnGoodsViewModel()
        {
            CommandCustomerReturnGoodsSave = new DelegateCommand(CustomerReturnGoodsSave);
            CommandReturnGoodsSearch = new DelegateCommand(ReturnGoodsSearch);
            CommandGetDown = new DelegateCommand(GetOrderDetailByOrderNo);
            CommandSetOrderRemark = new DelegateCommand(SetOrderRemark);
            ReturnGoodsGet = new ReturnGoodsGet();
            ClearOrInitData();
            InitCombo();
            RmaPost = new RMAPost();
        }


        public IList<KeyValue> BrandList { get; set; }


        public IList<KeyValue> PaymentTypeList { get; set; }

        public DelegateCommand CommandComboSelect { get; set; }
        public DelegateCommand CommandSetOrderRemark { get; set; }
        public DelegateCommand CommandReturnGoodsSearch { get; set; }
        public DelegateCommand CommandCustomerReturnGoodsSave { get; set; }
        public DelegateCommand CommandGetDown { get; set; }

        public RMAPost RmaPost
        {
            get { return rmaPost; }
            set { SetProperty(ref rmaPost, value); }
        }

        public List<OPC_SaleRMA> SaleRmaList
        {
            get { return _orderList; }
            set { SetProperty(ref _orderList, value); }
        }

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

        public List<int> ReturnCountList
        {
            get { return _returnList; }
            set { SetProperty(ref _returnList, value); }
        }

        public OrderItem OrderItem
        {
            get { return _orderItem; }
            set { SetProperty(ref _orderItem, value); }
        }

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

        public void ClearOrInitData()
        {
            OrderItemList = new List<OrderItem>();
            SaleRmaList = new List<OPC_SaleRMA>();
        }

        /*点击销售单显示明细*/

        public virtual  void GetOrderDetailByOrderNo()
        {
            if (SaleRma != null)
            {
                OrderItemList.Clear();
                IList<OrderItem> list = AppEx.Container.GetInstance<ICustomerReturnGoods>()
                    .GetOrderDetailByOrderNo(SaleRma.OrderNo);
                OrderItemList = list.ToList();
            }
        }

        /*退货单查询*/

        public  virtual  void ReturnGoodsSearch()
        {
            SaleRmaList.Clear();
            IList<OPC_SaleRMA> list =
                AppEx.Container.GetInstance<ICustomerReturnGoods>().ReturnGoodsSearch(ReturnGoodsGet);
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
            List<OrderItem> selectOrder = OrderItemList.Where(e => e.IsSelected).ToList();
            if (SaleRma == null)
            {
                MessageBox.Show("请选择订单", "提示");
                return;
            }
            if (selectOrder.Count == 0)
            {
                MessageBox.Show("请选择销售单明细", "提示");
                return;
            }
            List<KeyValuePair<int, int>> list =
                selectOrder.Select(
                    e => new KeyValuePair<int, int>(e.Id, e.NeedReturnCount)).ToList<KeyValuePair<int, int>>();
            RmaPost.OrderNo = SaleRma.OrderNo;
            RmaPost.ReturnProducts = list;
            bool bFlag = AppEx.Container.GetInstance<ICustomerReturnGoods>().CustomerReturnGoodsSave(RmaPost);
            MessageBox.Show(bFlag ? "客服退货成功" : "客服退货失败", "提示");
            if (bFlag)
            {
                ClearOrInitData();
                RmaPost = new RMAPost();
                ReturnGoodsSearch();
            }
        }
    }
}