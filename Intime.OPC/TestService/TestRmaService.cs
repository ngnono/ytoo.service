using System;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Dto.Custom;
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
            PackageReceiveDto dto=new PackageReceiveDto();
            dto.StartDate = new DateTime(2014, 4, 1);
            dto.EndDate = DateTime.Now.Date;
            dto.OrderNo = "114";
            dto.SaleOrderNo = "114";

            var lst = Service.GetAll(dto);
           AssertList<RMADto>(lst);
        }
    }
}
