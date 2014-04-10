using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using OPCApp.DataService.Common;
using OPCApp.DataService.Interface.Customer;
using OPCApp.DataService.Interface.Trans;
using OPCApp.Domain.Customer;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;

namespace OPCApp.DataService.Impl.Customer
{
    [Export(typeof(ICustomerReturnGoods))]
    public class CustomerReturnGoodsService : ICustomerReturnGoods
    {

        public IList<OPC_SaleRMA> ReturnGoodsSearch(Domain.Customer.ReturnGoodsGet returnGoodsGet)
        {
            try
            {
                return RestClient.Get<OPC_SaleRMA>("custom/GetOrder", returnGoodsGet.ToString());
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public IList<OrderItem> GetOrderDetailByOrderNo(string orderNO)
        {
            try
            {
                return RestClient.Get<OrderItem>("order/GetOrderItemsByOrderNo", string.Format("orderNo={0}", orderNO));
            }
            catch (Exception ex)
            {
                return new List<OrderItem>();
            }
        }

        public bool CustomerReturnGoodsSave(Domain.Customer.RMAPost rmaPost)
        {
            try
            {
                return RestClient.Post<RMAPost>("rma/CreateSaleRMA", rmaPost);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}