﻿using System;
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
            pageIndex = pageIndex - 1;
            if (pageIndex < 0)
            {
                pageIndex = 0;
            }
            var lst = source.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            int count = source.Count();
            
            return new PageResult<T>(lst,count);
        }
    }
}
