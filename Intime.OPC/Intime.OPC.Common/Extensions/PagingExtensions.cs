// ***********************************************************************
// Assembly         : GasMap.Core
// Author           : liuyh
// Created          : 03-26-2013
//
// Last Modified By : liuyh
// Last Modified On : 03-29-2013
// ***********************************************************************
// <copyright file="PagingExtensions.cs" company="Tecocity">
//     Copyright (c) Tecocity. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace System.Linq
{
    using Collections.Generic;


    /// <summary>
    /// Class PagingExtensions
    /// </summary>
    public static class PagingExtensions
    {
        #region Methods

        /// <summary>
        /// ��ҳ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="pageNumber">ҳ��</param>
        /// <param name="pageSize">ҳ��С</param>
        /// <returns>��ҳ��Ľ��</returns>
        public static IQueryable<T> Page<T>(this IQueryable<T> query, int pageIndex, int pageSize)
        {
            pageIndex = pageIndex - 1;
            if (pageIndex<0)
            {
                pageIndex = 0;
            }
            return query.Skip((pageIndex * pageSize)).Take(pageSize);
        }

        /// <summary>
        /// ��ҳ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">The query.</param>
        /// <param name="pageIndex">ҳ��(��1��ʼ)</param>
        /// <param name="pageSize">ҳ��С</param>
        /// <returns>��ҳ��Ľ��</returns>
        public static IEnumerable<T> Page<T>(this IEnumerable<T> query, int pageIndex, int pageSize)
        {
            pageIndex = pageIndex - 1;
            if (pageIndex < 0)
            {
                pageIndex = 0;
            }
            return query.Skip((pageIndex * pageSize)).Take(pageSize);
        }

        #endregion Methods
    }
}