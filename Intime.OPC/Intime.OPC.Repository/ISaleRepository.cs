using Intime.OPC.Domain.Enums;
using Intime.OPC.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intime.OPC.Repository
{
    public interface ISaleRepository:IRepository<OPC_Sale>
    {
        IList<OPC_Sale> Select();
        bool UpdateSatus(IEnumerable<string> saleNos,EnumSaleOrderStatus saleOrderStatus,int userID);

        OPC_Sale GetBySaleNo(string saleNo);

        IList<OPC_SaleDetail> GetSaleOrderDetails(string saleOrderNo);
    }
}
