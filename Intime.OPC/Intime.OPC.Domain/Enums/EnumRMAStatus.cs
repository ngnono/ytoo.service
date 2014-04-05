// ***********************************************************************
// Assembly         : OPCApp.Domain
// Author           : Liuyh
// Created          : 03-20-2014 23:49:37
//
// Last Modified By : Liuyh
// Last Modified On : 03-20-2014 23:56:27
// ***********************************************************************
// <copyright file="EnumRMAStatus.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.ComponentModel;

namespace Intime.OPC.Domain.Enums
{
    /// <summary>
    ///     退货单状态
    /// </summary>
    public enum EnumRMAStatus
    {
        /// <summary>
        ///     The no delivery
        /// </summary>
        [Description("未送货")] NoDelivery = 0,

        [Description("物流未收货")]
        ShipNoReceive = 2,
        /// <summary>
        ///     The ship receive
        /// </summary>
        [Description("物流收货")] ShipReceive = 5,

        /// <summary>
        ///     The ship verify
        /// </summary>
        [Description("物流审核")] ShipVerify = 10,


        /// <summary>
        ///     The ship verify
        /// </summary>
        [Description("物流审核未通过")]
        ShipVerifyNotPass = 15,
        /// <summary>
        ///     The ship verify
        /// </summary>
        [Description("物流审核通过")]
        ShipVerifyPass = 20,

        /// <summary>
        ///     The ship in storage
        /// </summary>
        [Description("物流入库")] ShipInStorage = 25,

        /// <summary>
        ///     The print rma
        /// </summary>
        [Description("打印退货单")] PrintRMA = 30,

        /// <summary>
        ///     The shopping guide receive
        /// </summary>
        [Description("导购确认收货")] ShoppingGuideReceive = 35
    }
}