// ***********************************************************************
// Assembly         : 02_Intime.OPC.Service
// Author           : Liuyh
// Created          : 03-26-2014 00:22:08
//
// Last Modified By : Liuyh
// Last Modified On : 03-26-2014 00:26:40
// ***********************************************************************
// <copyright file="IOrderService.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using Intime.OPC.Domain;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Dto.Custom;
using Intime.OPC.Domain.Dto.Financial;
using Intime.OPC.Domain.Models;

namespace Intime.OPC.Service
{
    /// <summary>
    ///     Interface IOrderService
    /// </summary>
    public interface IOrderService : IService
    {
        /// <summary>
        ///     Gets the order information.
        /// </summary>
        /// <param name="orderNo">The order no.</param>
        /// <param name="orderSource">The order source.</param>
        /// <param name="dtStart">The dt start.</param>
        /// <param name="dtEnd">The dt end.</param>
        /// <param name="storeId">The store identifier.</param>
        /// <param name="brandId">The brand identifier.</param>
        /// <param name="status">The status.</param>
        /// <param name="paymentType">Type of the payment.</param>
        /// <param name="outGoodsType">Type of the out goods.</param>
        /// <param name="shippingContactPhone">The shipping contact phone.</param>
        /// <param name="expressDeliveryCode">The express delivery code.</param>
        /// <param name="expressDeliveryCompany">The express delivery company.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>IList{OrderDto}.</returns>
        PageResult<OrderDto> GetOrder(string orderNo, string orderSource, DateTime dtStart, DateTime dtEnd, int storeId,
            int brandId, int status, string paymentType, string outGoodsType, string shippingContactPhone,
            string expressDeliveryCode, int expressDeliveryCompany, int userId, int pageIndex, int pageSize = 20);

        OrderDto GetOrderByOrderNo(string orderNo);

        IList<OPC_OrderComment> GetCommentByOderNo(string orderNo);

        /// <summary>
        /// 增加订单日志
        /// </summary>
        /// <param name="comment">The comment.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool AddOrderComment(OPC_OrderComment comment);

        /// <summary>
        /// 根据时间、订单号获取订单
        /// </summary>
        /// <param name="orderNo">The order no.</param>
        /// <param name="starTime">The star time.</param>
        /// <param name="endTime">The end time.</param>
        /// <returns>IList{OrderDto}.</returns>
        PageResult<OrderDto> GetOrderByOderNoTime(string orderNo, DateTime starTime, DateTime endTime, int pageIndex, int pageSize);


        PageResult<OrderItemDto> GetOrderItems(string orderNo, int pageIndex, int pageSize);
        PageResult<OrderDto> GetOrderByShippingNo(string shippingNo, int pageIndex, int pageSize);

        //IList<OrderDto> GetOrderByReturnGoodsInfo(ReturnGoodsInfoGet request);

        PageResult<OrderDto> GetByReturnGoodsInfo(ReturnGoodsInfoRequest request);

        PageResult<OrderDto> GetShippingBackByReturnGoodsInfo(ReturnGoodsInfoRequest request);

        PageResult<OrderDto> GetSaleRmaByReturnGoodsCompensate(ReturnGoodsInfoRequest request);
        PageResult<OrderDto> GetOrderByOutOfStockNotify(OutOfStockNotifyRequest request);
        PageResult<OrderDto> GetOrderOfVoid(OutOfStockNotifyRequest request);
        SaleDetailStatListDto WebSiteStatSaleDetail(SearchStatRequest request);
        ReturnGoodsStatListDto WebSiteStatReturnDetail(SearchStatRequest request);
        CashierList WebSiteCashier(SearchCashierRequest request);
    }
}