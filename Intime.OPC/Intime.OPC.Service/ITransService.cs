using System.Collections.Generic;
using Intime.OPC.Domain.Models;

namespace Intime.OPC.Service
{
    public interface ITransService
    {
        bool Finish(Dictionary<string, string> sale);
        IList<OPC_Sale> Select(string startDate, string endDate, string orderNo, string saleOrderNo);

        IList<OPC_SaleDetail> SelectSaleDetail(IEnumerable<string> saleIDs);

        IList<OPC_OrderComment> GetRemarksByOrderNo(string orderNo);

        /// <summary>
        /// 增加订单日志
        /// </summary>
        /// <param name="comment">The comment.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool AddOrderComment(OPC_OrderComment comment);
    }
}