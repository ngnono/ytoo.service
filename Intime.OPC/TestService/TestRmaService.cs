﻿using System;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Dto.Custom;
using Intime.OPC.Domain.Enums;
using Intime.OPC.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestService
{
    [TestClass]
    public class TestRmaService : TestService<IRmaService>
    {
        [TestMethod]
        public void TestGetAll_PackageReceiveDto()
        {
            //IList<RMADto> GetAll(PackageReceiveDto dto);
            PackageReceiveRequest dto=new PackageReceiveRequest();
            dto.StartDate = new DateTime(2014, 4, 1);
            dto.EndDate = DateTime.Now.Date;
            dto.OrderNo = "114";
            dto.SaleOrderNo = "114";

            var lst = Service.GetAll(dto);
           AssertList<RMADto>(lst);
        }
        [TestMethod]
        public void TestGetDetails_rmaNo()
        {
            var lst = Service.GetDetails("114201404086001",1,1000);
            AssertList<RmaDetail>(lst);
        }

        [TestMethod]
        public void TestGetByOrderNo()
        {
            var lst = Service.GetByOrderNo("114201404087",EnumRMAStatus.ShipNoReceive, EnumReturnGoodsStatus.NoProcess,1,1000);
            AssertList<RMADto>(lst);
        }
       
    }
}