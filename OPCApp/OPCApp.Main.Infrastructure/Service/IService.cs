﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OPCApp.Infrastructure.REST;
using Intime.OPC.Infrastructure.Service;

namespace Intime.OPC.Infrastructure.Service
{
    /// <summary>
    /// Service interface for CRUD
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IService<T>
    {
        /// <summary>
        /// Create
        /// </summary>
        /// <param name="obj">object to create</param>
        T Create(T obj);

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="obj">object to update</param>
        T Update(T obj);

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id">object id</param>
        void Delete(int id);

        /// <summary>
        /// Query by ID
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>Object with specifier type</returns>
        T Query(int id);

        /// <summary>
        /// Query
        /// </summary>
        /// <returns>Object collection</returns>
        PagedResult<T> Query(IQueryCriteria queryCriteria);
    }
}