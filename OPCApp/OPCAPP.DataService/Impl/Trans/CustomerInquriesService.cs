using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Intime.OPC.Domain.Models;
using OPCApp.DataService.Common;
using OPCApp.DataService.Interface.Trans;
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
                IList<Order> lst = RestClient.Get<Order>("order/getorder", orderfilter);
                return new PageResult<Order>(lst, lst.Count);
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
                IList<OPC_Sale> lst = RestClient.Get<OPC_Sale>("sale/GetSaleByOrderNo", orderNo);
                return new PageResult<OPC_Sale>(lst, lst.Count);
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
                IList<OPC_ShippingSale> lst = RestClient.Get<OPC_ShippingSale>("order/GetShipping", shippingfilter);
                return new PageResult<OPC_ShippingSale>(lst, lst.Count);
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
                IList<Order> lst = RestClient.Get<Order>("order/GetOrderByShippingId", shippingId);
                return new PageResult<Order>(lst, lst.Count);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}