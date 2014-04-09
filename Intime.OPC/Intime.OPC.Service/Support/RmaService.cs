using System;
using System.Collections.Generic;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Dto.Custom;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository;
using Intime.OPC.Service.Map;

namespace Intime.OPC.Service.Support
{
    public class RmaService:BaseService<OPC_RMA>, IRmaService
    {
        public RmaService(IRMARepository repository) : base(repository)
        {
        }

        #region IRmaService Members

        public IList<RMADto> GetAll(PackageReceiveDto dto)
        {
            dto.StartDate = dto.StartDate.Date;
            dto.EndDate = dto.EndDate.Date.AddDays(1);
            var rep = (IRMARepository) _repository;
            IList<OPC_RMA> lst = rep.GetAll(dto.OrderNo, dto.SaleOrderNo, dto.StartDate, dto.EndDate);
            return Mapper.Map<OPC_RMA, RMADto>(lst);
        }

        #endregion
    }
}