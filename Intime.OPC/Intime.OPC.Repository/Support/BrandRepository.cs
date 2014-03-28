// ***********************************************************************
// Assembly         : 01_Intime.OPC.Repository
// Author           : Liuyh
// Created          : 03-28-2014 21:10:15
//
// Last Modified By : Liuyh
// Last Modified On : 03-28-2014 21:10:30
// ***********************************************************************
// <copyright file="BrandRepository.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Collections.Generic;
using System.Linq;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository.Base;

namespace Intime.OPC.Repository.Support
{
    /// <summary>
    /// Class BrandRepository.
    /// </summary>
    public class BrandRepository : BaseRepository<Brand>, IBrandRepository
    {
        public IList<Brand> GetAll()
        {
            using (var db = new YintaiHZhouContext())
            {
                return db.Brands.Where(t => t.Status == 1).ToList();
            }
        }
    }
}