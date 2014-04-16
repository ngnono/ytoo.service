using System.Collections.Generic;
using Intime.OPC.Job.Trade.SplitOrder.Models;

namespace Intime.OPC.Job.Trade.SplitOrder
{
    /// <summary>
    /// 销售单存储接口
    /// </summary>
    public interface ISaleOrderStore
    {
        /// <summary>
        /// 保存销售单
        /// </summary>
        /// <param name="saleOrders">销售单列表</param>
        bool Save(IEnumerable<SaleOrderModel> saleOrders);
    }
}
