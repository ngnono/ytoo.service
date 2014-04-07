using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using OPCApp.DataService.Common;
using OPCApp.DataService.Interface.Trans;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;

namespace OPCApp.DataService.Impl.Trans
{
    [Export(typeof (ICustomerInquiriesService))]
    public class CustomerInquiriesService : ICustomerInquiriesService
    {
        public PageResult<Order> GetOrder(string orderfilter)
        {
            try
            {
                PageResult<Order> lst = RestClient.Get<Order>("order/getorder", orderfilter,1,100);
                return lst;//new PageResult<Order>(lst, lst.Count);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public PageResult<OPC_Sale> GetSaleByOrderNo(string orderNo)
        {
            try
            {
                PageResult<OPC_Sale> lst = RestClient.GetPage<OPC_Sale>("sale/GetSaleByOrderNo", orderNo);
                return lst;// new PageResult<OPC_Sale>(lst, lst.Count);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public PageResult<OPC_ShippingSale> GetShipping(string shippingfilter)
        {
            try
            {
                PageResult<OPC_ShippingSale> lst = RestClient.GetPage<OPC_ShippingSale>("trans/GetShipping", shippingfilter);
                return lst;// new PageResult<OPC_ShippingSale>(lst, lst.Count);
            }
            catch (Exception ex)
            {
                return null;
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
                return null;
            }
        }
    }
}