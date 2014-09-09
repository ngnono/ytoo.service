using com.intime.fashion.data.sync.Taobao.Request;
using com.intime.fashion.data.sync.Taobao.Response;
using com.intime.fashion.service.search;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using Yintai.Hangzhou.Model.ES;

namespace com.intime.fashion.data.sync.Taobao.Fetcher
{
    public class IMSProductFetcher:IProductFetcher
    {
        private readonly IElasticClient _elasticClient;

        public IMSProductFetcher(IElasticClient client)
        {
            this._elasticClient = client;
        }

        public IMSProductFetcher():this(SearchLogic.GetClient())
        {
        }

        public FetchResponse<ESProduct> Fetch(FetchRequest request)
        {
            int pageIndex = request.PageIndex;
            int pageSize = request.PageSize;

            pageSize = Math.Min(pageSize, 100);

            DateTime lastUpdate = request.LastUpdateTime;

            var result = _elasticClient.Search<ESProduct>(body =>
                body.From(Skip(pageIndex, pageSize))
                    .Size(pageSize)
                    .SortAscending(p => p.UpdatedDate)
                    .Query(q =>
                        q.Term(p => p.IsSystem, false) &
                        q.Term(p => p.Status, 1) &
                        q.Range(p => p.GreaterOrEquals(lastUpdate).OnField(f => f.UpdatedDate)
                    )
                ));

            IEnumerable<ESProduct> mergedProducts =
                result.Documents.GroupBy(x => string.Format("{0}/{1}/{2}", x.Store.GroupId,x.Brand.Id, x.UpcCode),
                    x => x).Select(p => p.ToList().OrderBy(x => x.CreatedDate).FirstOrDefault());

            return new FetchResponse<ESProduct>()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                Total = mergedProducts.Count(),
                Data = mergedProducts
            };
        }

        private int Skip(int pageIndex, int pageSize)
        {
            pageIndex = Math.Max(pageIndex - 1, 0);
            return pageIndex * pageSize;
        }
    }
}
