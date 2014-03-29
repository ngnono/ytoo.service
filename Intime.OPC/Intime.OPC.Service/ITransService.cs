using System.Collections.Generic;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Models;

namespace Intime.OPC.Service
{
    public interface ITransService : IService
    {
        bool Finish(Dictionary<string, string> sale);
        IList<OPC_Sale> Select(string startDate, string endDate, string orderNo, string saleOrderNo);

        IList<OPC_SaleDetail> SelectSaleDetail(IEnumerable<string> saleIDs);

        /// <summary>
        /// 增加订单日志
        /// </summary>
        /// <param name="comment">The comment.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool AddOrderComment(OPC_OrderComment comment);

        /// <summary>
        /// Gets the shipping sale by order no.
        /// </summary>
        /// <param name="orderNo">The order no.</param>
        /// <returns>IList{ShippingSaleDto}.</returns>
        ShippingSaleDto GetShippingSaleBySaleNo(string saleNo);
    }
}