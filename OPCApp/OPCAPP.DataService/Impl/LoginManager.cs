﻿// ***********************************************************************
// Assembly         : OPCApp.DataService
// Author           : Liuyh
// Created          : 03-15-2014 
//
// Last Modified By : Liuyh
// Last Modified On : 03-16-2014 
// ***********************************************************************
// <copyright file="LoginManager.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.ComponentModel.Composition;
using OPCApp.DataService.Common;
using OPCApp.Domain;
using OPCApp.Infrastructure.Interfaces;

namespace OPCApp.DataService.Impl
{
    /// <summary>
    ///     Class LoginManager
    /// </summary>
    [Export(typeof (ILoginManager))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class LoginManager : ILoginManager
    {
        /// <summary>
        ///     The password
        /// </summary>
        private string password;

        /// <summary>
        ///     The user name
        /// </summary>
        private string userName;

        /// <summary>
        ///     Logins the specified user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="password">The password.</param>
        /// <returns>ILoginModel.</returns>
        public ILoginModel Login(string userId, string password)
        {
            userName = userId;
            this.password = password;
            var info = new LoginInfo();
            info.UserName = userName;
            info.Password = this.password;
            TokenModel tk = RestClient.Post<LoginInfo, TokenModel>("account/token", info);
            if (null == tk)
            {
                var lm = new LoginModel {ErrorCode = RestClient.CurStatusCode};
                return lm;
            }
            IsLogin = true;
            RestClient.SetToken(tk.AccessToken);
            return new LoginModel(tk.UserId, tk.UserName, tk.AccessToken, "", tk.Expires);
        }

        /// <summary>
        ///     Res the login.
        /// </summary>
        /// <returns>ILoginModel.</returns>
        public ILoginModel ReLogin()
        {
            return Login(userName, password);
        }


        /// <summary>
        ///     Gets a value indicating whether this instance is login.
        /// </summary>
        /// <value><c>true</c> 如果实例 is login; 否则, <c>false</c>.</value>
        public bool IsLogin { get; private set; }

        /// <summary>
        ///     Class LoginInfo.
        /// </summary>
        internal class LoginInfo
        {
            /// <summary>
            ///     Gets or sets the name of the user.
            /// </summary>
            /// <value>The name of the user.</value>
            public String UserName { get; set; }

            /// <summary>
            ///     Gets or sets the password.
            /// </summary>
            /// <value>The password.</value>
            public String Password { get; set; }
        }
    }
}