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
using Intime.OPC.Domain.Models;

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

        IList<Order> GetOrderByOderNoTime(string orderNo, DateTime starTime, DateTime endTime);
        IList<Order> GetOrderByShippingNo(string shippingNo);

        IList<Order> GetByReturnGoodsInfo(Domain.Dto.Custom.ReturnGoodsInfoGet request);
    }
}