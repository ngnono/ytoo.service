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
    public abstract class BaseRespository<T> : IRespository<T> where T : class,IEntity
    {
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

       public IQueryable<T> Select(System.Linq.Expressions.Expression<Func<T, bool>> filter)
        {
            using (var db = new YintaiHZhouContext())
            {
                IDbSet<T> set = db.Set<T>();
                return set.Where(filter);
            }
        }
    }
}
