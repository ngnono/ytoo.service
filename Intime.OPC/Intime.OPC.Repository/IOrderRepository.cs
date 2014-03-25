// ***********************************************************************
// Assembly         : 01_Intime.OPC.Repository
// Author           : Liuyh
// Created          : 03-25-2014 13:36:49
//
// Last Modified By : Liuyh
// Last Modified On : 03-25-2014 13:37:17
// ***********************************************************************
// <copyright file="IOrderRepository.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Intime.OPC.Domain.Models;

namespace Intime.OPC.Repository
{
    /// <summary>
    /// Interface IOrderRepository
    /// </summary>
    public interface IOrderRepository : IRepository<Order>
    {
    }
}