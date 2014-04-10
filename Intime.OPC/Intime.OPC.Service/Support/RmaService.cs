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
        private IRmaDetailRepository _rmaDetailRepository;
        public RmaService(IRMARepository repository, IRmaDetailRepository rmaDetailRepository) : base(repository)
        {
            _rmaDetailRepository = rmaDetailRepository;
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

        public IList<RmaDetail> GetDetails(string rmaNo)
        {
            var lst= _rmaDetailRepository.GetByRmaNo(rmaNo);
            return Mapper.Map<OPC_RMADetail, RmaDetail>(lst);
        }

        #endregion
    }
}