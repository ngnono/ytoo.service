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
      IList<Order> ReturnGoodsSearch(RMAPost shipComment);
      bool CustomerReturnGoodsSave(ReturnGoodsGet shipComment);
    }
}
