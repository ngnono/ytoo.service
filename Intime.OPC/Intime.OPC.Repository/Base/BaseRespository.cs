// ***********************************************************************
// Assembly         : 01_Intime.OPC.Repository
// Author           : Liuyh
// Created          : 03-20-2014 20:57:33
//
// Last Modified By : Liuyh
// Last Modified On : 03-21-2014 00:44:04
// ***********************************************************************
// <copyright file="BaseRespository.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intime.OPC.Domain.Base;

namespace Intime.OPC.Repository.Base
{
    /// <summary>
    /// Class BaseRepository.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseRepository<T> : IRepository<T> where T : class,IEntity
    {
        /// <summary>
        /// Creates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Create(T entity)
        {
            using (var db = new YintaiHZhouContext())
            {
                if (entity != null)
                {
                    IDbSet<T> set = db.Set<T>();
                    set.Add(entity);
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Update(T entity)
        {
            using (var db = new YintaiHZhouContext())
            {
                if (entity != null)
                {
                    IDbSet<T> set = db.Set<T>();
                    set.AddOrUpdate(entity);
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Delete(int id)
        {
            using (var db = new YintaiHZhouContext())
            {
                IDbSet<T> set = db.Set<T>();
                var entity = set.FirstOrDefault(t => t.Id == id);
                if (null!=entity)
                {
                    set.Remove(entity);
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Selects the specified filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>IQueryable{`0}.</returns>
        protected IList<T> Select(System.Linq.Expressions.Expression<Func<T, bool>> filter)
        {
            using (var db = new YintaiHZhouContext())
            {
                IDbSet<T> set = db.Set<T>();
                return set.Where(filter).ToList();
            }
        }


       public T GetByID(int id)
       {
           using (var db = new YintaiHZhouContext())
           {
               IDbSet<T> set = db.Set<T>();
               return set.FirstOrDefault(t => t.Id == id);
           }
       }
    }
}
