using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.DataService.Interface.Trans;
using OPCApp.Domain.Customer;
using OPCAPP.Domain.Dto;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;

namespace OPCApp.Customer.ViewModels
{
    [Export("CustomerRuturnGoodsViewModel", typeof (CustomerRuturnGoodsViewModel))]
    public class CustomerRuturnGoodsViewModel : BindableBase
    {
        public DelegateCommand CommandReturnGoodsSearch { get; set; }
        public DelegateCommand CommandCustomerReturnGoodsSave { get; set; }
        public RMAPost RmaPost { get; set; }
        private List<Order> orderList;
        public List<Order> OrderList
        {
            get { return orderList; }
            set { SetProperty(ref orderList, value); }
        }
        public ReturnGoodsGet ReturnGoodsGet { get; set; }

        public CustomerRuturnGoodsViewModel()
        {
            CommandCustomerReturnGoodsSave = new DelegateCommand(CustomerReturnGoodsSave);
            CommandReturnGoodsSearch = new DelegateCommand(ReturnGoodsSearch);
        }

        private void ReturnGoodsSearch()
        {
            OrderList.Clear();
            var list = AppEx.Container.GetInstance<ICustomerReturnGoods>().ReturnGoodsSearch(RmaPost);
            if (OrderList != null)
            {
                OrderList = list.ToList();
            }
        }

        private void CustomerReturnGoodsSave()
        {
          var bFlag=  AppEx.Container.GetInstance<ICustomerReturnGoods>().CustomerReturnGoodsSave(ReturnGoodsGet);
          MessageBox.Show(bFlag ? "客服退货成功" : "客服退货失败", "提示");
        }
    }
}
