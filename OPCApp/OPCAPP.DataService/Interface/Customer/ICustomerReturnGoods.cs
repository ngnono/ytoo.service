using System.Collections.Generic;
using OPCApp.Domain.Customer;
using OPCApp.Domain.Models;

namespace OPCApp.DataService.Interface.Customer
{
    public interface ICustomerReturnGoods
    {
        IList<OPC_SaleRMA> ReturnGoodsSearch(ReturnGoodsGet shipComment);
        IList<OrderItem> GetOrderDetailByOrderNo(string orderNO);
        bool CustomerReturnGoodsSave(RMAPost shipComment);
    }
}