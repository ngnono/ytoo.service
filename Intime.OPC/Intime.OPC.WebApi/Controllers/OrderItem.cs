using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Intime.OPC.Domain.Dto.Request;
using Intime.OPC.WebApi.Bindings;
using Intime.OPC.WebApi.Core;
using Intime.OPC.WebApi.Core.Filters;

namespace Intime.OPC.WebApi.Controllers
{
    [APIExceptionFilter]
    [RoutePrefix("api/orderitems")]
    public class OrderItemController:BaseController
    {
        /// <summary>
        /// 获取出库单
        /// </summary>
        /// <returns></returns>
        [Route("")]
        [HttpGet]
        public IHttpActionResult GetList([FromUri]GetOrderItemsRequest request, [UserId]int userId)
        {
            //按创建日期倒序
            throw new NotImplementedException();
        }

    }
}
