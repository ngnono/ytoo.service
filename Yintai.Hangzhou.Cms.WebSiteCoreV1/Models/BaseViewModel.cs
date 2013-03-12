using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yintai.Architecture.Framework.Mapping;

namespace Yintai.Hangzhou.Cms.WebSiteCoreV1.Models
{
    public abstract class BaseViewModel
    {
        public virtual T ToEntity<TSource,T>() where TSource:BaseViewModel
            where T:class
        {
            TSource source = this as TSource;
            return Mapper.Map<TSource, T>(source);
        }
        public virtual T FromEntity<TSource, T>(TSource entity) where T : BaseViewModel
            where TSource:class
        {
            return Mapper.Map<TSource, T>(entity);
        }

    }

    
}
