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
using Intime.OPC.Domain.Partials.Models;
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
                var filter = db.Orders.Where(t => t.CreateDate >= starTime && t.CreateDate < endTime);

                if (string.IsNullOrWhiteSpace(orderNo))
                {
                    filter = filter.Where(t => t.OrderNo.Contains(orderNo));
                }
                filter = filter.OrderByDescending(t => t.CreateDate);
                return filter.ToPageResult(pageIndex, pageSize);
            }
        }

        public PageResult<Order> GetOrderByShippingNo(string shippingNo, int pageIndex, int pageSize)
        {
            using (var db = new YintaiHZhouContext())
            {
                var filter = db.OPC_ShippingSales.Where(t => t.ShippingCode == shippingNo).Join(db.Orders, t => t.OrderNo, o => o.OrderNo, (t, o) => o);
                filter = filter.OrderByDescending(t => t.CreateDate);
                return filter.ToPageResult(pageIndex, pageSize);
            }
        }

        public PageResult<Order> GetByReturnGoodsInfo(ReturnGoodsInfoRequest request)
        {
            //todo 查询所有有退货的订单
            using (var db = new YintaiHZhouContext())
            {
                var filter2 = db.OPC_RMAs.Where(t => t.CreatedDate >= request.StartDate && t.CreatedDate < request.EndDate); //&& t.RMAStatus==(int)EnumReturnGoodsStatus.NoProcess;

                var filter = db.Orders.Where(t => true);

                if (request.OrderNo.IsNotNull())
                {
                    filter = filter.Where(t => t.OrderNo == request.OrderNo);
                }
                if (request.PayType.IsNotNull())
                {
                    filter = filter.Where(t => t.PaymentMethodCode == request.PayType);
                }
                if (request.SaleOrderNo.IsNotNull())
                {
                    filter = filter.Join(db.OPC_Sales.Where(t => t.SaleOrderNo == request.SaleOrderNo),
                        t => t.OrderNo, o => o.OrderNo, (t, o) => t);
                }


                if (request.RmaNo.IsNotNull())
                {
                    filter2 = filter2.Where(t => t.RMANo == request.RmaNo);
                }

                if (request.RmaStatus.HasValue && request.RmaStatus != -1)
                {
                    filter2 = filter2.Where(t => t.Status == request.RmaStatus.Value);

                }

                if (request.StoreID.HasValue)
                {
                    filter2 = filter2.Where(t => t.StoreId == request.StoreID.Value);
                }

                var orderIds = filter2.ToList().Select(t => t.OrderNo).Distinct().ToList();

                filter = filter.Where(t => orderIds.Contains(t.OrderNo)).OrderByDescending(t => t.CreateDate);

                return filter.ToPageResult(request.pageIndex, request.pageSize);
            }
        }

        public PageResult<Order> GetBySaleRma(ReturnGoodsInfoRequest request, int? rmaStatus, EnumReturnGoodsStatus returnGoodsStatus)
        {
            using (var db = new YintaiHZhouContext())
            {


                var filter2 = db.OPC_RMAs.Where(t => t.CreatedDate >= request.StartDate && t.CreatedDate < request.EndDate && t.RMAStatus == (int)returnGoodsStatus);

                if (rmaStatus.HasValue)
                {
                    filter2 = filter2.Where(t => t.Status == rmaStatus.Value);
                }

                var filter = db.Orders.Where(t => true);

                if (CurrentUser != null)
                {
                    filter = filter.Where(t => CurrentUser.StoreIds.Contains(t.StoreId));
                }

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
                    filter = filter.Join(db.OPC_Sales.Where(t => t.SaleOrderNo.Contains(request.SaleOrderNo)),
                        t => t.OrderNo, o => o.OrderNo, (t, o) => t);
                }


                if (request.RmaNo.IsNotNull())
                {
                    filter2 = filter2.Where(t => t.RMANo == request.RmaNo);
                }

                if (request.RmaStatus.HasValue)
                {
                    filter2 = filter2.Where(t => t.Status == request.RmaStatus.Value);

                }

                if (request.StoreID.HasValue)
                {
                    filter2 = filter2.Where(t => t.StoreId == request.StoreID.Value);
                }

                var orderIds = filter2.ToList().Select(t => t.OrderNo).Distinct().ToList();

                filter = filter.Where(t => orderIds.Contains(t.OrderNo)).OrderByDescending(t => t.CreateDate);

                return filter.ToPageResult(request.pageIndex, request.pageSize);
            }
        }

        public PageResult<Order> GetByOutOfStockNotify(OutOfStockNotifyRequest request, int orderstatus)
        {
            using (var db = new YintaiHZhouContext())
            {
                var filter2 = db.OPC_Sales.Where(t => true);

                var filter = db.Orders.Where(t => t.CreateDate >= request.StartDate && t.CreateDate < request.EndDate && t.Status == orderstatus);
                if (request.OrderNo.IsNotNull())
                {
                    filter2 = filter2.Where(t => t.OrderNo == request.OrderNo);
                    filter = filter.Where(t => t.OrderNo == request.OrderNo);
                }

                if (request.SaleOrderStatus.HasValue)
                {
                    filter2 = filter2.Where(t => t.Status == request.SaleOrderStatus.Value);
                }

                if (request.PayType.IsNotNull())
                {
                    filter = filter.Where(t => t.PaymentMethodCode == request.PayType);
                }

                if (request.SaleOrderStatus.HasValue)
                {
                    filter2 = filter2.Where(t => t.Status == request.SaleOrderStatus.Value);
                }



                if (request.StoreId.HasValue)
                {
                    filter = filter.Where(t => t.StoreId == request.StoreId.Value);
                }
                filter = Queryable.Join(filter, filter2, t => t.OrderNo, o => o.OrderNo, (t, o) => t);

                filter = filter.OrderByDescending(t => t.CreateDate);

                return filter.ToPageResult(request.pageIndex, request.pageSize);
            }
        }

        public OrderModel GetItemByOrderNo(string orderno)
        {
            using (var db = new YintaiHZhouContext())
            {
                var order = db.Orders;
                var orderTransactions = db.OrderTransactions;
                var shipvia = db.ShipVias;

                var q = from o in order.Where(v => v.OrderNo == orderno)
                        join ot in orderTransactions on o.OrderNo equals ot.OrderNo into tmp1
                        from ot in tmp1.DefaultIfEmpty()
                        join via in shipvia on o.ShippingVia equals via.Id into tmp2
                        from via in tmp2.DefaultIfEmpty()
                        select new OrderModel
                        {
                            BuyDate = o.CreateDate,
                            CustomerAddress = o.ShippingAddress,
                            CustomerFreight = 0m,
                            CustomerName = o.ShippingContactPerson,
                            CustomerPhone = o.ShippingContactPhone,
                            //CustomerRemark = o.Memo,
                            ExpressCompany = via == null ? String.Empty : via.Name,
                            ExpressNo = o.ShippingNo,
                            Id = o.Id,
                            IfReceipt = o.NeedInvoice,
                            MustPayTotal= o.TotalAmount,
                            OrderChannelNo = ot!=null? ot.TransNo:String.Empty,
                            OrderNo = o.OrderNo,
                            OrderSouce = o.OrderSource,
                            //OutGoodsDate = 
                            //OutGoodsType = 
                            PaymentMethodName = o.PaymentMethodName,
                            PostCode = o.ShippingZipCode,
                            //Quantity = o.
                            ReceiptContent = o.InvoiceDetail,
                            ReceiptHead = o.InvoiceSubject,
                            ShippingNo = o.ShippingNo,
                            Status = o.Status,
                            TotalAmount = o.TotalAmount

                        };

                return q.FirstOrDefault();
            }
        }

        #endregion
    }
}