using System;
using System.Data.Entity.ModelConfiguration.Configuration;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Dto.Custom;
using Intime.OPC.Domain.Dto.Financial;
using Intime.OPC.Domain.Models;
using Intime.OPC.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestService
{
    [TestClass]
    public class TestOrderService:TestService<IOrderService>
    {
        [TestMethod]
        public void TestGetByReturnGoodsInfo()
        {
            


            ReturnGoodsInfoRequest goods=new ReturnGoodsInfoRequest();
            goods.StartDate = new DateTime(2010, 1, 1);
            goods.EndDate = DateTime.Now.Date.AddDays(1);
            

            var lst = Service.GetByReturnGoodsInfo(goods);
            AssertList<OrderDto>(lst);
        }

         [TestMethod]
        public void TestGetOrderItemsByOrderNo()
         {
             var lst = Service.GetOrderItems("1142014041211",1,1000);
            AssertList<OrderItemDto>(lst);
        }


         [TestMethod]
        public void TestWebSiteStatSaleDetail()
        {
             SearchStatRequest dto=new SearchStatRequest();
             dto.StartTime = new DateTime(2000, 1, 1);
             dto.EndTime = DateTime.Now;
             var o = Service.WebSiteStatSaleDetail(dto);
             Assert.IsNotNull(o);
             Assert.AreNotEqual(o.Count,0);
        }

         [TestMethod]
         public void TestWebSiteStatReturnDetail()
         {
             SearchStatRequest dto = new SearchStatRequest();
             dto.StartTime = new DateTime(2000, 1, 1);
             dto.EndTime = DateTime.Now;
             var o = Service.WebSiteStatReturnDetail(dto);
             Assert.IsNotNull(o);
             Assert.AreNotEqual(o.Count, 0);
         }

         [TestMethod]
         public void TestWebSiteCashier()
         {
             var dto = new SearchCashierRequest();
             //dto.StartTime = new DateTime(2000, 1, 1);
             dto.StartTime = DateTime.Now;
             dto.EndTime = DateTime.Now;
             var o = Service.WebSiteCashier(dto);
             Assert.IsNotNull(o);
             Assert.AreNotEqual(o.Count, 0);
         }
    }
}
