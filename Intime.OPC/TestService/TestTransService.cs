using System;
using Intime.OPC.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestService
{
    [TestClass]
    public class TestTransService : TestService<ITransService>
    {
        [TestMethod]
        public void TestGetSaleByShippingSaleN()
        {
            var lst= Service.GetSaleByShippingSaleNo("20140409-001");
            Assert.IsNotNull(lst);

            Assert.AreNotEqual(lst.Count,0);
        }
    }
}
