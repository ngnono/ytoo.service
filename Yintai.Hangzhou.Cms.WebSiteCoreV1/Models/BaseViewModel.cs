using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yintai.Hangzhou.Cms.WebSiteCoreV1.Models
{
    public abstract class BaseViewModel
    {
        public virtual T ToEntity<T>()
            where T:class
        {
            return AutoMapper.Mapper.DynamicMap<T>(this);
        }
        public virtual T FromEntity<T>(dynamic entity) where T : BaseViewModel
        {
            return AutoMapper.Mapper.DynamicMap<T>(entity);
        }

    }

    
}
