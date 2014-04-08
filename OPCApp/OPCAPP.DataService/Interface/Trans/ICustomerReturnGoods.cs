using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OPCApp.Domain.Customer;
using OPCApp.Domain.Models;

namespace OPCApp.DataService.Interface.Trans
{
  public  interface ICustomerReturnGoods
    {
      IList<OPC_SaleRMA> ReturnGoodsSearch(ReturnGoodsGet shipComment);
      IList<OrderItem> GetOrderDetailByOrderNo(string orderNO);
      bool CustomerReturnGoodsSave(RMAPost shipComment);
    }
}
