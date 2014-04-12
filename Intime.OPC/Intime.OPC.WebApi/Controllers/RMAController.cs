using System;
using System.Web.Http;
using Intime.OPC.Domain.Dto.Custom;
using Intime.OPC.Domain.Enums;
using Intime.OPC.Domain.Models;
using Intime.OPC.Service;
using Intime.OPC.WebApi.Core;

namespace Intime.OPC.WebApi.Controllers
{
    public class RMAController : BaseController
    {
        private ISaleRMAService _saleRmaService;
        private IRmaService _rmaService;


        public RMAController(ISaleRMAService saleRmaService, IRmaService rmaService)
        {
            _saleRmaService = saleRmaService;
            _rmaService = rmaService;
        }

        /// <summary>
        ///     客服退货 生成销售退货单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult CreateSaleRMA([FromBody] RMAPost request)
        {
            var user = GetCurrentUserID();
            //todo 客服退货 生成销售退货单
            return DoAction(() => { _saleRmaService.CreateSaleRMA(user,request);}, "生成销售退货单失败");
        }

        /// <summary>
        /// Gets the order by return goods information.
        /// </summary>
        /// <param name="orderNo">The order no.</param>
        /// <returns>IHttpActionResult.</returns>
        //[HttpGet]
        //public IHttpActionResult GetByReturnGoodsInfo([FromBody] ReturnGoodsInfoGet request)
        //{
        //    var userId = GetCurrentUserID();
        //    return DoFunction(() => { return _saleRmaService.GetByReturnGoodsInfo(request); }, "查询订单信息失败");
        //}


        /// <summary>
        ///  根据订单号获得退货单信息  客服退货查询
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetByOrderNo(string orderNo)
        {
            var userId = GetCurrentUserID();
            return DoFunction(() => { return _rmaService.GetByOrderNo(orderNo, EnumRMAStatus.ShipNoReceive, EnumReturnGoodsStatus.NoProcess); }, "查询订单信息失败");
        }

        /// <summary>
        ///  根据订单号获得退货单信息  包裹签收
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetRmaByOrderNo(string orderNo)
        {
            var userId = GetCurrentUserID();
            return DoFunction(() => { return _rmaService.GetByOrderNo(orderNo, EnumRMAStatus.ShipNoReceive, EnumReturnGoodsStatus.NoProcess); }, "查询订单信息失败");
        }

        /// <summary>
        /// 根据退货单号 获得退货明细
        /// </summary>
        /// <param name="rmaNo">The rma no.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpGet]
        public IHttpActionResult GetRmaDetailByRmaNo( string rmaNo)
        {
            var userId = GetCurrentUserID();
            return DoFunction(() => { return _rmaService.GetDetails(rmaNo) ; }, "查询订单信息失败");
        }
        #region 退货付款确认
        
        

        /// <summary>
        ///  查询退货单信息 退货付款确认
        /// </summary>
        /// <param name="rmaNo">The rma no.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpGet]
        public IHttpActionResult GetRmaByReturnGoodPay([FromUri]ReturnGoodsPay request)
        {
            var userId = GetCurrentUserID();
            return DoFunction(() => { return _saleRmaService.GetByReturnGoodPay(request); }, "查询退货单信息失败");
        }

        /// <summary>
        ///  查询退货单 退货付款确认
        /// </summary>
        /// <param name="rmaNo">The rma no.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpGet]
        public IHttpActionResult GetByRmaNo(string rmaNo)
        {
            var userId = GetCurrentUserID();
            return DoFunction(() => { return _saleRmaService.GetByRmaNo(rmaNo); }, "查询退货单信息失败");
        }


        /// <summary>
        ///  退货付款确认
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult CompensateVerify([FromBody]CompensateVerifyRequest request)
        {
            var userId = GetCurrentUserID();
            return DoAction(() => {  _saleRmaService.CompensateVerify(request.RmaNo, request.Money); }, "查询退货单信息失败");
        }

        #endregion
        #region 备注

        /// <summary>
        ///  增加退货单备注
        /// </summary>
        /// <param name="sale">The sale.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult AddSaleRmaComment([FromBody] OPC_SaleRMAComment comment)
        {
            return DoAction(() =>
            {

                comment.CreateDate = DateTime.Now;
                comment.CreateUser = this.GetCurrentUserID();
                comment.UpdateDate = comment.CreateDate;
                comment.UpdateUser = comment.CreateUser;
                 _saleRmaService.AddComment(comment);
            }, "增加退货单备注失败");
        }
        /// <summary>
        /// 查询退货单备注
        /// </summary>
        /// <param name="rmaNo">The order no.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpGet]
        public IHttpActionResult GetCommentByRmaNo(string rmaNo)
        {
            return base.DoFunction(() =>
            {
                return _saleRmaService.GetCommentByRmaNo(rmaNo);

            }, "查询退货单备注失败！");
        }


        #endregion

        #region 退货单 备注

        /// <summary>
        ///  增加退货单备注
        /// </summary>
        /// <param name="sale">The sale.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult AddRmaComment([FromBody] OPC_RMAComment comment)
        {
            return DoAction(() =>
            {

                comment.CreateDate = DateTime.Now;
                comment.CreateUser = this.GetCurrentUserID();
                comment.UpdateDate = comment.CreateDate;
                comment.UpdateUser = comment.CreateUser;
                _rmaService.AddComment(comment);
            }, "增加退货单备注失败");
        }
        /// <summary>
        /// 查询退货单备注
        /// </summary>
        /// <param name="rmaNo">The order no.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpGet]
        public IHttpActionResult GetRmaCommentByRmaNo(string rmaNo)
        {
            return base.DoFunction(() =>
            {
                return _rmaService.GetCommentByRmaNo(rmaNo);

            }, "查询退货单备注失败！");
        }


        #endregion
    }

    public class CompensateVerifyRequest
    {
        public string RmaNo { get; set; }
        public decimal Money { get; set; }
    }
}