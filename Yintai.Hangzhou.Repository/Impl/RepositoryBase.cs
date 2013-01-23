using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Yintai.Architecture.Common.Data.EF;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Repository.Contract;

namespace Yintai.Hangzhou.Repository.Impl
{
    public abstract class RepositoryBase<TEntity, TKey> : EFRepository<TEntity>, IRepository<TEntity, TKey> where TEntity : BaseEntity
    {
        //提供IOC注入方式接口
        /// <summary>
        /// EF构造
        /// </summary>
        /// <param name="context"></param>
        protected RepositoryBase(DbContext context)
            : base(context)
        {
        }

        //测试用
        protected RepositoryBase()
            : this(new YintaiHangzhouContext("YintaiHangzhouContext"))
        {
            //this.DbContext = new MyFinancialEntities();
        }

        //public DbContext Context { get; private set; }

        #region IRepository<TEntity, TKey> 成员

        TEntity IRepository<TEntity, TKey>.Create()
        {
            return base.Create();
        }

        void IRepository<TEntity, TKey>.Update(TEntity entity)
        {
            base.Update(entity);

            //执行验证业务
            //context.Entry<T>(entity).GetValidationResult();
            //if (Context.Entry<TEntity>(entity).State == EntityState.Modified)
            //    Context.SaveChanges();
            //return entity;
        }

        TEntity IRepository<TEntity, TKey>.Insert(TEntity entity)
        {
            return base.Insert(entity);
        }

        void IRepository<TEntity, TKey>.Delete(TEntity entity)
        {
            base.Delete(entity);
        }

        TEntity IRepository<TEntity, TKey>.Find(params object[] keyValues)
        {
            return base.Find(keyValues);
        }

        public List<TEntity> FindAll()
        {
            return base.GetAll().ToList();
        }

        #endregion

        /// <summary>
        /// 查找key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public abstract TEntity GetItem(TKey key);
    }
}
