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
            var dd = Service.Get("admin", "admin");
            Assert.AreEqual(dd.Name,"admin");
        }
    }
}
