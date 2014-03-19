using Intime.OPC.Domain.Models;
using System.Collections.Generic;

namespace Intime.OPC.Service
{
    public interface ITransService
    {
        bool Finish(Dictionary<string, string> sale);
        IList<OPC_Sale> Select(string startDate, string endDate, string orderNo, string saleOrderNo);

        IList<OPC_SaleDetail> SelectSaleDetail(string saleIDs);
    }
}