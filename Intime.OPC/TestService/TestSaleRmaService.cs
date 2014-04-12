using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Dto.Custom;
using Intime.OPC.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestService
{
    [TestClass]
    public class TestSaleRmaService:TestService<ISaleRMAService>
    {
        [TestMethod]
        public void TestGetByReturnGoods()
        {
            ReturnGoodsRequest request=new ReturnGoodsRequest();

            request.StartDate = new DateTime(2010, 1, 1);
            request.EndDate = DateTime.Now.Date.AddDays(1);

            var lst=  Service.GetByReturnGoods(request,1);
            AssertList<SaleRmaDto>(lst);
        }

        [TestMethod]
        public void TestCreateSaleRma()
        {
            RMAPost post=new RMAPost();
            post.CustomFee = 123123;
            post.StoreFee = 123;
            post.ReturnProducts.Add(new KeyValuePair<int, int>(7,1 ));
            post.ReturnProducts.Add(new KeyValuePair<int, int>(6, 13));
            post.RealRMASumMoney = 123123;

            post.OrderNo = "114201404118";
            post.Remark = "unitTest";

            Service.CreateSaleRMA(1,post);
        }

        [TestMethod]
        public void TestGetByPack_PackageReceiveDto()
        {

            PackageReceiveRequest dto = new PackageReceiveRequest();
            dto.StartDate = new DateTime(2014, 4, 1);
            dto.EndDate = DateTime.Now.Date;
            dto.OrderNo = "114";
            dto.SaleOrderNo = "114";

            var lst = Service.GetByPack(dto);
            Assert.IsNotNull(lst);
            Assert.AreNotEqual(0, lst.TotalCount);
        }
    }
}
