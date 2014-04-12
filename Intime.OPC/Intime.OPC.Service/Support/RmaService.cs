using System;
using System.Collections.Generic;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Dto.Custom;
using Intime.OPC.Domain.Enums;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository;
using Intime.OPC.Service.Map;

namespace Intime.OPC.Service.Support
{
    public class RmaService:BaseService<OPC_RMA>, IRmaService
    {
        private IRmaDetailRepository _rmaDetailRepository;
        private IRmaCommentRepository _rmaCommentRepository;
        public RmaService(IRMARepository repository, IRmaDetailRepository rmaDetailRepository, IRmaCommentRepository rmaCommentRepository) : base(repository)
        {
            _rmaDetailRepository = rmaDetailRepository;
            _rmaCommentRepository = rmaCommentRepository;
        }

        #region IRmaService Members

        public IList<RMADto> GetAll(PackageReceiveDto dto)
        {
            dto.StartDate = dto.StartDate.Date;
            dto.EndDate = dto.EndDate.Date.AddDays(1);
            var rep = (IRMARepository) _repository;
            IList<RMADto> lst = rep.GetAll(dto.OrderNo, dto.SaleOrderNo, dto.StartDate, dto.EndDate,EnumRMAStatus.NoDelivery,EnumReturnGoodsStatus.ServiceApprove);
            return lst;
        }

        public IList<RmaDetail> GetDetails(string rmaNo)
        {
            var lst= _rmaDetailRepository.GetByRmaNo(rmaNo);
            return lst;
        }

        public IList<RMADto> GetByOrderNo(string orderNo, EnumRMAStatus rmaStatus, EnumReturnGoodsStatus returnGoodsStatus)
        {
            var rep = (IRMARepository)_repository;
            IList<RMADto> lst = rep.GetAll(orderNo, "", new DateTime(2000,1,1), DateTime.Now.Date.AddDays(1),rmaStatus,returnGoodsStatus);
            return lst;
        }

        public void AddComment(OPC_RMAComment comment)
        {
            _rmaCommentRepository.Create(comment);
        }

        public IList<OPC_RMAComment> GetCommentByRmaNo(string rmaNo)
        {
            return _rmaCommentRepository.GetByRmaID(rmaNo);
        }

        #endregion


        public IList<RMADto> GetAllPackVerify(PackageReceiveDto dto)
        {
            dto.StartDate = dto.StartDate.Date;
            dto.EndDate = dto.EndDate.Date.AddDays(1);
            var rep = (IRMARepository)_repository;
            IList<RMADto> lst = rep.GetAll(dto.OrderNo, dto.SaleOrderNo, dto.StartDate, dto.EndDate, EnumRMAStatus.ShipVerify, EnumReturnGoodsStatus.ServiceApprove);
            return lst;
        }
    }
}