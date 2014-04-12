﻿using System.Collections.Generic;
using Intime.OPC.Domain;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Models;

namespace Intime.OPC.Repository
{
    public interface IOrderItemRepository : IRepository<OrderItem>
    {
        PageResult<OrderItemDto> GetByOrderNo(string orderNo, int pageIndex, int pageSize);

        /// <summary>
        /// 通过IDs 获得多个实体
        /// </summary>
        /// <param name="ids">The ids.</param>
        /// <returns>IList{OrderItem}.</returns>
        IList<OrderItem> GetByIDs(IEnumerable<int> ids);
    }
}