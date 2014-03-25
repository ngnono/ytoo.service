// ***********************************************************************
// Assembly         : OPCApp.Domain
// Author           : Liuyh
// Created          : 03-20-2014 22:49:04
//
// Last Modified By : Liuyh
// Last Modified On : 03-20-2014 23:09:08
// ***********************************************************************
// <copyright file="EnumSaleOrderStatue.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.ComponentModel;

namespace Intime.OPC.Domain.Enums
{
    /// <summary>
    ///     销售单状态
    /// </summary>
    public enum EnumSaleOrderStatus
    {
        /// <summary>
        /// 未提货
        /// </summary>
        [Description("未提货")] NoPickUp = 0,

        /// <summary>
        ///  打印销售单
        /// </summary>
        [Description("打印销售单")]
        PrintSale = 2,
        /// <summary>
        ///  缺货
        /// </summary>
        [Description("缺货")] StockOut = 5,

        /// <summary>
        ///  已补货
        /// </summary>
        [Description("已补货")] Replenished = 10,

        /// <summary>
        /// 已提货
        /// </summary>
        [Description("已提货")] PickUp = 15,

        /// <summary>
        ///  导购提货
        /// </summary>
        [Description("导购提货")] ShoppingGuidePickUp = 20,

        /// <summary>
        ///  物流入库
        /// </summary>
        [Description("物流入库")] ShipInStorage = 25,

        /// <summary>
        ///  打印发货单
        /// </summary>
        [Description("打印发货单")] PrintInvoice = 30,

        /// <summary>
        /// 打印快递单
        /// </summary>
        [Description("打印快递单")] PrintExpress = 35,

        /// <summary>
        ///  已发货
        /// </summary>
        [Description("已发货")] Shipped = 40,
    }
}