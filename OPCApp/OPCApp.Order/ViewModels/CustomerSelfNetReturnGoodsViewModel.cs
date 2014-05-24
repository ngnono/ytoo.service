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
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;

namespace OPCApp.Customer.ViewModels
{
    [Export(typeof(CustomerSelfNetReturnGoodsViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CustomerSelfNetReturnGoodsViewModel : CustomerReturnGoodsViewModel
    {
        public CustomerSelfNetReturnGoodsViewModel()
        {
            CommandCustomerReturnGoodsPass = new DelegateCommand(SetCustomerReturnGoodsPass);
            CommandCustomerReturnGoodsSeftReject = new DelegateCommand(SetCustomerReturnGoodsSeftReject);
        }

        private void SetCustomerReturnGoodsPass()
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
            //原因必填
            if (string.Empty == RmaPost.Remark)
            {
                MessageBox.Show("退货备注不能为空", "提示");
                return;
            }

            if (RmaPost.RealRMASumMoney<0)
            {
                MessageBox.Show("赔偿金额不能小于零", "提示");
                return;

            }

            List<KeyValuePair<int, int>> list =
                selectOrder.Select(
                    e => new KeyValuePair<int, int>(e.Id, e.NeedReturnCount)).ToList<KeyValuePair<int, int>>();
            RmaPost.OrderNo = SaleRma.OrderNo;
            RmaPost.ReturnProducts = list;
            bool bFlag = AppEx.Container.GetInstance<ICustomerReturnGoods>().CustomerReturnGoodsSelfPass(RmaPost);
            MessageBox.Show(bFlag ? "退货审核成功" : "退货审核失败", "提示");
            if (bFlag)
            {
                ClearOrInitData();
                RmaPost = new RMAPost();
                ReturnGoodsSearch();
            }
        }

        public DelegateCommand CommandCustomerReturnGoodsSeftReject { get; set; }

        public DelegateCommand CommandCustomerReturnGoodsPass { get; set; }

        private void SetCustomerReturnGoodsSeftReject()
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
            bool bFlag = AppEx.Container.GetInstance<ICustomerReturnGoods>().CustomerReturnGoodsSelfReject(RmaPost);
            MessageBox.Show(bFlag ? "拒绝退货申请成功" : "拒绝退货申请失败", "提示");
            if (bFlag)
            {
                ClearOrInitData();
                RmaPost = new RMAPost();
                ReturnGoodsSearch();
            }
        }

        public override void GetOrderDetailByOrderNo()
        {
            if (SaleRma != null)
            {
                OrderItemList.Clear();
                IList<OrderItem> list = AppEx.Container.GetInstance<ICustomerReturnGoods>()
                    .GetOrderDetailByOrderNoWithSelf(SaleRma.OrderNo);
                OrderItemList = list.ToList();
            }
        }

        public override void ReturnGoodsSearch()
        {
            SaleRmaList.Clear();
            IList<OPC_SaleRMA> list =
                AppEx.Container.GetInstance<ICustomerReturnGoods>().ReturnGoodsSearchForSelf(ReturnGoodsGet);
            if (list == null)
            {
                ClearOrInitData();
            }
            else
            {
                SaleRmaList = list.ToList();
            }
        }
    }
}

