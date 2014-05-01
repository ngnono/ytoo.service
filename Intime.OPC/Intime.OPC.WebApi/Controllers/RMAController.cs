using System;
using System.Collections.Generic;
using System.Web.Http;
using Intime.OPC.Domain.Dto.Custom;
using Intime.OPC.Domain.Enums;
using Intime.OPC.Domain.Models;
using Intime.OPC.Service;
using Intime.OPC.WebApi.Bindings;
using Intime.OPC.WebApi.Core;

namespace Intime.OPC.WebApi.Controllers
{
    public class RMAController : BaseController
    {
        private readonly IRmaService _rmaService;
        private readonly ISaleRMAService _saleRmaService;
        private readonly IShippingSaleService _shippingSaleService;

        public RMAController(ISaleRMAService saleRmaService, IRmaService rmaService, IShippingSaleService shippingSaleService)
        {
            _saleRmaService = saleRmaService;
            _rmaService = rmaService;
            _shippingSaleService = shippingSaleService;
        }

        /// <summary>
        ///     客服退货 生成销售退货单
        /// </summary>
        /// <param name="request"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult CreateSaleRMA([FromBody] RMAPost request,[UserId] int uid)
        {
            return DoAction(() =>
            {
                _saleRmaService.UserId = uid;
                _saleRmaService.CreateSaleRMA(uid, request);
            }, "生成销售退货单失败");
        }
        #region 网络自助退货


        /// <summary>
        ///      网络自助退货 生成销售退货单
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult CreateSaleRmaAuto([FromBody] RMAPost request, [UserId] int uid)
        {
            return DoAction(() =>
            {
                _saleRmaService.UserId = uid;
                _saleRmaService.CreateSaleRmaAuto(uid, request);
            }, "生成销售退货单失败");
        }

        #endregion

        /// <summary>
        ///     Gets the order by return goods information.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult GetByReturnGoodsInfo([FromUri] ReturnGoodsInfoRequest request,[UserId] int uid)
        {
            return DoFunction(() =>
            {
                _saleRmaService.UserId = uid;
                return _saleRmaService.GetByReturnGoodsInfo(request);
            }, "查询订单信息失败");
        }

        /// <summary>
        ///     根据订单号获得退货单信息  客服退货查询
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult GetByOrderNo(string orderNo, int pageIndex, int pageSize, [UserId] int uid)
        {
            return
                DoFunction(
                    () =>
                    {
                        _rmaService.UserId = uid;
                        return _rmaService.GetByOrderNo(orderNo, EnumRMAStatus.NoDelivery.AsID(),
                            EnumReturnGoodsStatus.NoProcess.GetDescription(), pageIndex, pageSize);
                    }, "查询订单信息失败");
        }

        /// <summary>
        ///     根据订单号获得退货单信息  包裹签收
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult GetRmaByOrderNo(string orderNo, int pageIndex, int pageSize, [UserId] int uid)
        {
            return
                DoFunction(
                    () =>
                    {
                        _rmaService.UserId = uid;
                        return _rmaService.GetByOrderNo(orderNo, EnumRMAStatus.ShipNoReceive.AsID(),
                            EnumReturnGoodsStatus.NoProcess.GetDescription(), pageIndex, pageSize);
                    }, "查询订单信息失败");
        }

        /// <summary>
        ///     根据退货单号 获得退货明细
        /// </summary>
        /// <param name="rmaNo">The rma no.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult GetRmaDetailByRmaNo(string rmaNo, int pageIndex, int pageSize, [UserId] int uid)
        {
            return DoFunction(() =>
            {
                _rmaService.UserId = uid;
                return _rmaService.GetDetails(rmaNo, pageIndex, pageSize);
            });
        }

        #region 客服退货查询-物流退回

        [HttpPost]
        public IHttpActionResult GetByOrderNoShippingBack(string orderNo, int pageIndex, int pageSize, [UserId] int uid)
        {
            string returnGoodsStatus = "";
            int status = EnumRMAStatus.ShipVerifyNotPass.AsID();
            return DoFunction(() =>
            {
                _rmaService.UserId = uid;
               return  _rmaService.GetByOrderNo(orderNo, status, returnGoodsStatus, pageIndex, pageSize);
            });
        }

        #endregion

        #region 客服退货查询-退货赔偿退回

        [HttpPost]
        public IHttpActionResult GetByOrderNoReturnGoodsCompensation(string orderNo, int pageIndex, int pageSize, [UserId] int uid)
        {
            string returnGoodsStatus = EnumReturnGoodsStatus.CompensateVerifyFailed.GetDescription();
            return
                DoFunction(
                    () =>
                    {
                        _rmaService.UserId = uid; 
                        return _rmaService.GetByOrderNo(orderNo, null, returnGoodsStatus, pageIndex, pageSize);
                    },
                    "查询订单信息失败");
        }

        /// <summary>
        /// 客服同意商品退回
        /// </summary>
        /// <param name="rmaNo">The rma no.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult SetSaleRmaServiceAgreeGoodsBack([FromBody] IEnumerable<string> rmaNos, [UserId] int uid)
        {
            return DoAction(() =>
            {
                _saleRmaService.UserId = uid;
                _shippingSaleService.UserId = uid;
                foreach (var rmaNo in rmaNos)
                {
                    _saleRmaService.SetSaleRmaServiceAgreeGoodsBack(rmaNo);
                    _shippingSaleService.CreateRmaShipping(rmaNo, uid);
                }
            });
        }



        #endregion

        #region 财务赔偿审核

        //finance
        /// <summary>
        ///     查询退货单信息 退货付款确认
        /// </summary>
        /// <param name="rmaNo">The rma no.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult GetByFinaceDto([FromUri] FinaceRequest request, [UserId] int uid)
        {
            return DoFunction(() =>
            {
                _rmaService.UserId = uid;
                //return _saleRmaService.GetByFinaceDto(request);
                return _rmaService.GetByFinaceDto(request);
            }, "查询退货单信息失败");
        }

        [HttpPost]
        public IHttpActionResult FinaceVerify([FromBody] PackageVerifyRequest request, [UserId] int uid)
        {
            return DoAction(() =>
            {
                _saleRmaService.UserId = uid;
                foreach (string rmaNo in request.RmaNos)
                {
                    _saleRmaService.FinaceVerify(rmaNo, request.Pass);
                }
            }, "查询退货单信息失败");
        }

        #endregion

        #region 退货付款确认

        /// <summary>
        ///     查询退货单信息 退货付款确认
        /// </summary>
        /// <param name="rmaNo">The rma no.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult GetRmaByReturnGoodPay([FromUri]ReturnGoodsPayRequest request, [UserId] int uid)
        {
            return DoFunction(() =>
            {
                _saleRmaService.UserId = uid;
                return _saleRmaService.GetByReturnGoodPay(request);
            }, "查询退货单信息失败");
        }

        /// <summary>
        ///     查询退货单 退货付款确认
        /// </summary>
        /// <param name="rmaNo">The rma no.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult GetByRmaNo(string rmaNo, int pageIndex, int pageSize, [UserId] int uid)
        {
            return DoFunction(() =>
            {
                _rmaService.UserId = uid;
                return _rmaService.GetByRmaNo(rmaNo);
            }, "查询退货单信息失败");
        }

        /// <summary>
        ///     退货付款确认
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult CompensateVerify([FromBody] CompensateVerifyRequest request, [UserId] int uid)
        {
            _saleRmaService.UserId = uid;
            return DoAction(() => { _saleRmaService.CompensateVerify(request.RmaNo, request.Money); }, "查询退货单信息失败");
        }

        #endregion

        #region 备注

        /// <summary>
        ///     增加退货单备注
        /// </summary>
        /// <param name="sale">The sale.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult AddSaleRmaComment([FromBody] OPC_SaleRMAComment comment, [UserId] int uid)
        {
            return DoAction(() =>
            {
                _saleRmaService.UserId = uid;
                comment.CreateDate = DateTime.Now;
                comment.CreateUser = uid;
                comment.UpdateDate = comment.CreateDate;
                comment.UpdateUser = comment.CreateUser;
                _saleRmaService.AddComment(comment);
            }, "增加退货单备注失败");
        }

        /// <summary>
        ///     查询退货单备注
        /// </summary>
        /// <param name="rmaNo">The order no.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult GetCommentByRmaNo(string rmaNo)
        {
            return base.DoFunction(() => { return _saleRmaService.GetCommentByRmaNo(rmaNo); }, "查询退货单备注失败！");
        }

        #endregion

        #region 退货单 备注

        /// <summary>
        ///     增加退货单备注
        /// </summary>
        /// <param name="sale">The sale.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult AddRmaComment([FromBody] OPC_RMAComment comment, [UserId] int uid)
        {
            return DoAction(() =>
            {
                _rmaService.UserId = uid;
                comment.CreateDate = DateTime.Now;
                comment.CreateUser = uid;
                comment.UpdateDate = comment.CreateDate;
                comment.UpdateUser = comment.CreateUser;
                _rmaService.AddComment(comment);
            }, "增加退货单备注失败");
        }

        /// <summary>
        ///     查询退货单备注
        /// </summary>
        /// <param name="rmaNo">The order no.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult GetRmaCommentByRmaNo(string rmaNo, [UserId] int uid)
        {
            return base.DoFunction(() =>
            {
                _rmaService.UserId = uid;
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