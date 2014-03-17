﻿// ***********************************************************************
// Assembly         : OPCApp.Main.Infrastructure
// Author           : Liuyh
// Created          : 03-15-2014 23:15:59
//
// Last Modified By : Liuyh
// Last Modified On : 03-15-2014 23:20:58
// ***********************************************************************
// <copyright file="ILoginModel.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCApp.Infrastructure.Interfaces
{
    /// <summary>
    /// Interface ILoginModel
    /// </summary>
    public interface ILoginModel
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        /// <value>The user identifier.</value>
        string UserID { get; }

        /// <summary>
        /// 用户名称
        /// </summary>
        /// <value>The name of the user.</value>
        string UserName { get; }

        /// <summary>
        /// Gets the token.
        /// </summary>
        /// <value>The token.</value>
        string Token { get; }

        /// <summary>
        /// 过期时间
        /// </summary>
        /// <value>The expires.</value>
        DateTime Expires { get; }

        /// <summary>
        /// 专柜ID
        /// </summary>
        /// <value>The shoppe identifier.</value>
        string ShoppeID { get; }

    }
}
