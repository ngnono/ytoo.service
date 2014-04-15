using Intime.O2O.ApiClient.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intime.O2O.ApiClient.Request
{
    public class GetOrderStatusRequest : Request<GetBrandByIdRequestData, GetOrderStatusResponse>
    {
        public override string GetOrderStatusUri()
        {
            return "production/queryorderdetail";
        }
    }
}
