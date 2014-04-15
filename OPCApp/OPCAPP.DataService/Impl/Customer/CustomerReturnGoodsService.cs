using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using OPCApp.DataService.Common;
using OPCApp.DataService.Interface.Customer;
using OPCApp.Domain.Customer;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;

namespace OPCApp.DataService.Impl.Customer
{
    [Export(typeof (ICustomerReturnGoods))]
    public class CustomerReturnGoodsService : ICustomerReturnGoods
    {
        public IList<OPC_SaleRMA> ReturnGoodsSearch(ReturnGoodsGet returnGoodsGet)
        {
            try
            {
                PageResult<OPC_SaleRMA> lst = RestClient.GetPage<OPC_SaleRMA>("custom/GetOrder",
                    returnGoodsGet.ToString());
                return lst.Result;
            }
            catch (Exception ex)
            {
                return new List<OPC_SaleRMA>();
            }
        }

        public IList<OrderItem> GetOrderDetailByOrderNo(string orderNO)
        {
            try
            {
                PageResult<OrderItem> lst = RestClient.GetPage<OrderItem>("order/GetOrderItemsByOrderNo",
                    string.Format("orderNo={0}&pageIndex={1}&pageSize={2}", orderNO, 1, 300));
                return lst.Result;
            }
            catch (Exception ex)
            {
                return new List<OrderItem>();
            }
        }

        public bool CustomerReturnGoodsSave(RMAPost rmaPost)
        {
            try
            {
                return RestClient.Post("rma/CreateSaleRMA", rmaPost);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        //自主退货明细查询
        public IList<OrderItem> GetOrderDetailByOrderNoWithSelf(string orderNo)
        {
            try
            {//接口不对
                PageResult<OrderItem> lst = RestClient.GetPage<OrderItem>("order/GetOrderItemsByOrderNo",
                    string.Format("orderNo={0}&pageIndex={1}&pageSize={2}", orderNo, 1, 300));
                return lst.Result;
            }
            catch (Exception ex)
            {
                return new List<OrderItem>();
            }
        }
        //自主退货明细查询
        public IList<OPC_SaleRMA> ReturnGoodsSearchForSelf(ReturnGoodsGet returnGoodsGet)
        {
            try
            {//接口不对
                PageResult<OPC_SaleRMA> lst = RestClient.GetPage<OPC_SaleRMA>("custom/GetOrder",
                    returnGoodsGet.ToString());
                return lst.Result;
            }
            catch (Exception ex)
            {
                return new List<OPC_SaleRMA>();
            }
        }
    }
}