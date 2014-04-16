using Intime.O2O.ApiClient.Response;
using System;

namespace Intime.O2O.ApiClient.Request
{
    public class GetProductsRequest : Request<GetProductsRequestData, GetProductsResponse>
    {
        public override string GetOrderStatusUri()
        {
            return "commodity/queryCommoditys";
        }
    }
}
