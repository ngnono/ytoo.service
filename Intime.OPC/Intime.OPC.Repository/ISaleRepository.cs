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
using Intime.OPC.Domain;
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
        ///     Updates the satus.
        /// </summary>
        /// <param name="saleNos">The sale nos.</param>
        /// <param name="saleOrderStatus">The sale order status.</param>
        /// <param name="userID">The user identifier.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool UpdateSatus(string saleNo, EnumSaleOrderStatus saleOrderStatus, int userID);

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
        PageResult<OPC_SaleDetail> GetSaleOrderDetails(string saleOrderNo, int pageIndex, int pageSize);

        /// <summary>
        ///     获得 未提货 的数据
        /// </summary>
        /// <param name="saleId">The sale identifier.</param>
        /// <param name="orderNo">The order no.</param>
        /// <param name="dtStart">The dt start.</param>
        /// <param name="dtEnd">The dt end.</param>
        /// <returns>IList{OPC_Sale}.</returns>
        PageResult<OPC_Sale> GetNoPickUp(string saleId, string orderNo, DateTime dtStart, DateTime dtEnd, int pageIndex, int pageSize,params int[] sectionIds);

        /// <summary>
        ///     获得 已发货 的数据
        /// </summary>
        /// <param name="saleId">The sale identifier.</param>
        /// <param name="orderNo">The order no.</param>
        /// <param name="dtStart">The dt start.</param>
        /// <param name="dtEnd">The dt end.</param>
        /// <returns>IList{OPC_Sale}.</returns>
        PageResult<OPC_Sale> GetPickUped(string saleId, string orderNo, DateTime dtStart, DateTime dtEnd, int pageIndex, int pageSize,params int[] sectionIds);

        /// <summary>
        ///     获得已完成 打印销售单 的数据
        /// </summary>
        /// <param name="saleId">The sale identifier.</param>
        /// <param name="orderNo">The order no.</param>
        /// <param name="dtStart">The dt start.</param>
        /// <param name="dtEnd">The dt end.</param>
        /// <returns>IList{OPC_Sale}.</returns>
        PageResult<OPC_Sale> GetPrintSale(string saleId, string orderNo, DateTime dtStart, DateTime dtEnd, int pageIndex, int pageSize,params int[] sectionIds);

        /// <summary>
        ///     获得 打印快递单 的数据
        /// </summary>
        /// <param name="saleOrderNo">The sale order no.</param>
        /// <param name="orderNo">The order no.</param>
        /// <param name="dtStart">The dt start.</param>
        /// <param name="dtEnd">The dt end.</param>
        /// <returns>IList{OPC_Sale}.</returns>
        PageResult<OPC_Sale> GetPrintExpress(string saleOrderNo, string orderNo, DateTime dtStart, DateTime dtEnd, int pageIndex, int pageSize,params int[] sectionIds);

        /// <summary>
        ///     获得 打印发货单 的数据
        /// </summary>
        /// <param name="saleOrderNo">The sale order no.</param>
        /// <param name="orderNo">The order no.</param>
        /// <param name="dtStart">The dt start.</param>
        /// <param name="dtEnd">The dt end.</param>
        /// <returns>IList{OPC_Sale}.</returns>
        PageResult<OPC_Sale> GetPrintInvoice(string saleOrderNo, string orderNo, DateTime dtStart, DateTime dtEnd, int pageIndex, int pageSize, params int[] sectionIds);

        /// <summary>
        ///     获得 物流入库 的数据
        /// </summary>
        /// <param name="saleOrderNo">The sale order no.</param>
        /// <param name="orderNo">The order no.</param>
        /// <param name="dtStart">The dt start.</param>
        /// <param name="dtEnd">The dt end.</param>
        /// <returns>IList{OPC_Sale}.</returns>
        PageResult<OPC_Sale> GetShipInStorage(string saleOrderNo, string orderNo, DateTime dtStart, DateTime dtEnd, int pageIndex, int pageSize,params int[] sectionIds);

        /// <summary>
        /// 读取某个专柜下 某个订单的销售单
        /// </summary>
        /// <param name="orderID">The order identifier.</param>
        /// <param name="sectinID">The sectin identifier.</param>
        /// <returns>IList{OPC_Sale}.</returns>
        IList<OPC_Sale> GetByOrderNo(string orderID, int sectinID);

        PageResult<OPC_Sale> GetShipped(string saleOrderNo, string orderNo, DateTime dtStart, DateTime dtEnd, int pageIndex, int pageSize,params int[] sectionIds);

        IList<OPC_Sale> GetByShippingCode(string shippingCode);
    }
}