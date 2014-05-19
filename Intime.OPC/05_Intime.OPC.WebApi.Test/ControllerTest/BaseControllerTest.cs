using Intime.OPC.WebApi.App_Start;
using NUnit.Framework;

namespace Intime.OPC.WebApi.Test.ControllerTest
{
    [TestFixture]
    public class BaseControllerTest
    {
        [TestFixtureSetUp]
        public void ClassInit()
        {
            AutoMapperConfig.Config();
        }

        [TestFixtureTearDown]
        public void ClassClear()
        {

        }
    }
}