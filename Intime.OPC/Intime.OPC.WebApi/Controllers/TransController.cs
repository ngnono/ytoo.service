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
using System.Collections.Generic;
using System.Web.Http;
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

        /// <summary>
        ///     Selects the sales.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="orderNo">The order no.</param>
        /// <param name="saleOrderNo">The sale order no.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpGet]
        public IHttpActionResult SelectSales(string startDate, string endDate, string orderNo, string saleOrderNo)
        {
            return Ok(_transService.Select(startDate, endDate, orderNo, saleOrderNo));
        }

        /// <summary>
        ///  增加订单备注
        /// </summary>
        /// <param name="sale">The sale.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult AddOrderComment([FromBody] OPC_OrderComment comment)
        {
           return  DoFunction(() =>
            {

                comment.CreateDate = DateTime.Now;
                comment.CreateUser = this.GetCurrentUserID();
                comment.UpdateDate = comment.CreateDate;
                comment.UpdateUser = comment.CreateUser;
                return _transService.AddOrderComment(comment);

            },"增加订单备注失败");
        }

        /// <summary>
        ///   查询销售单详情
        /// </summary>
        /// <param name="ids">The ids.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpGet]
        public IHttpActionResult SelectSaleDetail(IEnumerable<string> ids)
        {
            return Ok(_transService.SelectSaleDetail(ids));
        }

        /// <summary>
        ///     Selects the mark.
        /// </summary>
        /// <param name="orderNo">The order no.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpGet]
        public IHttpActionResult SelectMark([FromUri] string orderNo)
        {
            IList<OPC_OrderComment> result = _transService.GetRemarksByOrderNo(orderNo);
            return Ok(result);
        }


    }
}