using Intime.OPC.Domain.Models;
using OPCApp.Domain;
using OPCApp.Infrastructure;
using OPCAPP.Domain.Enums;

namespace OPCApp.DataService.Interface.Trans
{
    public interface IRMACashInService
    {


        PageResult<OPC_RMA> GetRMA(string filter, EnumRMACashStatus enumRMACashStatus);
        PageResult<OPC_RMADetail> GetRMADetailByRMANo(string rmaNo);
        bool FinishRMACashIn(OPC_RMA RMASelect);
    }
}
