// ***********************************************************************
// Assembly         : 01_Intime.OPC.Repository
// Author           : Liuyh
// Created          : 03-19-2014 23:55:28
//
// Last Modified By : Liuyh
// Last Modified On : 03-21-2014 00:36:23
// ***********************************************************************
// <copyright file="IRespository.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Linq;
using System.Linq.Expressions;
using Intime.OPC.Domain.Base;

namespace Intime.OPC.Repository
{
    /// <summary>
    /// Interface IRespository
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRespository<T> where T : class ,IEntity
    {
        /// <summary>
        /// Creates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Create(T entity);
        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Update(T entity);
        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Delete(int id);

        /// <summary>
        /// Selects the specified filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>IQueryable{`0}.</returns>
        IQueryable<T> Select(Expression<Func<T, bool>> filter);
    }
}
