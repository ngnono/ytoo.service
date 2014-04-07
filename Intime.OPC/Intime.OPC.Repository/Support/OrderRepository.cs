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
using Intime.OPC.Domain;
using Intime.OPC.Domain.Dto;
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

        public PageResult<Order> GetOrder(string orderNo, string orderSource, DateTime dtStart, DateTime dtEnd, int storeId,
            int brandId, int status, string paymentType, string outGoodsType, string shippingContactPhone,
            string expressDeliveryCode, int expressDeliveryCompany, int pageIndex, int pageSize = 20)
        {
            using (var db = new YintaiHZhouContext())
            {
                
                var query = db.Orders.Where(t => t.CreateDate >= dtStart && t.CreateDate < dtEnd);
                if (!string.IsNullOrWhiteSpace(orderNo))
                {
                    query = query.Where(t => t.OrderNo.Contains(orderNo));
                }
                if (!string.IsNullOrWhiteSpace(orderSource))
                {
                    query = query.Where(t => t.OrderSource == orderSource);
                }
                if (storeId > 0)
                {
                    query = query.Where(t => t.StoreId == storeId);
                }
                if (brandId > 0)
                {
                    query = query.Where(t => t.BrandId == brandId);
                }
                if (status > -1)
                {
                    query = query.Where(t => t.Status == status);
                }
                if (!string.IsNullOrWhiteSpace(paymentType))
                {
                    query = query.Where(t => t.PaymentMethodCode == paymentType);
                }
                if (!string.IsNullOrWhiteSpace(outGoodsType))
                {
                    query = query.Where(t => t.PaymentMethodCode == outGoodsType);
                }

                if (!string.IsNullOrWhiteSpace(shippingContactPhone))
                {
                    query = query.Where(t => t.ShippingContactPhone == shippingContactPhone);
                }
                if (!string.IsNullOrWhiteSpace(expressDeliveryCode))
                {
                    query = query.Where(t => t.ShippingNo == expressDeliveryCode);
                }
                if (expressDeliveryCompany > -1)
                {
                    query = query.Where(t => t.ShippingVia == expressDeliveryCompany);
                }
                query = query.OrderByDescending(t => t.CreateDate);
                return query.ToPageResult(pageIndex, pageSize);
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
                var filter = db.Orders.Where( t => t.CreateDate >= starTime && t.CreateDate < endTime);
               
                if (string.IsNullOrWhiteSpace(orderNo))
                {
                   filter= filter.Where(t => t.OrderNo.Contains(orderNo));
                }
                return filter.ToList();
            }
        }

        public PageResult<Order> GetOrderByShippingNo(string shippingNo, int pageIndex, int pageSize)
        {
            using (var db = new YintaiHZhouContext())
            {
                var filter = db.ShippingSales.Where(t => t.ShippingCode == shippingNo).Join(db.Orders,t=>t.OrderNo,o=>o.OrderNo,(t,o)=>o);

                filter.OrderByDescending(t => t.CreateDate);
                return filter.ToPageResult(pageIndex, pageSize);
            }
        }

        #endregion
    }
}