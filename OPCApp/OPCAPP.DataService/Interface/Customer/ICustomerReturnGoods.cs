using System.Collections.Generic;
using OPCApp.Domain.Customer;
using OPCApp.Domain.Models;

namespace OPCApp.DataService.Interface.Customer
{
    public interface ICustomerReturnGoods
    {
        IList<OPC_SaleRMA> ReturnGoodsSearch(ReturnGoodsGet returnGoodsGet);
        IList<OrderItem> GetOrderDetailByOrderNo(string orderNO);
        bool CustomerReturnGoodsSave(RMAPost shipComment);
        IList<OrderItem> GetOrderDetailByOrderNoWithSelf(string orderNo);

        IList<OPC_SaleRMA> ReturnGoodsSearchForSelf(ReturnGoodsGet returnGoodsGet);
        bool CustomerReturnGoodsSelfPass(RMAPost rmaPost);
        bool CustomerReturnGoodsSelfReject(RMAPost rmaPost);
    }
}