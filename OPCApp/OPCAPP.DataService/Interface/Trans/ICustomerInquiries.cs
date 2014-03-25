﻿using Intime.OPC.Domain.Models;
using OPCApp.Domain;
using OPCApp.Infrastructure;

namespace OPCApp.DataService.Interface.Trans
{
    public interface ICustomerInquiries
    {
        PageResult<Order> GetOrder(string filter);
        PageResult<OPC_Sale> GetSaleByOrderId(string orderId);
    }
}
