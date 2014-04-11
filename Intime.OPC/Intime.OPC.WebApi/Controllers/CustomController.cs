using System;
using System.Collections.Generic;
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
        private IRmaService _rmaService;

        public CustomController(IOrderService orderService, ISaleRMAService saleRmaService, IRmaService rmaService)
        {
            _orderService = orderService;
            _saleRmaService = saleRmaService;
            _rmaService = rmaService;
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

        /// <summary>
        /// 客服同意退货
        /// </summary>
        /// <param name="rmaNos">The rma nos.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult AgreeReturnGoods([FromBody]IEnumerable<string> rmaNos)
        {
            return DoAction(() =>
            {
                var userId = GetCurrentUserID();
                foreach (var rmaNo in rmaNos)
                {
                    _saleRmaService.AgreeReturnGoods(rmaNo);
                }
               
            }, "查询订单失败");
        }
    }
}