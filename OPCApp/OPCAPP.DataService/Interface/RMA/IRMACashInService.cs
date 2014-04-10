using OPCApp.Domain.Enums;
using OPCApp.Domain.Models;
using OPCApp.Infrastructure;

namespace OPCApp.DataService.Interface.Trans
{
    public interface IRMACashInService
    {
        PageResult<OPC_RMA> GetRMA(string filter, EnumRMACashStatus enumRMACashStatus);
        PageResult<OPC_RMADetail> GetRMADetailByRMANo(string rmaNo);
        bool FinishRMACashIn(OPC_RMA RMASelect);
    }
}