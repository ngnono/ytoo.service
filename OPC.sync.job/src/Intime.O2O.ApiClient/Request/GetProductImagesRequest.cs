﻿using Intime.O2O.ApiClient.Response;

namespace Intime.O2O.ApiClient.Request
{
    public class GetProductImagesRequest : Request<GetProductImagesRequestData, GetProductImagesResponse>
    {
        public override string GetOrderStatusUri()
        {
            return "commodity/queryImages";
        }
    }
}
