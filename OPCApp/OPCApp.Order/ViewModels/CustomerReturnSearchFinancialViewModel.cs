using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using OPCApp.DataService.Customer;
using OPCApp.Domain.Customer;
using OPCApp.Infrastructure;
using System.Windows.Forms;
using System;

namespace OPCApp.Customer.ViewModels
{
    [Export(typeof (CustomerReturnSearchFinancialViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CustomerReturnSearchFinancialViewModel : CustomerReturnSearchViewModel
    {
 
        public override void SearchGoodsInfo()
        {
            try
            {
                OrderDtoList =
                    AppEx.Container.GetInstance<ICustomerReturnSearch>()
                        .ReturnGoodsFinancialSearch(ReturnGoodsInfoGet)
                        .ToList();
            }
            catch { };
        }

        public override void SearchRmaDtoListInfo()
        {
            if (OrderDto == null)
            {
                if (RmaDetailList != null) RmaDetailList.Clear();
                return;
            }
            try
            { 
                List<RMADto> rmaList =
                    AppEx.Container.GetInstance<ICustomerReturnSearch>().GetRmaFinancialByOrderNo(OrderDto.OrderNo).ToList();
                RMADtoList = rmaList;
            }
            catch { };
        }
      
    }
}