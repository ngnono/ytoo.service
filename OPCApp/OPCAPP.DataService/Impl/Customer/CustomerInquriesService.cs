using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using OPCApp.DataService.Common;
using OPCApp.DataService.Interface.Customer;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;

namespace OPCApp.DataService.Impl.Customer
{
    [Export(typeof (ICustomerInquiriesService))]
    public class CustomerInquiriesService : ICustomerInquiriesService
    {
        public PageResult<Order> GetOrder(string orderfilter)
        {
            try
            {
                PageResult<Order> lst = RestClient.Get<Order>("order/getorder", orderfilter, 1, 100);
                return lst; //new PageResult<Order>(lst, lst.Count);
            }
            catch (Exception ex)
            {
                return new PageResult<Order>(new List<Order>(), 0);
            }
        }

        public PageResult<OPC_Sale> GetSaleByOrderNo(string orderNo)
        {
            try
            {
                PageResult<OPC_Sale> lst = RestClient.GetPage<OPC_Sale>("sale/GetSaleByOrderNo", orderNo);
                return lst; // new PageResult<OPC_Sale>(lst, lst.Count);
            }
            catch (Exception ex)
            {
                return new PageResult<OPC_Sale>(new List<OPC_Sale>(), 0);
            }
        }

        public PageResult<OPC_ShippingSale> GetShipping(string shippingfilter)
        {
            try
            {
                PageResult<OPC_ShippingSale> lst = RestClient.GetPage<OPC_ShippingSale>("trans/GetShipping",
                    shippingfilter);
                return lst; // new PageResult<OPC_ShippingSale>(lst, lst.Count);
            }
            catch (Exception ex)
            {
                return new PageResult<OPC_ShippingSale>(new List<OPC_ShippingSale>(), 0);
            }
        }

        public PageResult<Order> GetOrderByShippingId(string shippingId)
        {
            try
            {
                IList<Order> lst = RestClient.Get<Order>("order/GetOrderByShippingSaleNo", shippingId);
                return new PageResult<Order>(lst, lst.Count);
            }
            catch (Exception ex)
            {
                return new PageResult<Order>(new List<Order>(), 0);
            }
        }

        //public bool SetCustomerMoneyGoods(List<string> rmaNoList)
        //{
        //    throw new NotImplementedException();
        //}

        //赔偿金额退回审核
        public bool SetCustomerMoneyGoods(List<string> rmaNoList)
        {
            try
            {
                return RestClient.Post("rma/SetSaleRmaServiceApprove",rmaNoList);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #region 缺货提醒
        public bool SetCannotReplenish(List<string> saleOrderNoList)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}