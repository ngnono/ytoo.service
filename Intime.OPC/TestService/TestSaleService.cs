﻿using System;
using Intime.OPC.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestService
{
    [TestClass]
    public class TestSaleService:TestService<ISaleService>
    {
        [TestMethod]
        public void TestGetSaleDetail()
        {
           var lst= Service.GetSaleOrderDetails("114201404087-001", 1, 1, 1000);
           Debug(lst);
            Assert.IsNotNull(lst);
            
        }


        [TestMethod]
        public void TestGetSaleByOrderNo()
        {
            var lst = Service.GetByOrderNo("1142014041920", 1, 1, 1000);
            Debug(lst);
            Assert.IsNotNull(lst);

        }
    }
}
