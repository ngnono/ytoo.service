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

using System.Collections.Generic;
using Intime.OPC.Domain.Models;

namespace Intime.OPC.Repository
{
    /// <summary>
    /// Interface IOrderRepository
    /// </summary>
    public interface IOrderRepository : IRepository<Order>
    {
        IList<Order> GetOrder(string orderNo, string orderSource, System.DateTime dtStart, System.DateTime dtEnd, int storeId, int brandId, int status, string paymentType, string outGoodsType, string shippingContactPhone, string expressDeliveryCode, int expressDeliveryCompany);
    }
}