using System;
using System.Web.Http;
using Intime.OPC.Domain.Dto.Custom;
using Intime.OPC.Service;
using Intime.OPC.WebApi.Bindings;
using Intime.OPC.WebApi.Core;

namespace Intime.OPC.WebApi.Controllers
{
    public class CustomController : BaseController
    {
        private IOrderService _orderService;

        public CustomController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public IHttpActionResult GetOrder([FromBody] ReturnGoodsGet request)
        {
           
            return DoFunction(() =>
            {
                var userId = GetCurrentUserID();
                int brandid = request.BandId.HasValue ? request.BandId.Value : -1;
                return  _orderService.GetOrder(request.OrderNo, "", request.StartDate, request.EndDate, -1, brandid, -1,
                    request.PayType, "", request.Telephone, "", -1, userId, 1, 10000).Result;
            }, "查询订单失败");
        }
    }
}