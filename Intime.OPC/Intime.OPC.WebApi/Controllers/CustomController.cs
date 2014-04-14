using System.Collections.Generic;
using System.Web.Http;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Dto.Custom;
using Intime.OPC.Service;
using Intime.OPC.WebApi.Core;

namespace Intime.OPC.WebApi.Controllers
{
    public class CustomController : BaseController
    {
        private readonly IRmaService _rmaService;
        private readonly ISaleRMAService _saleRmaService;
        private readonly IShippingSaleService _shippingSaleService;
        private IOrderService _orderService;

        public CustomController(IOrderService orderService, ISaleRMAService saleRmaService, IRmaService rmaService,
            IShippingSaleService shippingSaleService)
        {
            _orderService = orderService;
            _saleRmaService = saleRmaService;
            _rmaService = rmaService;
            _shippingSaleService = shippingSaleService;
        }

        [HttpGet]
        public IHttpActionResult GetOrder([FromUri] ReturnGoodsRequest request)
        {
            return DoFunction(() =>
            {
                int brandid = request.BandId.HasValue ? request.BandId.Value : -1;

                return _saleRmaService.GetByReturnGoods(request, UserID);
            }, "查询订单失败");
        }

        #region 客服退货查询-物流退回

        /// <summary>
        ///     客服同意退货
        /// </summary>
        /// <param name="rmaNos">The rma nos.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult AgreeReturnGoods([FromBody] IEnumerable<string> rmaNos)
        {
            return DoAction(() =>
            {
                int userId = GetCurrentUserID();
                foreach (string rmaNo in rmaNos)
                {
                    _saleRmaService.AgreeReturnGoods(rmaNo);
                }
            }, "查询订单失败");
        }

        #endregion

        /// <summary>
        ///     物流确认收货
        /// </summary>
        /// <param name="rmaNos">The rma nos.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult ShippingReceiveGoods([FromBody] IEnumerable<string> rmaNos)
        {
            return DoAction(() =>
            {
                int userId = GetCurrentUserID();
                foreach (string rmaNo in rmaNos)
                {
                    _saleRmaService.ShippingReceiveGoods(rmaNo);
                }
            }, "查询订单失败");
        }

        #region 退货包裹审核

        /// <summary>
        ///     查询退货单信息
        /// </summary>
        /// <param name="rmaNo">The rma no.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpGet]
        public IHttpActionResult GetRmaPackVerifyByPack([FromUri] PackageReceiveRequest request)
        {
            return DoFunction(() =>
            {
                _rmaService.UserId = UserID;
                return _rmaService.GetAllPackVerify(request);
            }, "查询退货单信息失败");
        }

        /// <summary>
        ///     退货包裹审核
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult PackageVerify([FromBody] PackageVerifyRequest request)
        {
            return DoAction(() =>
            {
                foreach (string rmaNo in request.RmaNos)
                {
                    _saleRmaService.PackageVerify(rmaNo, request.Pass);
                    //_shippingSaleService.CreateRmaShipping(rmaNo, UserID);
                }
            }, "查询退货单信息失败");
        }

        #endregion

        #region 包裹退回-打印快递单

        [HttpGet]
        public IHttpActionResult GetRmaByPackPrintPress(string rmaNo)
        {
            return DoFunction(() =>
            {
                _rmaService.UserId = UserID;
                return _rmaService.GetByRmaNo(rmaNo);
            }, "查询退货单信息失败");
        }

        /// <summary>
        ///     包裹退回-打印快递单 设定快递公司
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult UpdateShipRmaVia([FromBody] RmaExpressSaveDto request)
        {
            return DoAction(() => { _shippingSaleService.UpdateRmaShipping(request); }, "查询退货单信息失败");
        }

        /// <summary>
        /// 打印快递单
        /// </summary>
        /// <param name="shippingCode">The shipping code.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult PintRmaShipping(string shippingCode)
        {
            return DoAction(() => _shippingSaleService.PintRmaShipping(shippingCode), "查询退货单信息失败");
        }

        /// <summary>
        /// 打印完成
        /// </summary>
        /// <param name="shippingCode">The shipping code.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult PintRmaShippingOver(string shippingCode)
        {
            return DoAction(() => _shippingSaleService.PintRmaShippingOver(shippingCode), "查询退货单信息失败");
        }

        [HttpGet]
        public IHttpActionResult GetRmaShippingByPackPrintPress([FromUri] RmaExpressRequest request)
        {
            return DoFunction(() =>
            {
                _shippingSaleService.UserId = UserID;
                return _shippingSaleService.GetRmaByPackPrintPress(request);
            }, "查询退货单信息失败");
        }
        #endregion

        #region 包裹退回-快递交接
        /// <summary>
        /// 查询快递
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>IHttpActionResult.</returns>
         [HttpGet]
        public IHttpActionResult GetRmaShippingPrintedByPack([FromUri] RmaExpressRequest request)
        {
            return DoFunction(() =>
            {
                _shippingSaleService.UserId = UserID;
                return _shippingSaleService.GetRmaShippingPrintedByPack(request);
            }, "查询退货单信息失败");
        }

         /// <summary>
         /// 打印完成快递交接
         /// </summary>
         /// <param name="shippingCode">The shipping code.</param>
         /// <returns>IHttpActionResult.</returns>
         [HttpPost]
         public IHttpActionResult PintRmaShippingOverConnect(string shippingCode)
         {

             return DoAction(() => _shippingSaleService.PintRmaShippingOverConnect(shippingCode), "查询退货单信息失败");
         }
        #endregion


        #region 导购退货收货查询
         /// <summary>
         /// 查询退货信息
         /// </summary>
         /// <param name="request">The request.</param>
         /// <returns>IHttpActionResult.</returns>
          [HttpGet]
        public IHttpActionResult GetRmaByShoppingGuide([FromUri] ShoppingGuideRequest request)
        {
            return DoFunction(() =>
            {
                _rmaService.UserId = UserID;
                return _rmaService.GetRmaByShoppingGuide(request);
            }, "查询退货单信息失败");
        }
        
        #endregion


          #region 已完成退货单查询
          /// <summary>
          ///已完成退货单查询
          /// </summary>
          /// <param name="request">The request.</param>
          /// <returns>IHttpActionResult.</returns>
          [HttpGet]
          public IHttpActionResult GetRmaByAllOver([FromUri] ShoppingGuideRequest request)
          {
              return DoFunction(() =>
              {
                  _rmaService.UserId = UserID;
                  return _rmaService.GetRmaByAllOver(request);
              }, "查询退货单信息失败");
          }

          #endregion
    }

   
}