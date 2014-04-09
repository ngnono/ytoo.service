using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Dto.Custom;

namespace Intime.OPC.Service
{
    public interface IRmaService:IService
    {
        IList<RMADto> GetAll(PackageReceiveDto dto);
    }
}
