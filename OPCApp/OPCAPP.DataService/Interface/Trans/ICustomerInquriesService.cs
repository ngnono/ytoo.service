﻿using Intime.OPC.Domain.Models;
using OPCApp.Domain;
using OPCApp.Infrastructure;

namespace OPCApp.DataService.Interface.Trans
{
    public interface ICustomerInquiriesService
    {

        
        PageResult<Order> GetOrder(string filter);
        PageResult<OPC_Sale> GetSaleByOrderNo(string orderNo);


        PageResult<OPC_ShippingSale> GetShipping(string filter);
        PageResult<Order> GetOrderByShippingId(string shippingId);
    }
}