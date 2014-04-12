using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OPCApp.Domain.Customer;
using OPCApp.Domain.Models;

namespace OPCApp.DataService.Customer
{
    public interface ICustomerReturnSearch
  {
      IList<OrderDto> ReturnGoodsSearch(ReturnGoodsInfoGet goodInfoGet);
      IList<RMADto> GetRmaByOrderNo(string orderNo);
      IList<RmaDetail> GetRmaDetailByRmaNo(string rmaNo);
      bool AgreeReturnGoods(List<string> rmaNos);
    }
}
