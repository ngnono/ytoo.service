using System;
using AutoMapper;
using Intime.OPC.Domain.BusinessModel;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Dto.Request;
using Intime.OPC.Domain.Enums;
using Intime.OPC.Domain.Models;
using Intime.OPC.Domain.Partials.Models;

namespace Intime.OPC.WebApi.App_Start
{
    public static class AutoMapperConfig
    {
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

            Mapper.CreateMap<OPC_Sale, SaleOrderModel>();
            Mapper.CreateMap<SaleOrderModel, OPC_Sale>();

            var saleordermodel = Mapper.CreateMap<SaleOrderModel, SaleDto>();
            saleordermodel.ForMember(d => d.ShippingStatusName, opt => opt.MapFrom(s => s.ShippingStatus.HasValue ? ((EnumSaleOrderStatus)s.ShippingStatus.Value).GetDescription() : String.Empty));
            saleordermodel.ForMember(d => d.ShippingFee,
                opt => opt.MapFrom(s => s.ShippingFee.HasValue ? s.ShippingFee : 0m));
            saleordermodel.ForMember(d => d.IfTrans,
                opt => opt.MapFrom(s => s.IfTrans.HasValue && s.IfTrans.Value ? "是" : "否"));
            saleordermodel.ForMember(d => d.TransStatus,
                opt => opt.MapFrom(s => s.TransStatus.HasValue && s.TransStatus.Value == 1 ? "调拨" : String.Empty));
            saleordermodel.ForMember(d => d.StatusName,
                opt => opt.MapFrom(s => ((EnumSaleOrderStatus)s.Status).GetDescription()));

            saleordermodel.ForMember(d => d.StoreName, opt => opt.MapFrom(s => s.Store.Name));
            saleordermodel.ForMember(d => d.SectionName, opt => opt.MapFrom(s => s.Section.Name));
            saleordermodel.ForMember(d => d.TransNo, opt => opt.MapFrom(s => s.OrderTransaction.TransNo));


            Mapper.CreateMap<OrderTransactionClone, OrderTransaction>();
            Mapper.CreateMap<OrderTransaction, OrderTransactionClone>();

            Mapper.CreateMap<SaleOrderQueryRequest, SaleOrderFilter>();
        }
    }
}