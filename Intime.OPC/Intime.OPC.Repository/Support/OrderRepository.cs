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
using Intime.OPC.Domain.Dto.Custom;
using Intime.OPC.Domain.Enums;
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

        public PageResult<Order> GetOrderByOderNoTime(string orderNo, DateTime starTime, DateTime endTime, int pageIndex, int pageSize)
        {
            using (var db = new YintaiHZhouContext())
            {
                var filter = db.Orders.Where( t => t.CreateDate >= starTime && t.CreateDate < endTime);
               
                if (string.IsNullOrWhiteSpace(orderNo))
                {
                   filter= filter.Where(t => t.OrderNo.Contains(orderNo));
                }
                filter = filter.OrderByDescending(t => t.CreateDate);
                return filter.ToPageResult(pageIndex,pageSize);
            }
        }

        public PageResult<Order> GetOrderByShippingNo(string shippingNo, int pageIndex, int pageSize)
        {
            using (var db = new YintaiHZhouContext())
            {
                var filter = db.ShippingSales.Where(t => t.ShippingCode == shippingNo).Join(db.Orders,t=>t.OrderNo,o=>o.OrderNo,(t,o)=>o);
                filter = filter.OrderByDescending(t => t.CreateDate);
                return filter.ToPageResult(pageIndex,pageSize);
            }
        }

        public PageResult<Order> GetByReturnGoodsInfo(ReturnGoodsInfoRequest request)
        {
            using (var db = new YintaiHZhouContext())
            {
                var filter = db.Orders.Where(t => t.CreateDate >= request.StartDate && t.CreateDate < request.EndDate);

                if (request.OrderNo.IsNotNull())
                {
                    filter = filter.Where(t => t.OrderNo.Contains(request.OrderNo));
                }
                if (request.PayType.IsNotNull())
                {
                    filter = filter.Where(t => t.PaymentMethodCode == request.PayType);
                }
                if (request.SaleOrderNo.IsNotNull())
                {
                    filter = filter.Join(db.OPC_Sale.Where(t => t.SaleOrderNo.Contains(request.SaleOrderNo)),
                        t => t.OrderNo, o => o.OrderNo, (t, o) => t);
                }

                IQueryable<OPC_RMA> filter2 = null;

                if (request.RmaNo.IsNotNull())
                {
                     filter2 = db.OPC_RMA.Where(t => t.RMANo.Contains(request.RmaNo));
                }

                if (request.RmaStatus.HasValue)
                {
                    if (filter2 != null)
                    {
                        filter2 = filter2.Where(t => t.Status == request.RmaStatus.Value);
                    }
                    else
                    {
                        filter2 = db.OPC_RMA.Where(t => t.Status == request.RmaStatus.Value);
                    }
                }

                if (request.StoreID.HasValue)
                {
                    if (filter2 != null)
                    {
                        filter2 = filter2.Where(t => t.StoreId == request.StoreID.Value);
                    }
                    else
                    {
                        filter2 = db.OPC_RMA.Where(t => t.StoreId == request.StoreID.Value);
                    }
                }
                if (filter2!=null)
                {
                    filter = Queryable.Join(filter, filter2, t => t.OrderNo, o => o.OrderNo, (t, o) => t);
                }
                filter = filter.OrderByDescending(t => t.CreateDate);
                return filter.ToPageResult(request.pageIndex,request.pageSize);
            }
        }


 
        #endregion
    }
}