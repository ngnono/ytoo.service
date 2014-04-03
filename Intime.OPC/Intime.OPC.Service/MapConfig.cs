using System;
using AutoMapper;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Enums;
using Intime.OPC.Domain.Models;

namespace Intime.OPC.Service
{
    public class MapConfig
    {
        public static void Config()
        {
            IMappingExpression<OPC_ShippingSale, ShippingSaleDto> map3 =
                Mapper.CreateMap<OPC_ShippingSale, ShippingSaleDto>();
            map3.ForMember(t => t.ExpressCode, o => o.MapFrom(t1 => t1.ShippingCode));
            map3.ForMember(t => t.GoodsOutCode, o => o.MapFrom(t1 => t1.ShippingCode));
            map3.ForMember(t => t.ShippingStatus, o => o.MapFrom((t1) => GetSaleOrderStatusName(t1.ShippingStatus)));
            map3.ForMember(t => t.GoodsOutDate, o => o.MapFrom(t1 => t1.CreateDate));

            map3.ForMember(t => t.ShipCompanyName, o => o.MapFrom(t1 => t1.ShipViaName));


            IMappingExpression<Order, OrderDto> map1 = Mapper.CreateMap<Order, OrderDto>();
            map1.ConvertUsing(t => converDto(t));

            IMappingExpression<OPC_Sale, SaleDto> map = Mapper.CreateMap<OPC_Sale, SaleDto>();
            map.ForMember(d => d.StatusName, opt => opt.MapFrom(t => GetSaleOrderStatusName(t.Status)));
            map.ForMember(d => d.CashStatusName, opt => opt.MapFrom(t => GetCashStatusName(t.CashStatus)));
            map.ForMember(d => d.ShippingStatusName, opt => opt.MapFrom(t => GetSaleOrderStatusName(t.ShippingStatus)));

            IMappingExpression<OPC_SaleDetail, SaleDetailDto> map2 = Mapper.CreateMap<OPC_SaleDetail, SaleDetailDto>();

            //todo 销售单明细 匹配
        }

        private static string GetSaleOrderStatusName(int status)
        {
            var orderStatus = (EnumSaleOrderStatus)status;
            return orderStatus.GetDescription();
        }
        private static string GetSaleOrderStatusName(int? status)
        {
            if (!status.HasValue)
            {
                return "";
            }
            return GetSaleOrderStatusName(status.Value);
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

            var orderStatus = (EnumOderStatus)o.Status;
            t.Status = orderStatus.GetDescription();

            t.TotalAmount = o.TotalAmount;

            //todo 没有映射完成
            return t;
        }
    }
}