using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intime.OPC.Domain;
using Intime.OPC.Domain.Models;

namespace Intime.OPC.Service
{
    public interface IShippingSaleService:IService
    {
        PageResult<OPC_ShippingSale> GetByShippingCode(string shippingCode, int pageIndex, int pageSize = 20);

        void Shipped(string saleOrderNo,int userID);

        void PrintExpress(string orderNo, int userId);
    }
}
