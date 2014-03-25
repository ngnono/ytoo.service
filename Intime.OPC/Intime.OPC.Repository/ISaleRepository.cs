// ***********************************************************************
// Assembly         : 01_Intime.OPC.Repository
// Author           : Liuyh
// Created          : 03-19-2014 20:11:42
//
// Last Modified By : Liuyh
// Last Modified On : 03-24-2014 01:28:54
// ***********************************************************************
// <copyright file="ISaleRepository.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using Intime.OPC.Domain.Enums;
using Intime.OPC.Domain.Models;

namespace Intime.OPC.Repository
{
    /// <summary>
    ///     Interface ISaleRepository
    /// </summary>
    public interface ISaleRepository : IRepository<OPC_Sale>
    {
        /// <summary>
        ///     Selects this instance.
        /// </summary>
        /// <returns>IList{OPC_Sale}.</returns>
        IList<OPC_Sale> Select();

        /// <summary>
        ///     Updates the satus.
        /// </summary>
        /// <param name="saleNos">The sale nos.</param>
        /// <param name="saleOrderStatus">The sale order status.</param>
        /// <param name="userID">The user identifier.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool UpdateSatus(IEnumerable<string> saleNos, EnumSaleOrderStatus saleOrderStatus, int userID);

        /// <summary>
        ///     Gets the by sale no.
        /// </summary>
        /// <param name="saleNo">The sale no.</param>
        /// <returns>OPC_Sale.</returns>
        OPC_Sale GetBySaleNo(string saleNo);

        /// <summary>
        ///     Gets the sale order details.
        /// </summary>
        /// <param name="saleOrderNo">The sale order no.</param>
        /// <returns>IList{OPC_SaleDetail}.</returns>
        IList<OPC_SaleDetail> GetSaleOrderDetails(string saleOrderNo);

        /// <summary>
        ///     获得 未提货 的数据
        /// </summary>
        /// <param name="saleId">The sale identifier.</param>
        /// <param name="orderNo">The order no.</param>
        /// <param name="dtStart">The dt start.</param>
        /// <param name="dtEnd">The dt end.</param>
        /// <returns>IList{OPC_Sale}.</returns>
        IList<OPC_Sale> GetNoPickUp(string saleId, string orderNo, DateTime dtStart, DateTime dtEnd);

        /// <summary>
        ///     获得已完成 打印销售单 的数据
        /// </summary>
        /// <param name="saleId">The sale identifier.</param>
        /// <param name="orderNo">The order no.</param>
        /// <param name="dtStart">The dt start.</param>
        /// <param name="dtEnd">The dt end.</param>
        /// <returns>IList{OPC_Sale}.</returns>
        IList<OPC_Sale> GetPrintSale(string saleId, string orderNo, DateTime dtStart, DateTime dtEnd);

        /// <summary>
        ///     获得 打印快递单 的数据
        /// </summary>
        /// <param name="saleOrderNo">The sale order no.</param>
        /// <param name="orderNo">The order no.</param>
        /// <param name="dtStart">The dt start.</param>
        /// <param name="dtEnd">The dt end.</param>
        /// <returns>IList{OPC_Sale}.</returns>
        IList<OPC_Sale> GetPrintExpress(string saleOrderNo, string orderNo, DateTime dtStart, DateTime dtEnd);

        /// <summary>
        ///     获得 打印发货单 的数据
        /// </summary>
        /// <param name="saleOrderNo">The sale order no.</param>
        /// <param name="orderNo">The order no.</param>
        /// <param name="dtStart">The dt start.</param>
        /// <param name="dtEnd">The dt end.</param>
        /// <returns>IList{OPC_Sale}.</returns>
        IList<OPC_Sale> GetPrintInvoice(string saleOrderNo, string orderNo, DateTime dtStart, DateTime dtEnd);

        /// <summary>
        ///     获得 物流入库 的数据
        /// </summary>
        /// <param name="saleOrderNo">The sale order no.</param>
        /// <param name="orderNo">The order no.</param>
        /// <param name="dtStart">The dt start.</param>
        /// <param name="dtEnd">The dt end.</param>
        /// <returns>IList{OPC_Sale}.</returns>
        IList<OPC_Sale> GetShipInStorage(string saleOrderNo, string orderNo, DateTime dtStart, DateTime dtEnd);
    }
}