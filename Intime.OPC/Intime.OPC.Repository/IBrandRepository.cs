// ***********************************************************************
// Assembly         : 01_Intime.OPC.Repository
// Author           : Liuyh
// Created          : 03-28-2014 21:07:30
//
// Last Modified By : Liuyh
// Last Modified On : 03-28-2014 21:07:45
// ***********************************************************************
// <copyright file="IBrandRepository.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Collections.Generic;
using Intime.OPC.Domain;
using Intime.OPC.Domain.Models;

namespace Intime.OPC.Repository
{
    /// <summary>
    /// Interface IBrandRepository
    /// </summary>
    public interface IBrandRepository : IRepository<Brand>
    {
        PageResult<Brand> GetAll(int pageIndex, int pageSize = 20);

        /// <summary>
        /// 获得多个品牌信息
        /// </summary>
        /// <param name="brandIds">The brand ids.</param>
        /// <returns>IList{Brand}.</returns>
        IList<Brand> GetByIds(int[] brandIds);
    }
}