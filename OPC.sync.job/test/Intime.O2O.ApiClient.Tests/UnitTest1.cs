using System;
using Intime.O2O.ApiClient.Request;
using Intime.O2O.ApiClient.Response;
using Intime.OPC.Job.Product.ProductSync.Supports.Intime.Jobs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Intime.O2O.ApiClient.Tests
{
    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        public void TestMethod1()
        {
            var client = new DefaultApiClient("http://guide.intime.com.cn:8008/production-o2o-rs/api/", "intime", "intime");

            var reponse = client.Post(new GetProductsRequest()
            {
                Data = new GetProductsRequestData()
                {
                    PageIndex = 1,
                    LastUpdate = "2013-11-11",
                    PageSize = 20
                }
            });

            client.Post(new GetProductsRequest()
            {
                Data = new GetProductsRequestData()
                {
                    PageIndex = 1,
                    LastUpdate = "2013-11-15",
                    PageSize = 20
                }
            });

            client.Post(new GetStoresRequest()
            {
                Data = new GetStoresRequestData()
                {
                    PageIndex = 1,
                    PageSize = 20
                }
            });

            client.Post(new GetProductImagesRequest()
            {
                Data = new GetProductImagesRequestData()
                {
                    PageIndex = 1,
                    PageSize = 20,
                    LastUpdate = "2013-11-15"
                }
            });

            String message = reponse.Message;
        }
    }
}
