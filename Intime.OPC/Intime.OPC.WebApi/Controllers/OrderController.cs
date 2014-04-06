﻿// ***********************************************************************
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
using System.Web.UI.WebControls;
using Intime.OPC.Domain;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository;
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
            int expressDeliveryCompany, int pageIndex, int pageSize = 20
            )
        {
            try
            {
                int userId = GetCurrentUserID();
                var lst = _orderService.GetOrder(orderNo, orderSource, startCreateDate, endCreateDate, storeId, brandId,
                    status, paymentType, outGoodsType, shippingContactPhone, expressDeliveryCode, expressDeliveryCompany,
                    userId,pageIndex,pageSize);
                
                return Ok(lst);
            }
            catch (Exception ex)
            {
                return InternalServerError();
            }
        }



        


        /// <summary>
        /// Gets the order by oder no.
        /// </summary>
        /// <param name="orderNo">The order no.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpGet]
        public IHttpActionResult GetOrderByOderNo(string orderNo)
        {
          return  Ok(  _orderService.GetOrderByOrderNo(orderNo));
        }

        /// <summary>
        /// 查询订单 根据时间，和订单编码
        /// </summary>
        /// <param name="orderNo">The order no.</param>
        /// <param name="starTime">The star time.</param>
        /// <param name="endTime">The end time.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpGet]
        public IHttpActionResult GetOrderByOderNoTime(string orderNo,DateTime starTime,DateTime endTime)
        {
            return Ok(_orderService.GetOrderByOderNoTime(orderNo,starTime,endTime));
            
        }
       

        #region 备注
        
        /// <summary>
        ///  增加订单备注
        /// </summary>
        /// <param name="sale">The sale.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult AddOrderComment([FromBody] OPC_OrderComment comment)
        {
            return DoFunction(() =>
             {

                 comment.CreateDate = DateTime.Now;
                 comment.CreateUser = this.GetCurrentUserID();
                 comment.UpdateDate = comment.CreateDate;
                 comment.UpdateUser = comment.CreateUser;
                 return _orderService.AddOrderComment(comment);

             }, "增加订单备注失败");
        }
        /// <summary>
        /// 根据订单编号读取订单备注
        /// </summary>
        /// <param name="orderNo">The order no.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpGet]
        public IHttpActionResult GetCommentByOderNo(string orderNo)
        {
            return base.DoFunction(() =>
            {
                return _orderService.GetCommentByOderNo(orderNo);

            }, "读取订单备注失败！");
        }


        #endregion
    }
}