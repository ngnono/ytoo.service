// ***********************************************************************
// Assembly         : GasMap.Core
// Author           : liuyh
// Created          : 03-26-2013
//
// Last Modified By : liuyh
// Last Modified On : 04-17-2013
// ***********************************************************************
// <copyright file="EnumExtensions.cs" company="Tecocity">
//     Copyright (c) Tecocity. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace System
{
    using Runtime.Serialization;
    using System.ComponentModel;

    /// <summary>
    /// Class EnumExtensions
    /// </summary>
    public static class EnumExtensions
    {
        #region Methods

        /// <summary>
        /// ��ȡö�����͵�������Ϣ
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        public static string GetDescription(this Enum value)
        {
            var attributes
                = value.GetType().GetField(value.ToString())
                  .GetCustomAttributes(typeof(DescriptionAttribute), false)
                  as DescriptionAttribute[];

            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }

        /// <summary>
        /// ���ö�����͵�ֵ
        /// </summary>
        /// <param name="enumeration">The enumeration.</param>
        /// <returns>System.Int32.</returns>
        public static int AsID(this Enum enumeration)
        {
            return Convert.ToInt32(enumeration);
        }

        #endregion Methods
    }
}