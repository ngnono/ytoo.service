
using Intime.OPC.Domain.Models;

namespace Intime.OPC.Repository
{
    public interface ISaleRMARepository:IRepository<OPC_SaleRMA>
    {
        /// <summary>
        /// 统计同一销售单的数量
        /// </summary>
        /// <param name="saleOrderNo">The sale order no.</param>
        /// <returns>System.Int32.</returns>
        int Count(string saleOrderNo);
    }
}
