using System.Collections.Generic;
using Intime.OPC.Job.Trade.SplitOrder.Models;

namespace Intime.OPC.Job.Trade.SplitOrder
{
    /// <summary>
    /// 订单接口
    /// </summary>
    public interface IOrderStore
    {
        /// <summary>
        /// 获取待处理的订单
        /// </summary>
        /// <returns></returns>
        IEnumerable<OrderModel> GetPendingOrders();

        /// <summary>
        /// 更新完成拆单状态
        /// </summary>
        /// <param name="orderId">订单Id</param>
        ///  <param name="statusCode">状态码</param>
        /// <returns>操作结果</returns>
        bool UpdateStatus(int orderId, int statusCode);
    }
}
