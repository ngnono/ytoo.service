using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intime.OPC.Domain;

namespace System.Linq
{
    public static  class QueryableEx
    {
        public static PageResult<T> ToPageResult<T>(this IQueryable<T> source ,int pageIndex, int pageSize = 20)
        {
            pageIndex = pageSize - 1;
            if (pageIndex < 0)
            {
                pageIndex = 0;
            }
            int count = source.Count();
            var lst = source.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            return new PageResult<T>(lst,count);
        }
    }
}
