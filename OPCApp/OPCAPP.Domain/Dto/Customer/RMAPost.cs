﻿// ***********************************************************************
// Assembly         : 00_Intime.OPC.Domain
// Author           : Liuyh
// Created          : 04-05-2014 18:13:20
//
// Last Modified By : Liuyh
// Last Modified On : 04-05-2014 18:17:51
// ***********************************************************************
// <copyright file="RMAPost.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace OPCApp.Domain.Customer
{
    /// <summary>
    /// 生成销售退货单时 接收实体
    /// </summary>
    public class RMAPost
    {
        /// <summary>
        /// Gets or sets the store fee.
        /// </summary>
        /// <value>The store fee.</value>
        public double StoreFee { get; set; }

        /// <summary>
        /// Gets or sets the custom fee.
        /// </summary>
        /// <value>The custom fee.</value>
        public double CustomFee { get; set; }
        /// <summary>
        /// Gets or sets the remark.
        /// </summary>
        /// <value>The remark.</value>
        public string Remark { get; set; }

        /// <summary>
        /// Gets or sets the real rma sum money.
        /// </summary>
        /// <value>The real rma sum money.</value>
        public double RealRMASumMoney { get; set; }

        /// <summary>
        /// Gets or sets the order no.
        /// </summary>
        /// <value>The order no.</value>
        public string OrderNo { get; set; }

        public int Count { get; set; }
    }
}