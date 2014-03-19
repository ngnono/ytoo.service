using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intime.OPC.Domain.Models;
using OPCApp.Infrastructure;

namespace OPCApp.DataService.Interface.Trans
{
    public interface ITransService
    {
        bool Finish(Dictionary<string, string> sale);
        PageResult<OPC_Sale> Search(IDictionary<string, object> filter);
        PageResult<OPC_SaleDetail> SelectSaleDetail(IDictionary<string, object> filter);
    }
}
