﻿// ***********************************************************************
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
using Intime.OPC.Domain.Dto;

namespace Intime.OPC.Service
{
    /// <summary>
    /// Interface IOrderService
    /// </summary>
    public interface IOrderService
    {
        /// <summary>
        /// Gets the order information.
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
        IList<OrderDto> GetOrderInfo(string orderNo, string orderSource, DateTime dtStart, DateTime dtEnd,
            string storeId, string brandId, string status, string paymentType, string outGoodsType,
            string shippingContactPhone, string expressDeliveryCode, string expressDeliveryCompany, int userId);
    }
}