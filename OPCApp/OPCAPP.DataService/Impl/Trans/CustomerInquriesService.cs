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
            var lst = RestClient.Get<Order>("order/getorder", orderfilter);
            return new PageResult<Order>(lst, lst.Count);
        }

        public PageResult<OPC_Sale> GetSaleByOrderNo(string orderNo)
        {
            var lst = RestClient.Get<OPC_Sale>("sale/GetSaleByOrderNo", orderNo);
            return new PageResult<OPC_Sale>(lst, lst.Count);
        }

        public PageResult<OPC_ShippingSale> GetShipping(string shippingfilter)
        {
            var lst = RestClient.Get<OPC_ShippingSale>("order/GetShipping", shippingfilter);
            return new PageResult<OPC_ShippingSale>(lst, lst.Count);
        }

        public PageResult<Order> GetOrderByShippingId(string shippingId)
        {
            var lst = RestClient.Get<Order>("order/GetOrderByShippingId", shippingId);
            return new PageResult<Order>(lst, lst.Count);
        }
    }
}
