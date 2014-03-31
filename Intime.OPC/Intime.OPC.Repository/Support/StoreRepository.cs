// ***********************************************************************
// Assembly         : 01_Intime.OPC.Repository
// Author           : Liuyh
// Created          : 03-25-2014 01:20:45
//
// Last Modified By : Liuyh
// Last Modified On : 03-25-2014 01:21:19
// ***********************************************************************
// <copyright file="StoreRepository.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Linq;
using Intime.OPC.Domain;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository.Base;
using System.Collections.Generic;

namespace Intime.OPC.Repository.Support
{
    /// <summary>
    ///     Class StoreRepository.
    /// </summary>
    public class StoreRepository : BaseRepository<Store>, IStoreRepository
    {
        public PageResult<Store> GetAll(int pageIndex, int pageSize = 20)
        {

          return Select(t => t.Status == 1,pageIndex,pageSize);
           
        }
    }
}