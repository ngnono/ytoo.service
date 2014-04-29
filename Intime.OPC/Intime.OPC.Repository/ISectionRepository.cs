// ***********************************************************************
// Assembly         : 01_Intime.OPC.Repository
// Author           : Liuyh
// Created          : 03-25-2014 01:14:33
//
// Last Modified By : Liuyh
// Last Modified On : 03-25-2014 01:14:48
// ***********************************************************************
// <copyright file="ISectionRepository.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Collections.Generic;
using Intime.OPC.Domain.Models;

namespace Intime.OPC.Repository
{
    /// <summary>
    ///     Interface ISectionRepository
    /// </summary>
    public interface ISectionRepository : IRepository<Section>
    {
        IList<Section> GetByStoreIDs(IList<int> storeIDs);
    }
}