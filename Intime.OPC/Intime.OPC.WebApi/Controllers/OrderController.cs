// ***********************************************************************
// Assembly         : 03_Intime.OPC.WebApi
// Author           : Liuyh
// Created          : 03-25-2014 13:43:29
//
// Last Modified By : Liuyh
// Last Modified On : 03-26-2014 00:26:13
// ***********************************************************************
// <copyright file="OrderController.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Web;
using System.Web.Http;
using Intime.OPC.Domain.Models;
using Intime.OPC.Service;
using Intime.OPC.WebApi.Bindings;
using Intime.OPC.WebApi.Core;

namespace Intime.OPC.WebApi.Controllers
{
    /// <summary>
    ///     Class OrderController.
    /// </summary>
    public class OrderController : BaseController
    {
        /// <summary>
        ///     The _order service
        /// </summary>
        private readonly IOrderService _orderService;

        /// <summary>
        ///     Initializes a new instance of the <see cref="OrderController" /> class.
        /// </summary>
        /// <param name="orderService">The order service.</param>
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        ///     获得未提货的数据
        /// </summary>
        /// <param name="orderNo">The order no.</param>
        /// <param name="orderSource">The order source.</param>
        /// <param name="startCreateDate">The start create date.</param>
        /// <param name="endCreateDate">The end create date.</param>
        /// <param name="storeId">The store identifier.</param>
        /// <param name="brandId">The brand identifier.</param>
        /// <param name="status">The status.</param>
        /// <param name="paymentType">Type of the payment.</param>
        /// <param name="outGoodsType">Type of the out goods.</param>
        /// <param name="shippingContactPhone">The shipping contact phone.</param>
        /// <param name="expressDeliveryCode">The express delivery code.</param>
        /// <param name="expressDeliveryCompany">The express delivery company.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpGet]
        public IHttpActionResult GetOrder(string orderNo,
            string orderSource,
            DateTime startCreateDate,
            DateTime endCreateDate,
            int storeId,
            int brandId,
            int status,
            string paymentType,
            string outGoodsType,
            string shippingContactPhone,
            string expressDeliveryCode,
            int expressDeliveryCompany,
            [UserId] int userId)
        {
            try
            {
                return Ok(_orderService.GetOrder(orderNo, orderSource, startCreateDate, endCreateDate, storeId, brandId,
                    status, paymentType, outGoodsType, shippingContactPhone, expressDeliveryCode, expressDeliveryCompany,
                    userId));
            }
            catch (Exception ex)
            {
                return InternalServerError();
            }
        }

        [HttpGet]
        public IHttpActionResult GetOrderByOderNo(string orderNo)
        {
          return  Ok(  _orderService.GetOrderByOrderNo(orderNo));
        }
    }
}