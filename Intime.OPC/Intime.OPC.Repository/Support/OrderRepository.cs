// ***********************************************************************
// Assembly         : 01_Intime.OPC.Repository
// Author           : Liuyh
// Created          : 03-25-2014 13:37:53
//
// Last Modified By : Liuyh
// Last Modified On : 03-25-2014 13:38:11
// ***********************************************************************
// <copyright file="OrderRepository.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository.Base;

namespace Intime.OPC.Repository.Support
{
    /// <summary>
    ///     Class OrderRepository.
    /// </summary>
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        #region IOrderRepository Members

        public IList<Order> GetOrder(string orderNo, string orderSource, DateTime dtStart, DateTime dtEnd, int storeId,
            int brandId, int status, string paymentType, string outGoodsType, string shippingContactPhone,
            string expressDeliveryCode, int expressDeliveryCompany)
        {
            using (var db = new YintaiHZhouContext())
            {
                Expression<Func<Order, bool>> query = t => t.CreateDate >= dtStart && t.CreateDate < dtEnd;
                if (!string.IsNullOrWhiteSpace(orderNo))
                {
                    query = query.And(t => t.OrderNo.Contains(orderNo));
                }
                if (!string.IsNullOrWhiteSpace(orderSource))
                {
                    query = query.And(t => t.OrderSource == orderSource);
                }
                if (storeId > 0)
                {
                    query = query.And(t => t.StoreId == storeId);
                }
                if (brandId > 0)
                {
                    query = query.And(t => t.BrandId == brandId);
                }
                if (status > -1)
                {
                    query = query.And(t => t.Status == status);
                }
                if (!string.IsNullOrWhiteSpace(paymentType))
                {
                    query = query.And(t => t.PaymentMethodCode == paymentType);
                }
                if (!string.IsNullOrWhiteSpace(outGoodsType))
                {
                    query = query.And(t => t.PaymentMethodCode == outGoodsType);
                }

                if (!string.IsNullOrWhiteSpace(shippingContactPhone))
                {
                    query = query.And(t => t.ShippingContactPhone == shippingContactPhone);
                }
                if (!string.IsNullOrWhiteSpace(expressDeliveryCode))
                {
                    query = query.And(t => t.ShippingNo == expressDeliveryCode);
                }
                if (expressDeliveryCompany > -1)
                {
                    query = query.And(t => t.ShippingVia == expressDeliveryCompany);
                }

                return db.Orders.Where(query.Compile()).ToList();
            }
        }

        public Order GetOrderByOrderNo(string orderNo)
        {
            return Select(t => t.OrderNo == orderNo).FirstOrDefault();
        }

        public IList<Order> GetOrderByOderNoTime(string orderNo, DateTime starTime, DateTime endTime)
        {
            using (var db = new YintaiHZhouContext())
            {
                Expression<Func<Order, bool>> filter =  t => t.CreateDate >= starTime && t.CreateDate < endTime;
                if (string.IsNullOrWhiteSpace(orderNo))
                {
                   filter= filter.And(t => t.OrderNo.Contains(orderNo));
                }
                return db.Orders.Where(filter).ToList();
            }
        }

        #endregion
    }
}