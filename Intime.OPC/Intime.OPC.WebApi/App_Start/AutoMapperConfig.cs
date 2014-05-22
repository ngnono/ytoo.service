﻿using System;
using AutoMapper;
using Intime.OPC.Domain.BusinessModel;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Dto.Request;
using Intime.OPC.Domain.Enums;
using Intime.OPC.Domain.Enums.SortOrder;
using Intime.OPC.Domain.Models;
using Intime.OPC.Domain.Partials.Models;
using log4net.Util;

namespace Intime.OPC.WebApi.App_Start
{
    public static class AutoMapperConfig
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(AutoMapperConfig));
        public static void Config()
        {
            Mapper.CreateMap<Section, SectionDto>().ForMember(s => s.Code, opt => opt.MapFrom(d => d.SectionCode));
            var sectionModel = Mapper.CreateMap<SectionDto, Section>();
            sectionModel.ForMember(s => s.SectionCode, opt => opt.MapFrom(d => d.Code));
            sectionModel.ConstructUsing(v => new Section
            {
                ContactPerson = String.Empty,
                ContactPhone = String.Empty,
                Location = String.Empty,
                Status = 1,
                StoreCode = String.Empty,
                Id = v.Id,
                SectionCode = String.Empty
            });



            Mapper.CreateMap<Brand, BrandDto>().ForMember(s => s.Description, d => d.NullSubstitute(String.Empty));
            var brandModel = Mapper.CreateMap<BrandDto, Brand>();
            brandModel.ConstructUsing(v => new Brand
            {
                Logo = String.Empty,
                Group = String.Empty,
                WebSite = String.Empty,
                Id = v.Id
            });

            brandModel.ForMember(s => s.Description, d => d.NullSubstitute(String.Empty));

            var supplierModelMapper = Mapper.CreateMap<SupplierDto, OpcSupplierInfo>();
            supplierModelMapper.ForMember(d => d.SupplierName, opt => opt.MapFrom(s => s.Name));
            supplierModelMapper.ForMember(d => d.SupplierNo, opt => opt.MapFrom(s => s.Code));


            var supplierDtoMapper = Mapper.CreateMap<OpcSupplierInfo, SupplierDto>();
            supplierDtoMapper.ForMember(d => d.Name, opt => opt.MapFrom(s => s.SupplierName));
            supplierDtoMapper.ForMember(d => d.Code, opt => opt.MapFrom(s => s.SupplierNo));



            Mapper.CreateMap<OpcSupplierInfoClone, OpcSupplierInfo>();
            Mapper.CreateMap<BrandClone, Brand>();
            Mapper.CreateMap<SectionClone, Section>();
            Mapper.CreateMap<Store, StoreClone>();
            Mapper.CreateMap<StoreClone, Store>();

            Mapper.CreateMap<OPC_SaleClone, OPC_Sale>();
            Mapper.CreateMap<OPC_Sale, OPC_SaleClone>();

            //var saleordermodel = Mapper.CreateMap<OPC_Sale, SaleDto>();
            //saleordermodel.ForMember(d => d.ShippingStatusName, opt => opt.MapFrom(s => s.ShippingStatus.HasValue ? ((EnumSaleOrderStatus)s.ShippingStatus.Value).GetDescription() : String.Empty));
            //saleordermodel.ForMember(d => d.ShippingFee,
            //    opt => opt.MapFrom(s => s.ShippingFee.HasValue ? s.ShippingFee : 0m));
            //saleordermodel.ForMember(d => d.IfTrans,
            //    opt => opt.MapFrom(s => s.IfTrans.HasValue && s.IfTrans.Value ? "是" : "否"));
            //saleordermodel.ForMember(d => d.TransStatus,
            //    opt => opt.MapFrom(s => s.TransStatus.HasValue && s.TransStatus.Value == 1 ? "调拨" : String.Empty));
            //saleordermodel.ForMember(d => d.StatusName,
            //    opt => opt.MapFrom(s => ((EnumSaleOrderStatus)s.Status).GetDescription()));

            //saleordermodel.ForMember(d => d.StoreName, opt => opt.MapFrom(s => s.Store.Name));
            //saleordermodel.ForMember(d => d.SectionName, opt => opt.MapFrom(s => s.Section.Name));
            //saleordermodel.ForMember(d => d.TransNo, opt => opt.MapFrom(s => s.OrderTransaction.TransNo));

            Mapper.CreateMap<OrderTransactionClone, OrderTransaction>();
            Mapper.CreateMap<OrderTransaction, OrderTransactionClone>();
            Mapper.CreateMap<OPC_ShippingSaleClone, OPC_ShippingSale>();
            Mapper.CreateMap<OPC_ShippingSale, OPC_ShippingSaleClone>();

            Mapper.CreateMap<SaleOrderQueryRequest, SaleOrderFilter>();

            Mapper.CreateMap<OrderClone, Order>();
            Mapper.CreateMap<OrderClone, OrderClone>();

            Mapper.CreateMap<OrderDto, Order>();
            Mapper.CreateMap<Order, OrderDto>();


            Mapper.CreateMap<SalesOrderModel, SalesOrderDto>();
            var saledto = Mapper.CreateMap<SalesOrderModel, SaleDto>();
            saledto.ForMember(v => v.IfTrans,
                opt => opt.MapFrom(s => s.IfTrans == null ? "" : s.IfTrans.Value ? "是" : "否"));
            saledto.ForMember(v => v.TransStatus,
                opt => opt.MapFrom(s => s.TransStatus.HasValue && s.TransStatus.Value == 1 ? "调拨" : String.Empty));
            saledto.ForMember(v => v.StatusName, opt => opt.MapFrom(s => ((EnumSaleOrderStatus)s.Status).GetDescription()));

            saledto.ForMember(v => v.CashStatusName,
                opt =>
                    opt.MapFrom(
                        s => s.CashStatus.HasValue ? ((EnumCashStatus)s.CashStatus).GetDescription() : String.Empty));
            saledto.ForMember(v => v.Invoice,
                opt => opt.MapFrom(s => s.Invoice.HasValue && s.Invoice.Value ? "是" : String.Empty));
            saledto.ForMember(v => v.ShippingStatusName, opt => opt.MapFrom(s => s.ShippingStatus.HasValue ? ((EnumSaleOrderStatus)s.ShippingStatus).GetDescription() : String.Empty));

            Mapper.CreateMap<Store, StoreDto>();
            Mapper.CreateMap<StoreDto, Store>();
            Mapper.CreateMap<StoreRequest, StoreFilter>().ForMember(d => d.SortOrder, opt => opt.MapFrom(s => s.SortOrder == null ? StoreSortOrder.Default : (StoreSortOrder)s.SortOrder));

            var shippingSaleDto = Mapper.CreateMap<ShippingOrderModel, ShippingSaleDto>();
            shippingSaleDto.ForMember(v => v.PrintStatus,
                opt => opt.MapFrom(s => s.PrintTimes > 0 ? String.Format("{0}次", s.PrintTimes) : "未打印"));
            shippingSaleDto.ForMember(v => v.ShippingStatus, opt => opt.MapFrom(s => s.ShippingStatus.HasValue ? ((EnumSaleOrderStatus)s.ShippingStatus).GetDescription() : String.Empty));
            shippingSaleDto.ForMember(v => v.GoodsOutCode, opt => opt.MapFrom(s => s.Id.ToString()));

            Mapper.CreateMap<GetShippingSaleOrderRequest, ShippingOrderFilter>();

            var orderdto = Mapper.CreateMap<OrderModel, OrderDto>();
            orderdto.ForMember(d => d.Status, opt => opt.MapFrom(s => ((EnumOderStatus)s.Status).GetDescription()));
            orderdto.ForMember(d => d.IfReceipt,
                opt => opt.MapFrom(s => s.IfReceipt == null ? String.Empty : (s.IfReceipt.Value ? "是" : "否")));
        }
    }
}