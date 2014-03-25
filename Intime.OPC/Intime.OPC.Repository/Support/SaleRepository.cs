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
using Intime.OPC.Domain.Enums;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository.Base;

namespace Intime.OPC.Repository.Support
{
    /// <summary>
    /// Class SaleRepository.
    /// </summary>
    public class SaleRepository : BaseRepository<OPC_Sale>, ISaleRepository
    {
        #region ISaleRepository Members

        /// <summary>
        /// Selects this instance.
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

        /// <summary>
        /// Gets the by sale no.
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
        /// Gets the sale order details.
        /// </summary>
        /// <param name="saleOrderNo">The sale order no.</param>
        /// <returns>IList{OPC_SaleDetail}.</returns>
        public IList<OPC_SaleDetail> GetSaleOrderDetails(string saleOrderNo)
        {
            using (var db = new YintaiHZhouContext())
            {
                return db.OPC_SaleDetail.Where(t => t.SaleOrderNo == saleOrderNo).ToList();
            }
        }

        /// <summary>
        /// 获得已完成 打印销售单 的数据
        /// </summary>
        /// <param name="saleId">The sale identifier.</param>
        /// <param name="orderNo">The order no.</param>
        /// <param name="dtStart">The dt start.</param>
        /// <param name="dtEnd">The dt end.</param>
        /// <returns>IList{OPC_Sale}.</returns>
        public IList<OPC_Sale> GetPrintSale(string saleId, string orderNo, DateTime dtStart, DateTime dtEnd)
        {
            return GetData(saleId, orderNo, dtStart, dtEnd, EnumSaleOrderStatus.PrintSale);
        }

        /// <summary>
        /// 获得 未提货 的数据
        /// </summary>
        /// <param name="saleId">The sale identifier.</param>
        /// <param name="orderNo">The order no.</param>
        /// <param name="dtStart">The dt start.</param>
        /// <param name="dtEnd">The dt end.</param>
        /// <returns>IList{OPC_Sale}.</returns>
        public IList<OPC_Sale> GetNoPickUp(string saleId, string orderNo, DateTime dtStart, DateTime dtEnd)
        {
            return GetData(saleId, orderNo, dtStart, dtEnd, EnumSaleOrderStatus.NoPickUp);
        }

        /// <summary>
        /// Updates the satus.
        /// </summary>
        /// <param name="saleNos">The sale nos.</param>
        /// <param name="saleOrderStatus">The sale order status.</param>
        /// <param name="userID">The user identifier.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool UpdateSatus(IEnumerable<string> saleNos, EnumSaleOrderStatus saleOrderStatus, int userID)
        {
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
                try
                {
                    db.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    //todo 增加错误日志
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets the print express.
        /// </summary>
        /// <param name="saleOrderNo">The sale order no.</param>
        /// <param name="orderNo">The order no.</param>
        /// <param name="dtStart">The dt start.</param>
        /// <param name="dtEnd">The dt end.</param>
        /// <returns>IList{OPC_Sale}.</returns>
        public IList<OPC_Sale> GetPrintExpress(string saleOrderNo, string orderNo, DateTime dtStart, DateTime dtEnd)
        {
            return GetData(saleOrderNo, orderNo, dtStart, dtEnd, EnumSaleOrderStatus.PrintExpress);
        }

        /// <summary>
        /// Gets the print invoice.
        /// </summary>
        /// <param name="saleOrderNo">The sale order no.</param>
        /// <param name="orderNo">The order no.</param>
        /// <param name="dtStart">The dt start.</param>
        /// <param name="dtEnd">The dt end.</param>
        /// <returns>IList{OPC_Sale}.</returns>
        public IList<OPC_Sale> GetPrintInvoice(string saleOrderNo, string orderNo, DateTime dtStart, DateTime dtEnd)
        {
            return GetData(saleOrderNo, orderNo, dtStart, dtEnd, EnumSaleOrderStatus.PrintInvoice);
        }

        public IList<OPC_Sale> GetShipInStorage(string saleOrderNo, string orderNo, DateTime dtStart, DateTime dtEnd)
        {
            return GetData(saleOrderNo, orderNo, dtStart, dtEnd, EnumSaleOrderStatus.ShipInStorage);
        }

        public IList<OPC_Sale> GetStockOut(string saleOrderNo, string orderNo, DateTime dtStart, DateTime dtEnd)
        {
            return getSalesData(saleOrderNo, orderNo, dtStart, dtEnd, EnumSaleOrderStatus.StockOut);
        }

        #endregion

    
        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <param name="saleId">The sale identifier.</param>
        /// <param name="orderNo">The order no.</param>
        /// <param name="dtStart">The dt start.</param>
        /// <param name="dtEnd">The dt end.</param>
        /// <param name="saleOrderStatus">The sale order status.</param>
        /// <returns>IList{OPC_Sale}.</returns>
        private IList<OPC_Sale> GetData(string saleId, string orderNo, DateTime dtStart, DateTime dtEnd,
            EnumSaleOrderStatus saleOrderStatus)
        {
            using (var db = new YintaiHZhouContext())
            {
                IQueryable<OPC_Sale> result = db.OPC_Sale.Where(t => t.Status == (int) saleOrderStatus
                                                                     && t.SellDate > dtStart
                                                                     && t.SellDate <= dtEnd);
                if (!string.IsNullOrWhiteSpace(orderNo))
                {
                    result.Where(t => t.OrderNo == orderNo);
                }

                if (!string.IsNullOrWhiteSpace(saleId))
                {
                    result.Where(t => t.SaleOrderNo == saleId);
                }

                return result.ToList();
            }
        }
    }
}