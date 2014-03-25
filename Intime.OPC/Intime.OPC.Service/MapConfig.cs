using System;
using System.Collections.Generic;

using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Models;
using Intime.OPC.Service.Map;

namespace Intime.OPC.Service
{
    public  class MapConfig
    {
        public static void Config()
        {
            //Mapper.CreateMap<OPC_Sale, SaleDto>();
           // Mapper.CreateMap<Order, OrderDto>((o) => { return converDto(o); });
             
            
        }

        private static OrderDto converDto(Order o)
        {
            var t = new OrderDto();
            t.Id = o.Id;
            t.BuyDate = o.CreateDate;
            t.CustomerAddress = o.ShippingAddress;
            t.CustomerName = o.ShippingContactPerson;
            t.CustomerPhone = o.ShippingContactPhone;
            t.OrderNo = o.OrderNo;
            t.OrderSouce = o.OrderSource;
            t.ExpressNo = o.ShippingNo;

            //todo 没有映射完成
            return t;
        }
    }
}
