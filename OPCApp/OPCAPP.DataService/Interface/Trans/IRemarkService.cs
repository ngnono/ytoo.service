using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intime.OPC.Domain.Models;
using OPCApp.Infrastructure;
using OPCApp.Infrastructure.DataService;

namespace OPCApp.DataService.Interface.Trans
{
    public interface IRemarkService
    {
        bool WriteSaleRemark(OPC_SaleComment saleComment);
        PageResult<OPC_SaleComment> GetSaleRemark(string saleId);


        bool WriteSaleDetailsRemark(OPC_SaleDetailsComment saleDetailsComment);
        PageResult<OPC_SaleDetailsComment> GetSaleDetailsRemark(string SaleDetailId);

    }
}
