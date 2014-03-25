// ***********************************************************************
// Assembly         : 01_Intime.OPC.Repository
// Author           : Liuyh
// Created          : 03-25-2014 13:37:53
//
// Last Modified By : Liuyh
// Last Modified On : 03-25-2014 13:38:11
// ***********************************************************************
// <copyright file="OrderRepository.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository.Base;

namespace Intime.OPC.Repository.Support
{
    /// <summary>
    /// Class OrderRepository.
    /// </summary>
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
    }
}