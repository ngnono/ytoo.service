using Intime.OPC.Domain;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Dto.Request;
using Intime.OPC.Repository.Support;
using Intime.OPC.Service.Support;
using Intime.OPC.WebApi.Controllers;
using NUnit.Framework;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;

namespace Intime.OPC.WebApi.Test.ControllerTest
{
    [TestFixture]
    public class StoreControllerTest : BaseControllerTest
    {
        private StoreController _controller;

        public StoreController GetController()
        {
            var storeRepository = new StoreRepository();

            _controller = new StoreController(new StoreService(storeRepository));

            _controller.Request = new HttpRequestMessage();
            _controller.Request.SetConfiguration(new HttpConfiguration());

            return _controller;
        }

        [SetUp]
        public void TestInit()
        {
            _controller = GetController();
        }

        [TearDown]
        public void TestCleanUp()
        {
            _controller = null;
        }


        [Test()]
        public void GetListTest_1()
        {
            _controller.Request.Method = HttpMethod.Get;
            var actual = _controller.GetList(new StoreRequest()
            {
                Page = 1,
                PageSize = 10
            }, 0) as OkNegotiatedContentResult<PagerInfo<StoreDto>>;

            Assert.IsNotNull(actual);
        }

        [Test()]
        public void GetListTest_3([Values("银泰")]string namePrefix)
        {
            _controller.Request.Method = HttpMethod.Get;

            var actual = _controller.GetList(new StoreRequest
            {
                Page = 1,
                PageSize = 20,
                NamePrefix = namePrefix,
            }, 0) as OkNegotiatedContentResult<PagerInfo<StoreDto>>;

            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Content.Datas.Count > 0);
        }

        [Test()]
        public void GetListTest_4([Values("杭州市所有银泰百货商场")]string name)
        {
            _controller.Request.Method = HttpMethod.Get;

            var actual = _controller.GetList(new StoreRequest
            {
                Page = 1,
                PageSize = 20,
                Name = name
            }, 0) as OkNegotiatedContentResult<PagerInfo<StoreDto>>;

            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Content.Datas.Count > 0);
        }
    }
}