using Intime.O2O.ApiClient.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intime.O2O.ApiClient.Request
{

    public class GetOrderStatusByIdRequest : Request<GetOrderStatusByIdRequestData, GetOrderStatusByIdResponse>
    {
        public override string GetResourceUri()
        {
            return "production/queryorderdetail";
        }
    }
}
