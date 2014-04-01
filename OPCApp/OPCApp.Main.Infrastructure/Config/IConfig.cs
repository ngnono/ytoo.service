// ***********************************************************************
// Assembly         : OPCApp.Main.Infrastructure
// Author           : Liuyh
// Created          : 03-16-2014 22:29:21
//
// Last Modified By : Liuyh
// Last Modified On : 03-16-2014 22:44:59
// ***********************************************************************
// <copyright file="IConfig.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace OPCApp.Infrastructure.Config
{
    /// <summary>
    ///     Interface IConfig
    /// </summary>
    public interface IConfig
    {
        /// <summary>
        ///     获得配置文件的值
        /// </summary>
        /// <param name="key">主键</param>
        /// <param name="defaultValue">默认值，当没有配置的时候，返回该值</param>
        /// <returns>System.String.</returns>
        string GetValue(string key, string defaultValue = "");
    }
}