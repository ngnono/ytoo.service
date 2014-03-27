using Intime.OPC.Domain.Models;
using OPCApp.Infrastructure;

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