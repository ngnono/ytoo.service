using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net.Http;
using Intime.OPC.ApiClient;
using Intime.OPC.Domain.Models;
using OPCApp.DataService.Common;
using OPCApp.DataService.Interface;
using OPCApp.DataService.Interface.Trans;
using OPCApp.Domain;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;
using OPCApp.Infrastructure.DataService;

namespace OPCApp.DataService.Impl.Trans
{
    [Export(typeof(ICustomerInquiriesService))]
    public class CustomerInquiriesService : ICustomerInquiriesService
    {

        public PageResult<Order> GetOrder(string orderfilter)
        {
            try
            {
                var lst = RestClient.Get<Order>("order/getorder", orderfilter);
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
                var lst = RestClient.Get<OPC_Sale>("sale/GetSaleByOrderNo", orderNo);
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

          
            var lst = RestClient.Get<OPC_ShippingSale>("order/GetShipping", shippingfilter);
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
                var lst = RestClient.Get<Order>("order/GetOrderByShippingId", shippingId);
                return new PageResult<Order>(lst, lst.Count);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
