using System;
using System.Collections.Generic;
using Intime.OPC.Job.Order.Models;
using Intime.O2O.ApiClient.Domain;

namespace Intime.OPC.Job.Order.Repository
{
    /// <summary>
    /// 数据接口
    /// </summary>
    public interface IRemoteRepository
    {
        /// <summary>
        /// 根据Id获取订单状态 
        /// </summary>
        /// <param name="id">订单Id</param>
        /// <returns>信息</returns>
        OrderStatusResultDto GetOrderStatusById(string id, string storeno);

        OrderStatusDto GetOderStatus(OrderStatusDetail orderStatusDetail);
    }
}
