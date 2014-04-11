
using System.Collections.Generic;
using Intime.OPC.Domain.Dto;
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

        IList<SaleRmaDto> GetAll(string orderNo, string payType, int? bandId, System.DateTime startTime, System.DateTime endTime, string telephone);

        IList<SaleRmaDto> GetAll(string orderNo, string saleOrderNo, string payType, string rmaNo, System.DateTime startTime, System.DateTime endTime, int? rmaStatus, int? storeId);

        OPC_SaleRMA GetByRmaNo(string rmaNo);
    }
}
