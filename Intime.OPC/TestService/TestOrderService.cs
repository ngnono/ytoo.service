using System;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Dto.Custom;
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
    }
}
