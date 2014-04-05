
using Intime.OPC.Domain.Dto.Custom;

namespace Intime.OPC.Service
{
    public interface ISaleRMAService:IService
    {
        void CreateSaleRMA(int userId, RMAPost rma);
    }
}
