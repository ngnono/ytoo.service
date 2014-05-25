using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nest;
using Yintai.Hangzhou.Model.ES;
using Yintai.Hangzhou.WebApiCore.Areas.Gg.Controllers;
using Yintai.Hangzhou.WebApiCore.Areas.Gg.ViewModels;

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
                    query.Term(p => p.IsSystem, true)));
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

        private int Skip(int pageIndex, int pageSize)
        {
            pageIndex = Math.Max(pageIndex - 1, 0);
            return pageIndex * pageSize;
        }

        [TestMethod]
        public void TestProduct()
        {
            int pageIndex = 1;
            int pageSize = 10;

            pageSize = Math.Min(pageSize, 100);

            string lastUpdate = "2012-5-21T00:00:10";

            // ===========================================================================
            //  根据最后更新时间获取商品信息
            // ===========================================================================

            var result = ElasticClient.Search<ESProducts>(body =>
                body.From(Skip(pageIndex, pageSize))
                    .Size(pageSize)
                    .SortAscending(p => p.CreatedDate)
                    .Query(q =>
                        q.Term(p => p.IsSystem, false) &
                        q.Range(p => p.GreaterOrEquals(lastUpdate).OnField(f => f.CreatedDate)
                    )
                ));
        }
    }
}
