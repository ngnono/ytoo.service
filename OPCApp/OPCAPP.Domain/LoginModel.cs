﻿// ***********************************************************************
// Assembly         : OPCApp.Domain
// Author           : Liuyh
// Created          : 03-16-2014 20:40:15
//
// Last Modified By : Liuyh
// Last Modified On : 03-16-2014 20:46:36
// ***********************************************************************
// <copyright file="LoginModel.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using OPCApp.Infrastructure.Interfaces;

namespace OPCAPP.Domain
{
    /// <summary>
    /// Class LoginModel.
    /// </summary>
    public  class LoginModel : ILoginModel
    {
        public LoginModel(string userId, string userName, string token, string shoppeId, DateTime expires)
        {
            this.UserID = userId;
            this.UserName = userName;
            this.Token = token;
            this.ShoppeID = shoppeId;
            this.Expires = expires;
        }

        /// <summary>
        /// 用户ID
        /// </summary>
        /// <value>The user identifier.</value>
        public string UserID
        {
            get; private set;
        }

        /// <summary>
        /// 用户名称
        /// </summary>
        /// <value>The name of the user.</value>
        public string UserName { get; private set; }

        /// <summary>
        /// Gets the token.
        /// </summary>
        /// <value>The token.</value>
        public string Token { get; private set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        /// <value>The expires.</value>
        public DateTime Expires { get; private set; }

        /// <summary>
        /// 专柜ID
        /// </summary>
        /// <value>The shoppe identifier.</value>
        public string ShoppeID { get; private set; }
    }
}
