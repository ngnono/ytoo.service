using System.Collections.Generic;
using Intime.OPC.Domain.Models;

namespace Intime.OPC.Repository
{
    public interface ITransRepository
    {
        bool Finish(Dictionary<string, string> sale);
        IList<OPC_Sale> Select(string startDate, string endDate, string orderNo, string saleOrderNo);
        IList<OPC_SaleDetail> SelectSaleDetail(IEnumerable<string> saleNos);

        ResultMsg InputRemark(string strID, string remark, string strType);
    }
}