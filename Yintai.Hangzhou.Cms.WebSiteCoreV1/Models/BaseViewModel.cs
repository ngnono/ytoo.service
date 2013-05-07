using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yintai.Architecture.Common.Models;

namespace Yintai.Hangzhou.Cms.WebSiteCoreV1.Models
{
    public abstract class BaseViewModel
    {
        public virtual T ToEntity<T>()
            where T:class
        {
            return AutoMapper.Mapper.DynamicMap<T>(this);
        }
        public virtual T ToEntity<T>(Action<T> moreMapping)
    where T : class
        {
            T entity = AutoMapper.Mapper.DynamicMap<T>(this);
            if (moreMapping != null)
                moreMapping(entity);
            return entity;
        }
        public virtual T FromEntity<T>(dynamic entity) where T:class
        {
            return FromEntity<T>(entity,null) as T;
        }

        public virtual T FromEntity<T>(dynamic entity, Action<T> moreMapping) where T : class
        {
            if (entity == null)
                return default(T);
            T model = AutoMapper.Mapper.DynamicMap(entity, entity.GetType(), typeof(T)) as T;
            if (moreMapping != null)
            {
                moreMapping(model);
            }
            return model;
        }
        public virtual IEnumerable<T> FromEntities<T>(IEnumerable<dynamic> entities) where T : class
        {
            return FromEntities<T>(entities, null);

        }
        public virtual IEnumerable<T> FromEntities<T>(IEnumerable<dynamic> entities, Action<T> moreMapping) where T : class
        {
            if (entities == null)
                return null;
            var models = from entity in entities
                         select this.FromEntity<T>(entity, moreMapping) as T;
            
            return models;
        }
    }

    
}
