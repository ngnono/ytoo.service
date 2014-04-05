using System;
using System.Web.Http;
using Intime.OPC.Domain.Dto.Custom;
using Intime.OPC.WebApi.Bindings;
using Intime.OPC.WebApi.Core;

namespace Intime.OPC.WebApi.Controllers
{
    public class CustomController : BaseController
    {
        [HttpGet]
        public IHttpActionResult GetOrder([FromBody] ReturnGoodsGet request)
        {
            // todo 客服查询 读取订单
            return DoFunction(() => { return null; }, "查询订单失败");
        }
    }
}