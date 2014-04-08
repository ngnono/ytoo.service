using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            ReturnGoodsGet request=new ReturnGoodsGet();

            request.StartDate = new DateTime(2010, 1, 1);
            request.EndDate = DateTime.Now.Date.AddDays(1);

            var lst=  Service.GetByReturnGoods(request);
            Assert.IsNotNull(lst);
            Assert.AreNotEqual(lst.Count,0);
        }
    }
}
