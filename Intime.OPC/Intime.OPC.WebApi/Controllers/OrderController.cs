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
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using System.Web.UI.WebControls;
using Intime.OPC.Domain;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Dto.Custom;
using Intime.OPC.Domain.Dto.Financial;
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


        [HttpGet]
        public IHttpActionResult GetOrderItemsByOrderNo(string orderNo, int pageIndex, int pageSize)
        {
            //todo 查询订单明细 未实现
            return DoFunction(() =>
            {
                return _orderService.GetOrderItems(orderNo,pageIndex,pageSize);
                return null;
            }, "读取订单明细失败");

        }

        [HttpGet]
        public IHttpActionResult GetOrderByShippingSaleNo(string shippingNo, int pageIndex, int pageSize)
        {
            return DoFunction(() =>
            {
                return _orderService.GetOrderByShippingNo(shippingNo,pageIndex,pageSize);

            }, "通过快递单查询订单失败");
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
        public IHttpActionResult GetOrderByOderNoTime(string orderNo, DateTime starTime, DateTime endTime, int pageIndex, int pageSize)
        {
            return Ok(_orderService.GetOrderByOderNoTime(orderNo,starTime,endTime,pageIndex,pageSize));
            
        }

        #region 客服退货查询-退货信息
        
       

        [HttpGet]
        public IHttpActionResult GetByReturnGoodsInfo([FromUri] ReturnGoodsInfoRequest request)
        {
            var userId = GetCurrentUserID();
            return DoFunction(() => { return _orderService.GetByReturnGoodsInfo(request); }, "查询订单信息失败");
        }

        #endregion

        #region 客服退货查询-物流退回



        [HttpGet]
        public IHttpActionResult GetShippingBackByReturnGoodsInfo([FromUri] ReturnGoodsInfoRequest request)
        {
            var userId = GetCurrentUserID();
            return DoFunction(() => { return _orderService.GetShippingBackByReturnGoodsInfo(request); }, "查询订单信息失败");
        }

        #endregion

        #region 客服退货查询-退货赔偿退回
        [HttpGet]
        public IHttpActionResult GetByReturnGoodsCompensate([FromUri] ReturnGoodsInfoRequest request)
        {
            var userId = GetCurrentUserID();
            return DoFunction(() => _orderService.GetSaleRmaByReturnGoodsCompensate(request));
            //return DoFunction(() => _saleRmaService.GetByReturnGoodsCompensate(request));
            //return _orderService.GetShippingBackByReturnGoodsInfo(request);
        }

        //ReturnGoodsInfoRequest
        #endregion

        #region 缺货提醒-缺货订单

        /// <summary>
        /// Gets the order by oder no time.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpGet]
        public IHttpActionResult GetOrderByOutOfStockNotify([FromBody] OutOfStockNotifyRequest request)
        {
            return DoFunction(() => _orderService.GetOrderByOutOfStockNotify(request));
        }

        [HttpPost]
        public IHttpActionResult SetSaleOrderVoid([FromBody] IEnumerable<string> saleOrderNos)
        {
            //todo 缺货提醒-缺货订单 取消销售单
            return DoAction(() => { });
        }

        #endregion

        #region 缺货提醒-已取消订单

        /// <summary>
        /// Gets the order by oder no time.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpGet]
        public IHttpActionResult GetOrderOfVoid([FromBody] OutOfStockNotifyRequest request)
        {
            return DoFunction(() => _orderService.GetOrderOfVoid(request));
        }

        #endregion


        #region 网站销售明细统计
        //SearchStatRequest

        [HttpGet]
        public IHttpActionResult WebSiteStatSaleDetail([FromBody] SearchStatRequest request)
        {
            return DoFunction(() => _orderService.WebSiteStatSaleDetail(request));
        }

        #endregion

        #region 网站退货明细统计
        //SearchStatRequest

        [HttpGet]
        public IHttpActionResult WebSiteStatReturnDetail([FromBody] SearchStatRequest request)
        {
            return DoFunction(() => _orderService.WebSiteStatReturnDetail(request));
        }

        #endregion

        #region  网上收银流水对账查询
        //SearchStatRequest

        [HttpGet]
        public IHttpActionResult WebSiteCashier([FromBody] SearchCashierRequest request)
        {
            return DoFunction(() => _orderService.WebSiteCashier(request));
        }

        #endregion

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