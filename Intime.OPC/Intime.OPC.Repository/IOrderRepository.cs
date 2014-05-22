// ***********************************************************************
// Assembly         : 01_Intime.OPC.Repository
// Author           : Liuyh
// Created          : 03-25-2014 13:36:49
//
// Last Modified By : Liuyh
// Last Modified On : 03-25-2014 13:37:17
// ***********************************************************************
// <copyright file="IOrderRepository.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using Intime.OPC.Domain;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Dto.Custom;
using Intime.OPC.Domain.Enums;
using Intime.OPC.Domain.Models;
using Intime.OPC.Domain.Partials.Models;

namespace Intime.OPC.Repository
{
    /// <summary>
    ///     Interface IOrderRepository
    /// </summary>
    public interface IOrderRepository : IRepository<Order>
    {
        PageResult<Order> GetOrder(string orderNo, string orderSource, DateTime dtStart, DateTime dtEnd, int storeId,
            int brandId, int status, string paymentType, string outGoodsType, string shippingContactPhone,
            string expressDeliveryCode, int expressDeliveryCompany, int pageIndex, int pageSize = 20);

        Order GetOrderByOrderNo(string orderNo);

        PageResult<Order> GetOrderByOderNoTime(string orderNo, DateTime starTime, DateTime endTime, int pageIndex, int pageSize);
        PageResult<Order> GetOrderByShippingNo(string shippingNo, int pageIndex, int pageSize);

        PageResult<Order> GetByReturnGoodsInfo(Domain.Dto.Custom.ReturnGoodsInfoRequest request);

        PageResult<Order> GetBySaleRma(ReturnGoodsInfoRequest request, int? rmaStatus, EnumReturnGoodsStatus status);
        PageResult<Order> GetByOutOfStockNotify(OutOfStockNotifyRequest request, int orderstatus);

        OrderModel GetItemByOrderNo(string orderno);
    }
}