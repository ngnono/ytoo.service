using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using OPCApp.DataService.Customer;
using OPCApp.Domain.Customer;
using OPCApp.Infrastructure;

namespace OPCApp.Customer.ViewModels
{
    [Export(typeof (CustomerReturnSearchTransViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CustomerReturnSearchTransViewModel : CustomerReturnSearchViewModel
    {
        public CustomerReturnSearchTransViewModel()
        {
            IsShowCustomerReViewBtn = false;
        }
        public override void SearchGoodsInfo()
        {
            try
            {
                OrderDtoList =
                    AppEx.Container.GetInstance<ICustomerReturnSearch>().ReturnGoodsTransSearch(ReturnGoodsInfoGet).ToList();
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
                    AppEx.Container.GetInstance<ICustomerReturnSearch>().GetRmaTransByOrderNo(OrderDto.OrderNo).ToList();
                RMADtoList = rmaList;
            }
            catch { };
        }
    }
}