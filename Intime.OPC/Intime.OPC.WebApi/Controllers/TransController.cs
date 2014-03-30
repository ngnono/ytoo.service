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

        /// <summary>
        ///     Initializes a new instance of the <see cref="TransController" /> class.
        /// </summary>
        /// <param name="transService">The trans service.</param>
        public TransController(ITransService transService)
        {
            _transService = transService;
        }

        ///// <summary>
        /////     Selects the sales.
        ///// </summary>
        ///// <param name="startDate">The start date.</param>
        ///// <param name="endDate">The end date.</param>
        ///// <param name="orderNo">The order no.</param>
        ///// <param name="saleOrderNo">The sale order no.</param>
        ///// <returns>IHttpActionResult.</returns>
        //[HttpGet]
        //public IHttpActionResult SelectSales(string startDate, string endDate, string orderNo, string saleOrderNo)
        //{
        //    return Ok(_transService.Select(startDate, endDate, orderNo, saleOrderNo));
        //}

        ///// <summary>
        /////   查询销售单详情
        ///// </summary>
        ///// <param name="ids">The ids.</param>
        ///// <returns>IHttpActionResult.</returns>
        //[HttpGet]
        //public IHttpActionResult SelectSaleDetail(IEnumerable<string> ids)
        //{
        //    return Ok(_transService.SelectSaleDetail(ids));
        //}

        /// <summary>
        ///     查询快递单信息
        /// </summary>
        /// <param name="saleNo">销售单编号</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpGet]
        public IHttpActionResult GetShippingSaleByOrderNo(string saleNo)
        {
            return DoFunction(() => { return _transService.GetShippingSaleBySaleNo(saleNo); }, "查询快递单信息失败");
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

        [HttpGet]
        public IHttpActionResult GetShippingSale(string shippingCode, DateTime startDate, DateTime endDate)
        {
            
            return DoFunction(() => { return _transService.GetShippingSale(shippingCode, startDate, endDate); },
                "查询快递单信息失败");
        }

        /// <summary>
        ///     创建快递单
        /// </summary>
        /// <param name="comment">The comment.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult CreateShippingSale(ShippingSaleCreateDto shippingSaleDto)
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
        public IHttpActionResult AddShippingSaleComment(OPC_ShippingSaleComment comment)
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

        #region 销售单提货记录查询
        /// <summary>
        /// 查询已发货的数据
        /// </summary>
        /// <param name="orderCode">The order code.</param>
        /// <param name="saleOrderNo">The sale order no.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpGet]
        public IHttpActionResult GetSaleOrderPickup(string orderCode, string saleOrderNo, DateTime startDate, DateTime endDate)
        {
            
            return DoFunction(() =>
            {
                return _transService.GetSaleOrderPickup(orderCode,saleOrderNo, startDate, endDate);
            },
                "查询快递单信息失败");
        }
        #endregion
    }
}