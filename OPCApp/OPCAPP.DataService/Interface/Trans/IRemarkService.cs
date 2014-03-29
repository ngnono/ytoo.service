using  OPCApp.Domain.Models;
using OPCApp.Infrastructure;

namespace OPCApp.DataService.Interface.Trans
{
    public interface IRemarkService
    {
        bool WriteSaleRemark(OPC_SaleComment saleComment);
        PageResult<OPC_SaleComment> GetSaleRemark(string saleId);
        bool WriteOrderRemark(OPC_OrderComment orderComment);
        
        bool WriteSaleDetailsRemark(OPC_SaleDetailsComment saleDetailsComment);
        PageResult<OPC_SaleDetailsComment> GetSaleDetailsRemark(string saleDetailId);
    }
}