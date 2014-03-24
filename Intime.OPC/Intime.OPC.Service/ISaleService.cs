<<<<<<< HEAD
﻿// ***********************************************************************
// Assembly         : 02_Intime.OPC.Service
// Author           : Liuyh
// Created          : 03-19-2014 20:11:42
//
// Last Modified By : Liuyh
// Last Modified On : 03-23-2014 23:16:55
// ***********************************************************************
// <copyright file="ISaleService.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Models;

namespace Intime.OPC.Service
{
    /// <summary>
    /// Interface ISaleService
    /// </summary>
    public interface ISaleService
    {
        /// <summary>
        /// Selects this instance.
        /// </summary>
        /// <returns>IList{OPC_Sale}.</returns>
        IList<OPC_Sale> Select();
  

        /// <summary>
        /// Gets the remarks by sale no.
        /// </summary>
        /// <param name="saleNo">The sale no.</param>
        /// <returns>IList{OPC_SaleComment}.</returns>
        IList<OPC_SaleComment> GetRemarksBySaleNo(string saleNo);

        /// <summary>
        /// 打印销售单
        /// </summary>
        /// <param name="orderNo">The order no.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool PrintSale(string orderNo, int userId);

        /// <summary>
        /// Gets the sale order details.
        /// </summary>
        /// <param name="saleOrderNo">The sale order no.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>IList{OPC_SaleDetail}.</returns>
        IList<OPC_SaleDetail> GetSaleOrderDetails(string saleOrderNo, int userId);

        /// <summary>
        /// 销售提货
        /// </summary>
        /// <param name="saleOrderNo">The sale order no.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool SetSaleOrderPickUp(string saleOrderNo, int userId);

        /// <summary>
        /// 物流入库
        /// </summary>
        /// <param name="saleOrderNo">The sale order no.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool SetShipInStorage(string saleOrderNo, int userId);

        /// <summary>
        /// 打印发货单
        /// </summary>
        /// <param name="orderNo">The order no.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool PrintInvoice(string orderNo, int userId);

        /// <summary>
        /// 完成打印销售单
        /// </summary>
        /// <param name="orderNo">The order no.</param>
        /// <param name="userId">The user identifier.</param>
        bool FinishPrintSale(string orderNo, int userId);

        /// <summary>
        /// 打印快递单
        /// </summary>
        /// <param name="orderNo">The order no.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool PrintExpress(string orderNo, int userId);

        /// <summary>
        /// 发货
        /// </summary>
        /// <param name="orderNo">The order no.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool  Shipped(string orderNo, int userId);

        /// <summary>
        /// 缺货
        /// </summary>
        /// <param name="orderNo">The order no.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool  StockOut(string orderNo, int userId);



        /// <summary>
        ///  获得 未提货 的数据
        /// </summary>
        /// <param name="saleOrderNo"></param>
        /// <param name="userId"></param>
        /// <param name="dtStart"></param>
        /// <param name="dtEnd"></param>
        /// <returns></returns>
        IList<SaleDto> GetNoPickUp(string saleOrderNo, int userId, string orderNo, System.DateTime dtStart, System.DateTime dtEnd);

        /// <summary>
        /// 获得已完成 打印销售单 的数据
        /// </summary>
        /// <param name="saleId"></param>
        /// <param name="userId"></param>
        /// <param name="dtStart"></param>
        /// <param name="dtEnd"></param>
        /// <returns></returns>
        IList<OPC_Sale> GetPrintSale(string saleId, int userId, string orderNo, System.DateTime dtStart, System.DateTime dtEnd);

        /// <summary>
        ///获得已完成 打印快递单 的数据
        /// </summary>
        /// <param name="saleOrderNo">The sale order no.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="orderNo">The order no.</param>
        /// <param name="dtStart">The dt start.</param>
        /// <param name="dtEnd">The dt end.</param>
        /// <returns>IList{OPC_Sale}.</returns>
        IList<OPC_Sale> GetPrintExpress(string saleOrderNo, int userId, string orderNo, System.DateTime dtStart, System.DateTime dtEnd);

        /// <summary>
        /// 获得已完成 打印发货单 的数据
        /// </summary>
        /// <param name="saleOrderNo">The sale order no.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="orderNo">The order no.</param>
        /// <param name="dtStart">The dt start.</param>
        /// <param name="dtEnd">The dt end.</param>
        /// <returns>IList{OPC_Sale}.</returns>
        IList<OPC_Sale> GetPrintInvoice(string saleOrderNo, int userId, string orderNo, System.DateTime dtStart, System.DateTime dtEnd);

        /// <summary>
        /// 获得已完成 物流入库 的数据
        /// </summary>
        /// <param name="saleOrderNo">The sale order no.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="orderNo">The order no.</param>
        /// <param name="dtStart">The dt start.</param>
        /// <param name="dtEnd">The dt end.</param>
        /// <returns>IList{OPC_Sale}.</returns>
        IList<OPC_Sale> GetShipInStorage(string saleOrderNo, int userId, string orderNo, System.DateTime dtStart, System.DateTime dtEnd);

        bool WriteSaleRemark(OPC_SaleComment comment);
    }
}
=======
﻿using Intime.OPC.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intime.OPC.Service
{
    public interface ISaleService
    {
        IList<OPC_Sale> Select();
        bool UpdateSatus(OPC_Sale sale);

        IList<OPC_SaleComment> GetRemarksBySaleNo(string saleNo);

    }
}
>>>>>>> 57c447bbe22f6b2f14a2f332de06985506fd2e28
