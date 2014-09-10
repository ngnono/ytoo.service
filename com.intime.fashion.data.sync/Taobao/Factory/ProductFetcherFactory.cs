using com.intime.fashion.data.sync.Taobao.Fetcher;
using com.intime.fashion.data.sync.Taobao.Request;
using System;

namespace com.intime.fashion.data.sync.Taobao.Factory
{
    public static class ProductFetcherFactory
    {
        public static IProductFetcher Create(FetchRequest request)
        {
            switch (request.Channel.ToUpper())
            {
                case "YINTAI":
                    return null;
                case "IMS":
                    return new IMSProductFetcher();
                default:
                    throw new NotSupportedException(request.Channel);
            }
        }
    }
}
