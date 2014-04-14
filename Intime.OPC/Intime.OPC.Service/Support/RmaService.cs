using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IRmaDetailRepository _rmaDetailRepository;
        private readonly IRmaCommentRepository _rmaCommentRepository;
        private readonly IConnectProduct _connectProduct;
        private readonly ISaleRMARepository _saleRmaRepository;
        private IStockRepository _stockRepository;
        public RmaService(IRMARepository repository, IRmaDetailRepository rmaDetailRepository, IRmaCommentRepository rmaCommentRepository, IConnectProduct connectProduct, ISaleRMARepository saleRmaRepository, IStockRepository stockRepository)
            : base(repository)
        {
            _rmaDetailRepository = rmaDetailRepository;
            _rmaCommentRepository = rmaCommentRepository;
            _connectProduct = connectProduct;
            _saleRmaRepository = saleRmaRepository;
            _stockRepository = stockRepository;
            
        }

        #region IRmaService Members

        public PageResult<RMADto> GetAll(PackageReceiveRequest dto)
        {
            dto.StartDate = dto.StartDate.Date;
            dto.EndDate = dto.EndDate.Date.AddDays(1);
            var rep = (IRMARepository)_repository;
            var lst = rep.GetAll(dto.OrderNo, dto.SaleOrderNo, dto.StartDate, dto.EndDate, EnumRMAStatus.ShipNoReceive.AsID(),"",dto.pageIndex,dto.pageSize);
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
            var  lst = rep.GetAll(orderNo, "", new DateTime(2000, 1, 1), DateTime.Now.Date.AddDays(1), rmaStatus.AsID(), returnGoodsStatus.GetDescription(),pageIndex,pageSize);
            return lst;
        }

        public PageResult<RMADto> GetByRmaNo(string rmaNo)
        {
            var rep = (IRMARepository)_repository;
            var lst = rep.GetByRmaNo(rmaNo);
          
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


        /// <summary>
        /// Gets all pack verify.
        /// </summary>
        /// <param name="dto">The dto.</param>
        /// <returns>PageResult{RMADto}.</returns>
        public PageResult<RMADto> GetAllPackVerify(PackageReceiveRequest dto)
        {
            dto.StartDate = dto.StartDate.Date;
            dto.EndDate = dto.EndDate.Date.AddDays(1);
            var rep = (IRMARepository)_repository;
            PageResult<RMADto> lst = rep.GetAll(dto.OrderNo, dto.SaleOrderNo, dto.StartDate, dto.EndDate, EnumRMAStatus.ShipReceive.AsID(), "",dto.pageIndex,dto.pageSize);
            return lst;
        }

        public PageResult<RMADto> GetByFinaceDto(FinaceRequest dto)
        {
            dto.StartDate = dto.StartDate.Date;
            dto.EndDate = dto.EndDate.Date.AddDays(1);
            var rep = (IRMARepository)_repository;
            PageResult<RMADto> lst = rep.GetAll(dto.OrderNo, dto.SaleOrderNo, dto.StartDate, dto.EndDate, EnumRMAStatus.NoDelivery.AsID(), EnumReturnGoodsStatus.CompensateVerify.GetDescription(), dto.pageIndex, dto.pageSize);
            return lst;
        }

        public PageResult<RMADto> GetRmaByPackPrintPress(RmaExpressRequest dto)
        {
            dto.StartDate = dto.StartDate.Date;
            dto.EndDate = dto.EndDate.Date.AddDays(1);
            var rep = (IRMARepository)_repository;
            PageResult<RMADto> lst = rep.GetByPackPrintPress(dto.OrderNo, "", dto.StartDate, dto.EndDate,EnumRMAStatus.ShipReceive.AsID(), dto.pageIndex, dto.pageSize);
            return lst;
        }

        public PageResult<RMADto> GetRmaCashByExpress(RmaExpressRequest dto)
        {
            dto.StartDate = dto.StartDate.Date;
            dto.EndDate = dto.EndDate.Date.AddDays(1);
            var rep = (IRMARepository)_repository;
            PageResult<RMADto> lst = rep.GetByPackPrintPress(dto.OrderNo, "", dto.StartDate, dto.EndDate, EnumRMAStatus.ShipVerifyPass.AsID(), dto.pageIndex, dto.pageSize);
            return lst;
        }

        public void SetRmaCash(string rmaNo)
        {
            var rep = (IRMARepository)_repository;

            var rma = rep.GetByRmaNo2(rmaNo);
            var saleRma = _saleRmaRepository.GetByRmaNo(rmaNo);

            var cashNo=_connectProduct.GetCashNo(saleRma.OrderNo, rmaNo, saleRma.RealRMASumMoney.Value);
            rma.RmaCashNum = cashNo;
            rma.RmaCashDate = DateTime.Now;
            rep.Update(rma);

            

        }

        public void SetRmaCashOver(string rmaNo)
        {
            var saleRma = _saleRmaRepository.GetByRmaNo(rmaNo);

            saleRma.RMACashStatus = EnumRMACashStatus.CashOver.GetDescription();
            saleRma.RMAStatus = EnumReturnGoodsStatus.Valid.GetDescription();
            _saleRmaRepository.Update(saleRma);


            var lstDetail = _rmaDetailRepository.GetByRmaNo(rmaNo, 1, 1000);

            //更新库存
            foreach (var detail in lstDetail.Result)
            {
                var stock = _stockRepository.GetByID(detail.StockId.Value);
                stock.Count += detail.BackCount;
                _stockRepository.Update(stock);
            }
        }

        public PageResult<RMADto> GetRmaReturnByExpress(RmaExpressRequest dto)
        {
            dto.StartDate = dto.StartDate.Date;
            dto.EndDate = dto.EndDate.Date.AddDays(1);
            var rep = (IRMARepository)_repository;
            PageResult<RMADto> lst = rep.GetRmaReturnByExpress(dto.OrderNo, dto.StartDate, dto.EndDate, dto.pageIndex, dto.pageSize);
            return lst;
        }

        public void SetRmaShipInStorage(string rmaNo)
        {
            var saleRma = _saleRmaRepository.GetByRmaNo(rmaNo);

            saleRma.Status = EnumRMAStatus.ShipInStorage.AsID();
            _saleRmaRepository.Update(saleRma);

        }

        public PageResult<RMADto> GetRmaPrintByExpress(RmaExpressRequest dto)
        {
            dto.StartDate = dto.StartDate.Date;
            dto.EndDate = dto.EndDate.Date.AddDays(1);
            var rep = (IRMARepository)_repository;
          // PageResult<RMADto> lst = rep.GetByPackPrintPress(dto.OrderNo, "", dto.StartDate, dto.EndDate, EnumRMAStatus.ShipInStorage.AsID(), dto.pageIndex, dto.pageSize);
            var lst = rep.GetRmaPrintByExpress(dto.OrderNo, dto.StartDate, dto.EndDate, dto.pageIndex, dto.pageSize);
            return lst;
        }

        public void SetRmaPint(string rmaNo)
        {
            var saleRma = _saleRmaRepository.GetByRmaNo(rmaNo);

            saleRma.Status = EnumRMAStatus.PrintRMA.AsID();
            _saleRmaRepository.Update(saleRma);

        }

        public PageResult<RMADto> GetRmaByShoppingGuide(ShoppingGuideRequest dto)
        {
            dto.StartDate = dto.StartDate.Date;
            dto.EndDate = dto.EndDate.Date.AddDays(1);

            var rep = (IRMARepository)_repository;
            var lst = rep.GetRmaByShoppingGuide(dto.OrderNo, dto.StartDate, dto.EndDate, dto.pageIndex, dto.pageSize);
            return lst;

        }

        public PageResult<RMADto> GetRmaByAllOver(ShoppingGuideRequest dto)
        {
            dto.StartDate = dto.StartDate.Date;
            dto.EndDate = dto.EndDate.Date.AddDays(1);

            var rep = (IRMARepository)_repository;
            var lst = rep.GetRmaByAllOver(dto.OrderNo, dto.StartDate, dto.EndDate, dto.pageIndex, dto.pageSize);
            return lst;
        }
    }
}