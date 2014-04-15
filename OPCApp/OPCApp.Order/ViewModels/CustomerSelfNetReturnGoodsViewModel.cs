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
    public class CustomerSelfNetReturnGoodsViewModel : CustomerReturnGoodsViewModel
    {
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

