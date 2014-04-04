// ***********************************************************************
// Assembly         : 03_Intime.OPC.WebApi
// Author           : Liuyh
// Created          : 03-19-2014 22:06:35
//
// Last Modified By : Liuyh
// Last Modified On : 03-23-2014 18:24:59
// ***********************************************************************
// <copyright file="TransController.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Web.Http;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Models;
using Intime.OPC.Service;
using Intime.OPC.WebApi.Core;

namespace Intime.OPC.WebApi.Controllers
{
    /// <summary>
    ///     账户相关接口
    /// </summary>
    public class TransController : BaseController
    {
        /// <summary>
        ///     The _trans service
        /// </summary>
        private readonly ITransService _transService;

         IEnumService _enumService;
        /// <summary>
        ///     Initializes a new instance of the <see cref="TransController" /> class.
        /// </summary>
        /// <param name="transService">The trans service.</param>
        public TransController(ITransService transService, IEnumService enumService)
        {
            _transService = transService;
            _enumService = enumService;
        }

        /// <summary>
        ///     查询快递单信息
        /// </summary>
        /// <param name="saleNo">销售单编号</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpGet] //ddd
        public IHttpActionResult GetShippingSaleBySaleOrderNo(string saleOrderNo)
        {
            return DoFunction(() => { return _transService.GetShippingSaleBySaleNo(saleOrderNo); }, "查询快递单信息失败");
        }

        /// <summary>
        /// 获得销售单数据
        /// </summary>
        /// <param name="shippingSaleNo">快递单编号</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpGet]
        public IHttpActionResult GetSaleByShippingSaleNo(string shippingSaleNo)
        {
            return DoFunction(() => { return _transService.GetSaleByShippingSaleNo(shippingSaleNo); }, "查询销售单信息失败");
        }

        //ddd
        /// <summary>
        /// Gets the shipping sale.
        /// </summary>
        /// <param name="orderNo">订单号</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpGet]
        public IHttpActionResult GetShippingSale(string orderNo, DateTime startDate, DateTime endDate, int pageIndex, int pageSize = 20)
        {
            
            return DoFunction(() => { return _transService.GetShippingSale(orderNo, startDate, endDate, pageIndex, pageSize); },
                "查询快递单信息失败");
        }

        /// <summary>
        /// 获得发货单数据
        /// </summary>
        /// <param name="orderNo">The order no.</param>
        /// <param name="expressNo">The express no.</param>
        /// <param name="startGoodsOutDate">The start goods out date.</param>
        /// <param name="endGoodsOutDate">The end goods out date.</param>
        /// <param name="outGoodsCode">The out goods code.</param>
        /// <param name="sectionId">The section identifier.</param>
        /// <param name="shippingStatus">The shipping status.</param>
        /// <param name="customerPhone">The customer phone.</param>
        /// <param name="brandId">The brand identifier.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpGet]
        public IHttpActionResult GetShipping(string orderNo,
            string expressNo,
            DateTime startGoodsOutDate,
            DateTime endGoodsOutDate,
            string outGoodsCode,
            int storeId,
            int shippingStatus,
            string customerPhone,
            int brandId,
            int pageIndex, int pageSize = 20
            )
        {
            try
            {
                int userId = GetCurrentUserID();

                var lst = _transService.GetShippingSale(orderNo, expressNo, startGoodsOutDate, endGoodsOutDate,
                    outGoodsCode, storeId, shippingStatus, customerPhone, brandId, pageIndex, pageSize);

                return Ok(lst);
            }
            catch (Exception ex)
            {
                return InternalServerError();
            }
        }

        /// <summary>
        ///     创建发货单
        /// </summary>
        /// <param name="comment">The comment.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost] //dddd
        public IHttpActionResult CreateShippingSale([FromBody]ShippingSaleCreateDto shippingSaleDto)
        {
            return base.DoFunction(() =>
            {
                int userId = GetCurrentUserID();

                return _transService.CreateShippingSale(userId, shippingSaleDto);
            }, "读取快递单备注失败！");
        }

        #region 备注

        /// <summary>
        ///     增加快递单备注
        /// </summary>
        /// <param name="comment">The comment.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult AddShippingSaleComment([FromBody]OPC_ShippingSaleComment comment)
        {
            return base.DoFunction(() =>
            {
                comment.CreateDate = DateTime.Now;
                comment.CreateUser = GetCurrentUserID();
                comment.UpdateDate = comment.CreateDate;
                comment.UpdateUser = comment.CreateUser;

                return _transService.AddShippingSaleComment(comment);
            }, "增加快递单备注失败！");
        }

        /// <summary>
        ///     根据订单编号读取快递单备注
        /// </summary>
        /// <param name="orderNo">The order no.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpGet]
        public IHttpActionResult GetShippingSaleCommentByShippingSaleNo(string shippingSaleNo)
        {
            return DoFunction(() => { return _transService.GetByShippingCommentCode(shippingSaleNo); }, "读取快递单备注失败！");
        }

        #endregion

        [HttpGet]
        public IHttpActionResult GetPayTypeEnums()
        {
            return DoFunction(() => { return _enumService.All("PayType"); }, "读取快递单备注失败！");
        }

    }
}