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

        /// <summary>
        /// 物流确认收货
        /// </summary>
        /// <param name="rmaNos">The rma nos.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult ShippingReceiveGoods([FromBody]IEnumerable<string> rmaNos)
        {
            return DoAction(() =>
            {
                var userId = GetCurrentUserID();
                foreach (var rmaNo in rmaNos)
                {
                    _saleRmaService.ShippingReceiveGoods(rmaNo);
                }

            }, "查询订单失败");
        }

        #region 退货包裹审核

        /// <summary>
        ///  查询退货单信息
        /// </summary>
        /// <param name="rmaNo">The rma no.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpGet]
        public IHttpActionResult GetRmaPackVerifyByPack([FromUri]PackageReceiveDto request)
        {
            var userId = GetCurrentUserID();
            return DoFunction(() => { return _rmaService.GetAllPackVerify(request); }, "查询退货单信息失败");
        }

       


        /// <summary>
        ///  退货包裹审核
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult PackageVerify([FromBody]PackageVerifyRequest request)
        {
            var userId = GetCurrentUserID();
            return DoAction(() =>
            {
                foreach (var rmaNo in request.RmaNos)
                {
                    _saleRmaService.PackageVerify(rmaNo, request.Pass);
                }
                
            }, "查询退货单信息失败");
        }
        #endregion
    }
}