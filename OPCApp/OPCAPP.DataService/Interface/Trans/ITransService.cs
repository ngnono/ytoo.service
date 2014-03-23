using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intime.OPC.Domain.Models;
using OPCAPP.Domain.Enums;
using OPCApp.Infrastructure;

namespace OPCApp.DataService.Interface.Trans
{
    public interface ITransService
    {
        bool SetStatusAffirmPrintSaleFinish(string saleId);
        bool SetStatusStoreInSure(string saleId);
        bool SetStatusSoldOut(string saleId);
        PageResult<OPC_Sale> Search(string salesfilter);
        PageResult<OPC_SaleDetail> SelectSaleDetail(string saleIds);
    }
}
