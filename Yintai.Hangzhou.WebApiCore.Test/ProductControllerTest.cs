using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nest;
using Yintai.Hangzhou.Model.ES;

namespace Yintai.Hangzhou.WebApiCore.Test
{
    [TestClass]
    public class ProductControllerTest
    {
        [TestMethod]
        public void Test()
        {
            var result = ElasticClient.Search<ESProduct>(body =>
                // return first 5 results, default is 10
                body.Size(5).Query(query =>
                    query.Term(p=>p.IsSystem,true)));
            Assert.IsNotNull(result);
        }

        private static ElasticClient ElasticClient
        {
            get
            {
                var uriString = "http://218.244.139.79:9200/";
                var searchBoxUri = new Uri(uriString);
                var settings = new ConnectionSettings(searchBoxUri);
                settings.SetDefaultIndex("intime");
                return new ElasticClient(settings);

            }
        }
    }
}
