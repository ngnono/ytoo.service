﻿using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using OPCApp.DataService.Customer;
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
    [Export(typeof(CustomerReturnSearchTransViewModel))]
    public class CustomerReturnSearchTransViewModel : CustomerReturnSearchViewModel
    {
        public override void SearchGoodsInfo()
        {
            OrderDtoList =
              AppEx.Container.GetInstance<ICustomerReturnSearch>().ReturnGoodsTransSearch(ReturnGoodsInfoGet).ToList();
        }
        public override void SearchRmaDtoListInfo()
        {
            if (OrderDto == null)
            {
                if (RmaDetailList != null) RmaDetailList.Clear();
                return;
            }
            var rmaList = AppEx.Container.GetInstance<ICustomerReturnSearch>().GetRmaTransByOrderNo(OrderDto.OrderNo).ToList();
            RMADtoList = rmaList;
        }
    }
}