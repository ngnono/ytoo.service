using System;
using AutoMapper;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Models;

namespace Intime.OPC.WebApi.App_Start
{
    public static class AutoMapperConfig
    {
        public static void Config()
        {
            Mapper.CreateMap<Section, SectionDto>();
            var sectionDto = Mapper.CreateMap<SectionDto, Section>();



            Mapper.CreateMap<Brand, BrandDto>().ForMember(s => s.Description, d => d.NullSubstitute(String.Empty));
            var brandModel = Mapper.CreateMap<BrandDto, Brand>();
            brandModel.ConstructUsing(v => new Brand()
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
        }
    }
}