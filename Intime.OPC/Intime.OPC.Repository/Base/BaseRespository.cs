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
using System.Linq.Expressions;
using Intime.OPC.Domain;
using Intime.OPC.Domain.Base;

namespace Intime.OPC.Repository.Base
{
    /// <summary>
    ///     Class BaseRepository.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseRepository<T> : IRepository<T> where T : class, IEntity
    {
        #region IRepository<T> Members

        /// <summary>
        ///     Creates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool Create(T entity)
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
        ///     Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool Update(T entity)
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
        ///     Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool Delete(int id)
        {
            using (var db = new YintaiHZhouContext())
            {
                IDbSet<T> set = db.Set<T>();
                T entity = set.FirstOrDefault(t => t.Id == id);
                if (null != entity)
                {
                    set.Remove(entity);
                    db.SaveChanges();
                    return true;
                }
                return false;
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

        #endregion

        /// <summary>
        ///     Selects the specified filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>IQueryable{`0}.</returns>
        protected IList<T> Select(Expression<Func<T, bool>> filter)
        {
            using (var db = new YintaiHZhouContext())
            {
                IDbSet<T> set = db.Set<T>();
                return set.Where(filter).ToList();
            }
        }
        protected PageResult<T> Select<S>(Expression<Func<T, bool>> filter,Expression<Func<T, S>> orderByLambda, bool isAsc, int pageIndex, int pageSize = 20)
        {
            using (var db = new YintaiHZhouContext())
            {
                DbSet<T> set = db.Set<T>();
                int count = set.Count(filter);
                
                pageIndex = pageIndex - 1;
                if (pageIndex < 0)
                {
                    pageIndex = 0;
                }
                IList<T> lst;
                if (isAsc)
                {
                    lst = set.Where(filter).OrderBy(orderByLambda).Skip(pageIndex*pageSize).Take(pageSize).ToList();
                }
                else
                {
                    lst = set.Where(filter).OrderByDescending(orderByLambda).Skip(pageIndex * pageSize).Take(pageSize).ToList();
                }


                return new PageResult<T>(lst, count);

            }
        }

        protected PageResult<TEntity> Select2<TEntity, S>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, S>> orderByLambda, bool isAsc, int pageIndex, int pageSize = 20) where TEntity : class 
        {
            using (var db = new YintaiHZhouContext())
            {
                IDbSet<TEntity> set = db.Set<TEntity>();
                int count = set.Where(filter).Count();

                pageIndex = pageIndex - 1;
                if (pageIndex < 0)
                {
                    pageIndex = 0;
                }
                IList<TEntity> lst;
                if (isAsc)
                {
                    lst = set.Where(filter).OrderBy(orderByLambda).Skip(pageIndex * pageSize).Take(pageSize).ToList();
                }
                else
                {
                    lst = set.Where(filter).OrderByDescending(orderByLambda).Skip(pageIndex * pageSize).Take(pageSize).ToList();
                }


                return new PageResult<TEntity>(lst, count);

            }
        }




        public PageResult<T> GetAll(int pageIndex, int pageSize)
        {
            return Select2<T, int>(t => 1 == 1, o => o.Id, true, pageIndex, pageSize);
        }
    }
}