using System.Collections.Generic;
using Intime.OPC.Domain.Models;

namespace Intime.OPC.Repository
{
    public interface ISaleDetailRepository : IRepository<OPC_SaleDetail>
    {
        /// <summary>
        ///    根据销售单号获得销售单明细
        /// </summary>
        /// <param name="saleOrderNo">The sale order no.</param>
        /// <returns>IList{OPC_SaleDetail}.</returns>
        IList<OPC_SaleDetail> GetBySaleOrderNo(string saleOrderNo);
    }
}