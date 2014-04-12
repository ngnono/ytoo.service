using System;
using System.Collections.Generic;
using Intime.OPC.Domain;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Dto.Custom;
using Intime.OPC.Domain.Enums;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository;
using Intime.OPC.Service.Map;

namespace Intime.OPC.Service.Support
{
    public class RmaService : BaseService<OPC_RMA>, IRmaService
    {
        private IRmaDetailRepository _rmaDetailRepository;
        private IRmaCommentRepository _rmaCommentRepository;
        public RmaService(IRMARepository repository, IRmaDetailRepository rmaDetailRepository, IRmaCommentRepository rmaCommentRepository)
            : base(repository)
        {
            _rmaDetailRepository = rmaDetailRepository;
            _rmaCommentRepository = rmaCommentRepository;
        }

        #region IRmaService Members

        public PageResult<RMADto> GetAll(PackageReceiveRequest dto)
        {
            dto.StartDate = dto.StartDate.Date;
            dto.EndDate = dto.EndDate.Date.AddDays(1);
            var rep = (IRMARepository)_repository;
            var lst = rep.GetAll(dto.OrderNo, dto.SaleOrderNo, dto.StartDate, dto.EndDate, EnumRMAStatus.NoDelivery, EnumReturnGoodsStatus.ServiceApprove,dto.pageIndex,dto.pageSize);
            return lst;
        }

        public PageResult<RmaDetail> GetDetails(string rmaNo,int pageIndex,int pageSize)
        {
            var lst = _rmaDetailRepository.GetByRmaNo(rmaNo,pageIndex,pageSize);
            return lst;
        }

        public PageResult<RMADto> GetByOrderNo(string orderNo, EnumRMAStatus rmaStatus, EnumReturnGoodsStatus returnGoodsStatus,int pageIndex,int pageSize)
        {
            var rep = (IRMARepository)_repository;
            var  lst = rep.GetAll(orderNo, "", new DateTime(2000, 1, 1), DateTime.Now.Date.AddDays(1), rmaStatus, returnGoodsStatus,pageIndex,pageSize);
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


        public PageResult<RMADto> GetAllPackVerify(PackageReceiveRequest dto)
        {
            dto.StartDate = dto.StartDate.Date;
            dto.EndDate = dto.EndDate.Date.AddDays(1);
            var rep = (IRMARepository)_repository;
            PageResult<RMADto> lst = rep.GetAll(dto.OrderNo, dto.SaleOrderNo, dto.StartDate, dto.EndDate, EnumRMAStatus.ShipVerify, EnumReturnGoodsStatus.ServiceApprove,dto.pageIndex,dto.pageSize);
            return lst;
        }
    }
}