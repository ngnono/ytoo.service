using System;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Enums;
using Intime.OPC.Domain.Models;
using Intime.OPC.Service.Map;

namespace Intime.OPC.Service
{
    public class MapConfig
    {
        public static void Config()
        {
            //Mapper.CreateMap<OPC_Sale, SaleDto>();
            Mapper.CreateMap<Order, OrderDto>(o => converDto(o));

            var map = AutoMapper.Mapper.CreateMap<OPC_Sale, SaleDto>();
            map.ForMember(d => d.StatusName, opt => opt.MapFrom(t => GetSaleOrderStatusName(t.Status)));
            map.ForMember(d => d.CashStatusName, opt => opt.MapFrom(t => GetCashStatusName(t.CashStatus))); 
          
        }

        private static string GetSaleOrderStatusName(int status)
        {
            var orderStatus = (EnumSaleOrderStatus)status;
            return  orderStatus.GetDescription();
        }

        private static string GetCashStatusName(int? status)
        {
            if (!status.HasValue)
            {
                return "";
            }
            var orderStatus = (EnumCashStatus)status.Value;
            return orderStatus.GetDescription();
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
            t.IfReceipt = o.NeedInvoice.HasValue && o.NeedInvoice.Value ? "是" : "否";
            t.MustPayTotal = o.TotalAmount;
            //t.OrderChannelNo=o.
            //t.OutGoodsDate=o.
            //t.OutGoodsType=
            t.PaymentMethodName = o.PaymentMethodName;
            t.PostCode = o.ShippingZipCode;
            //t.Quantity=
            t.ReceiptContent = o.InvoiceDetail;
            t.ReceiptHead = o.InvoiceSubject;
            t.ShippingNo = o.ShippingNo;

            var orderStatus = (EnumOderStatus) o.Status;
            t.Status = orderStatus.GetDescription();

            t.TotalAmount = o.TotalAmount;

            //todo 没有映射完成
            return t;
        }
    }
}