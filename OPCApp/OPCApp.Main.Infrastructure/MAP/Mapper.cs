// ***********************************************************************
// Assembly         : OPCApp.Main.Infrastructure
// Author           : Liuyh
// Created          : 03-15-2014 15:01:02
//
// Last Modified By : Liuyh
// Last Modified On : 03-15-2014 15:09:58
// ***********************************************************************
// <copyright file="Mapper.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCApp.Infrastructure
{
    /// <summary>
    /// Class Mapper.
    /// </summary>
    public  class Mapper
    {
        /// <summary>
        /// The regist collection
        /// </summary>
        static ICollection<string> registCollection= new Collection<string>();

        /// <summary>
        /// 两个类型之间的映射
        /// </summary>
        /// <typeparam name="TSource">The type of the t source.</typeparam>
        /// <typeparam name="TTagart">The type of the t tagart.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>``1.</returns>
        public static TTagart Map<TSource, TTagart>(TSource source)
        {
            string key = typeof(TSource).FullName + "_" + typeof(TTagart).FullName;
            if (!registCollection.Contains(key))
            {
                AutoMapper.Mapper.CreateMap<TSource, TTagart>();
                registCollection.Add(key);
            }

            return AutoMapper.Mapper.Map<TSource, TTagart>(source);
        }
    }
}
