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
        //退货订单
      IList<OrderDto> ReturnGoodsRmaSearch(ReturnGoodsInfoGet goodInfoGet);
     //物流退回
      IList<OrderDto> ReturnGoodsTransSearch(ReturnGoodsInfoGet goodInfoGet);
        //赔偿退回
      IList<OrderDto> ReturnGoodsFinancialSearch(ReturnGoodsInfoGet goodInfoGet);
      IList<RMADto> GetRmaByOrderNo(string orderNo);
      IList<RmaDetail> GetRmaDetailByRmaNo(string rmaNo);
      bool AgreeReturnGoods(List<string> rmaNos);
    }
}
