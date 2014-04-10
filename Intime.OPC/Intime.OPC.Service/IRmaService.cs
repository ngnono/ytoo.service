using System.Collections.Generic;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Dto.Custom;

namespace Intime.OPC.Service
{
    public interface IRmaService : IService
    {
        IList<RMADto> GetAll(PackageReceiveDto dto);

        /// <summary>
        ///     获得退货单详情
        /// </summary>
        /// <param name="rmaNo">The rma no.</param>
        /// <returns>IList{RmaDetail}.</returns>
        IList<RmaDetail> GetDetails(string rmaNo);
    }
}