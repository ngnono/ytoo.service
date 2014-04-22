using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intime.OPC.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestService
{
    
    [TestClass]
    public class TestAccountService : TestService<IAccountService>
    {

        [TestMethod]
        public void TestGet()
        {
            var dd = Service.Get("wxh", "123456");
            Assert.AreEqual(dd.Name,"wxh");
        }
        [TestMethod]
        public void TestDelete()
        {
            var dd = Service.DeleteById(20);
            Assert.AreNotEqual(dd,true);
        }
    }
}
