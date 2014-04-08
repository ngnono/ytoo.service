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
        private ISaleRMAService _saleRmaService;

        public CustomController(IOrderService orderService, ISaleRMAService saleRmaService)
        {
            _orderService = orderService;
            _saleRmaService = saleRmaService;
        }

        [HttpGet]
        public IHttpActionResult GetOrder([FromUri] ReturnGoodsGet request)
        {
            return DoFunction(() =>
            {
                var userId = GetCurrentUserID();

                return _saleRmaService.GetByReturnGoods(request);
            }, "查询订单失败");
        }

      
    }
}