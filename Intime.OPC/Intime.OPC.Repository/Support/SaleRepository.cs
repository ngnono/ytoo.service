// ***********************************************************************
// Assembly         : 01_Intime.OPC.Repository
// Author           : Liuyh
// Created          : 03-19-2014 20:11:42
//
// Last Modified By : Liuyh
// Last Modified On : 03-24-2014 01:29:37
// ***********************************************************************
// <copyright file="SaleRepository.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intime.OPC.Domain;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Enums;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository.Base;

namespace Intime.OPC.Repository.Support
{
    /// <summary>
    ///     Class SaleRepository.
    /// </summary>
    public class SaleRepository : BaseRepository<OPC_Sale>, ISaleRepository
    {
        #region ISaleRepository Members

        /// <summary>
        ///     Selects this instance.
        /// </summary>
        /// <returns>IList{OPC_Sale}.</returns>
        public IList<OPC_Sale> Select()
        {
            using (var db = new YintaiHZhouContext())
            {
                List<OPC_Sale> saleList = db.OPC_Sale.ToList();
                return saleList;
            }
        }

        public bool UpdateSatus(string saleNo, EnumSaleOrderStatus saleOrderStatus, int userID)
        {
            using (var db = new YintaiHZhouContext())
            {
                OPC_Sale sale = db.OPC_Sale.FirstOrDefault(t => t.SaleOrderNo == saleNo);
                if (sale != null)
                {
                    sale.UpdatedDate = DateTime.Now;
                    sale.UpdatedUser = userID;
                    sale.Status = (int) saleOrderStatus;
                }

                db.SaveChanges();
                return true;
            }
        }

        /// <summary>
        ///     Gets the by sale no.
        /// </summary>
        /// <param name="saleNo">The sale no.</param>
        /// <returns>OPC_Sale.</returns>
        public OPC_Sale GetBySaleNo(string saleNo)
        {
            using (var db = new YintaiHZhouContext())
            {
                return db.OPC_Sale.FirstOrDefault(t => t.SaleOrderNo == saleNo);
            }
        }

        /// <summary>
        ///     Gets the sale order details.
        /// </summary>
        /// <param name="saleOrderNo">The sale order no.</param>
        /// <returns>IList{OPC_SaleDetail}.</returns>
        public PageResult<SaleDetailDto> GetSaleOrderDetails(string saleOrderNo, int pageIndex, int pageSize)
        {
            //return Select2<OPC_SaleDetail, DateTime>(t => t.SaleOrderNo == saleOrderNo, t => t.CreatedDate, false,
            //    pageIndex, pageSize);
            using (var db = new YintaiHZhouContext())
            {
                var query= Queryable.Join(
                    Queryable.Join(db.OPC_SaleDetail.Where(t => t.SaleOrderNo == saleOrderNo), db.OrderItems,
                        t => t.OrderItemId, o => o.Id, (t, o) => new {Sale = t, OrderItem = o}), db.Brands,
                    t => t.OrderItem.BrandId, o => o.Id,
                    (t, o) => new {Sale = t.Sale, OrderItem = t.OrderItem, BrandName = o.Name}).OrderByDescending(t=>t.Sale.CreatedDate);
                var lst = query.ToPageResult(pageIndex,pageSize);
                var lstDto = new List<SaleDetailDto>();
                foreach (var t in lst.Result)
                {
                    var o = Mapper.Map<OPC_SaleDetail, SaleDetailDto>(t.Sale);
                    o.Brand = t.BrandName;
                    o.Color = t.OrderItem.ColorValueName;
                    o.Size = t.OrderItem.SizeValueName;
                    o.ProductNo = t.OrderItem.StoreSalesCode;
                    o.StyleNo = t.OrderItem.StoreItemNo;
                    lstDto.Add(o);
                }
                return new PageResult<SaleDetailDto>(lstDto,lst.TotalCount);
            }
        }

        public PageResult<OPC_Sale> GetPickUped(string saleId, string orderNo, DateTime dtStart, DateTime dtEnd,
            int pageIndex, int pageSize, params int[] sectionIds)
        {
            return getSalesData(saleId, orderNo, dtStart, dtEnd, EnumSaleOrderStatus.PickUp, pageIndex, pageSize,
                sectionIds);
        }

        /// <summary>
        ///     获得已完成 打印销售单 的数据
        /// </summary>
        /// <param name="saleId">The sale identifier.</param>
        /// <param name="orderNo">The order no.</param>
        /// <param name="dtStart">The dt start.</param>
        /// <param name="dtEnd">The dt end.</param>
        /// <returns>IList{OPC_Sale}.</returns>
        public PageResult<OPC_Sale> GetPrintSale(string saleId, string orderNo, DateTime dtStart, DateTime dtEnd,
            int pageIndex, int pageSize, params int[] sectionIds)
        {
            return getSalesData(saleId, orderNo, dtStart, dtEnd, EnumSaleOrderStatus.PrintSale, pageIndex, pageSize,
                sectionIds);
        }

        /// <summary>
        ///     获得 未提货 的数据
        /// </summary>
        /// <param name="saleId">The sale identifier.</param>
        /// <param name="orderNo">The order no.</param>
        /// <param name="dtStart">The dt start.</param>
        /// <param name="dtEnd">The dt end.</param>
        /// <returns>IList{OPC_Sale}.</returns>
        public PageResult<OPC_Sale> GetNoPickUp(string saleId, string orderNo, DateTime dtStart, DateTime dtEnd,
            int pageIndex, int pageSize, params int[] sectionIds)
        {
            return getSalesData(saleId, orderNo, dtStart, dtEnd, EnumSaleOrderStatus.NoPickUp, pageIndex, pageSize,
                sectionIds);
        }

        /// <summary>
        ///     Updates the satus.
        /// </summary>
        /// <param name="saleNos">The sale nos.</param>
        /// <param name="saleOrderStatus">The sale order status.</param>
        /// <param name="userID">The user identifier.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool UpdateSatus(IEnumerable<string> saleNos, EnumSaleOrderStatus saleOrderStatus, int userID)
        {
            //todo 更新状态为实现
            throw new Exception("UpdateSatus 未实现");
            using (var db = new YintaiHZhouContext())
            {
                foreach (string saleNo in saleNos)
                {
                    OPC_Sale sale = db.OPC_Sale.FirstOrDefault(t => t.SaleOrderNo == saleNo);
                    if (sale != null)
                    {
                        sale.UpdatedDate = DateTime.Now;
                        sale.UpdatedUser = userID;
                        sale.Status = (int) saleOrderStatus;
                    }
                }
                IQueryable<OPC_Sale> lst = db.OPC_Sale.Where(t => saleNos.Contains(t.SaleOrderNo));

                IEnumerable<OPC_Sale> lst2 = saleNos.Join(db.OPC_Sale, t => t, o => o.SaleOrderNo, (t, o) => o);

                db.SaveChanges();
                return true;
            }
        }

        /// <summary>
        ///     Gets the print express.
        /// </summary>
        /// <param name="saleOrderNo">The sale order no.</param>
        /// <param name="orderNo">The order no.</param>
        /// <param name="dtStart">The dt start.</param>
        /// <param name="dtEnd">The dt end.</param>
        /// <returns>IList{OPC_Sale}.</returns>
        public PageResult<OPC_Sale> GetPrintExpress(string saleOrderNo, string orderNo, DateTime dtStart, DateTime dtEnd,
            int pageIndex, int pageSize, params int[] sectionIds)
        {
            return getSalesData(saleOrderNo, orderNo, dtStart, dtEnd, EnumSaleOrderStatus.PrintExpress, pageIndex,
                pageSize, sectionIds);
        }

        /// <summary>
        ///     Gets the print invoice.
        /// </summary>
        /// <param name="saleOrderNo">The sale order no.</param>
        /// <param name="orderNo">The order no.</param>
        /// <param name="dtStart">The dt start.</param>
        /// <param name="dtEnd">The dt end.</param>
        /// <returns>IList{OPC_Sale}.</returns>
        public PageResult<OPC_Sale> GetPrintInvoice(string saleOrderNo, string orderNo, DateTime dtStart, DateTime dtEnd,
            int pageIndex, int pageSize, params int[] sectionIds)
        {
            return getSalesData(saleOrderNo, orderNo, dtStart, dtEnd, EnumSaleOrderStatus.PrintInvoice, pageIndex,
                pageSize, sectionIds);
        }

        public PageResult<OPC_Sale> GetShipInStorage(string saleOrderNo, string orderNo, DateTime dtStart,
            DateTime dtEnd, int pageIndex, int pageSize, params int[] sectionIds)
        {
            return getSalesData(saleOrderNo, orderNo, dtStart, dtEnd, EnumSaleOrderStatus.ShipInStorage, pageIndex,
                pageSize, sectionIds);
        }

        public IList<OPC_Sale> GetByOrderNo(string orderID, int sectinID)
        {
            if (sectinID > -1)
            {
                return Select(t => t.OrderNo == orderID && t.SectionId == sectinID);
            }
            return Select(t => t.OrderNo == orderID);
        }

        public PageResult<OPC_Sale> GetShipped(string saleOrderNo, string orderNo, DateTime dtStart, DateTime dtEnd,
            int pageIndex, int pageSize, params int[] sectionIds)
        {
            return getSalesData(saleOrderNo, orderNo, dtStart, dtEnd, EnumSaleOrderStatus.Shipped, pageIndex, pageSize,
                sectionIds);
        }

        public IList<OPC_Sale> GetByShippingCode(string shippingCode)
        {
            var va = (int) (EnumSaleOrderStatus.Void);
            return Select(t => t.ShippingCode == shippingCode && t.Status > va);
        }

        #endregion

        /// <summary>
        ///     Gets the data.
        /// </summary>
        /// <param name="saleId">The sale identifier.</param>
        /// <param name="orderNo">The order no.</param>
        /// <param name="dtStart">The dt start.</param>
        /// <param name="dtEnd">The dt end.</param>
        /// <param name="saleOrderStatus">The sale order status.</param>
        /// <returns>IList{OPC_Sale}.</returns>
        private PageResult<OPC_Sale> getSalesData(string saleId, string orderNo, DateTime dtStart, DateTime dtEnd,
            EnumSaleOrderStatus saleOrderStatus, int pageIndex, int pageSize, params int[] sectionIds)
        {
            using (var db = new YintaiHZhouContext())
            {
                IQueryable<OPC_Sale> query = db.OPC_Sale.Where(t => t.Status == (int) saleOrderStatus
                                                                    && t.SellDate >= dtStart
                                                                    && t.SellDate < dtEnd);

                if (sectionIds!=null)
                {
                    query = query.Where(t => sectionIds.Contains(t.SectionId.Value));
                }

                if (!string.IsNullOrWhiteSpace(orderNo))
                {
                    query = query.Where(t => t.OrderNo.Contains(orderNo));
                }

                if (!string.IsNullOrWhiteSpace(saleId))
                {
                    query = query.Where(t => t.SaleOrderNo.Contains(saleId));
                }
                query = query.OrderByDescending(t => t.CreatedDate);
                return query.ToPageResult(pageIndex, pageSize);
            }
        }
    }
}