using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intime.OPC.Domain;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Dto.Custom;
using Intime.OPC.Domain.Enums;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository;
using Intime.OPC.Repository.Base;

namespace Intime.OPC.Service.Support
{
    public class SaleRMAService : BaseService<OPC_SaleRMA>, ISaleRMAService
    {
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IRmaDetailRepository _rmaDetailRepository;
        private readonly IRMARepository _rmaRepository;
        private readonly ISaleDetailRepository _saleDetailRepository;
        private readonly ISaleRepository _saleRepository;
        private readonly ISectionRepository _sectionRepository;
        private ISaleRmaCommentRepository _saleRmaCommentRepository;
        private IAccountService _accountService;

        public SaleRMAService(ISaleRMARepository saleRmaRepository, ISaleDetailRepository saleDetailRepository,
            ISaleRepository saleRepository, IOrderItemRepository orderItemRepository,
            IRmaDetailRepository rmaDetailRepository, ISectionRepository sectionRepository, IRMARepository rmaRepository, ISaleRmaCommentRepository saleRmaCommentRepository, IAccountService accountService)
            : base(saleRmaRepository)
        {
            _saleDetailRepository = saleDetailRepository;
            _saleRepository = saleRepository;
            _orderItemRepository = orderItemRepository;
            _rmaDetailRepository = rmaDetailRepository;
            _sectionRepository = sectionRepository;
            _rmaRepository = rmaRepository;
            _saleRmaCommentRepository = saleRmaCommentRepository;
            _accountService = accountService;
            
        }

        #region ISaleRMAService Members

        public void CreateSaleRMA(int userId, RMAPost rma)
        {
            List<OPC_SaleDetail> saleDetails =
                _saleDetailRepository.GetByOrderNo(rma.OrderNo,1,1000).Result.OrderByDescending(t => t.SaleCount).ToList();
            List<OPC_Sale> sales =
                _saleRepository.GetByOrderNo(rma.OrderNo, -1).OrderByDescending(t => t.SalesCount).ToList();
            IList<OrderItem> orderItems =
                _orderItemRepository.GetByIDs(rma.ReturnProducts.Select(t => t.Key));

            IList<OPC_RMADetail> lstRmaDetails = new List<OPC_RMADetail>(); //生成的退货单明细
            IList<OPC_SaleDetail> lstSaleDetails = new List<OPC_SaleDetail>(); //需要退货的 销售单明细
            IDictionary<string, decimal> dicSaleOrderNoAcount = new Dictionary<string, decimal>();

            int orderCount = ((ISaleRMARepository) _repository).Count(rma.OrderNo) + 1;
            IList<RmaConfig> lstRmaConfigs = new List<RmaConfig>();
            foreach (var kv in rma.ReturnProducts)
            {
                OrderItem oItem = orderItems.FirstOrDefault(t => t.Id == kv.Key);

                List<OPC_SaleDetail> details =
                    saleDetails.Where(t => t.OrderItemId == kv.Key).OrderByDescending(t => t.SaleCount).ToList();
                int returnCount = kv.Value;
                OPC_SaleDetail detail = details.FirstOrDefault();
                while (returnCount > detail.SaleCount)
                {
                    RmaConfig config = lstRmaConfigs.FirstOrDefault(t => t.SaleOrderNo == detail.SaleOrderNo);
                    if (config == null)
                    {
                        config = new RmaConfig(userId);
                        config.RmaNo = CreateRmaNo(rma.OrderNo, orderCount);
                        config.SaleOrderNo = detail.SaleOrderNo;
                        config.RefundAmount = (Decimal)(rma.RealRMASumMoney);
                        config.StoreFee = (Decimal)rma.StoreFee;
                        config.CustomFee = (Decimal)rma.CustomFee;
                        rma.RealRMASumMoney = 0;
                        rma.StoreFee = 0;
                        rma.CustomFee = 0;

                        config.OpcSale = sales.FirstOrDefault(t => t.SaleOrderNo == config.SaleOrderNo);
                        config.StoreID = _sectionRepository.GetByID(config.OpcSale.SectionId.Value).StoreId.Value;
                        lstRmaConfigs.Add(config);
                        orderCount++;
                    }
                    var subConfig = new SubRmaConfig();
                    subConfig.OpcSaleDetail = detail;
                    subConfig.OrderItem = oItem;
                    subConfig.OrderDetailID = kv.Key;
                    subConfig.ReturnCount = detail.SaleCount;
                    config.Details.Add(subConfig);
                    lstRmaConfigs.Add(config);

                    details.Remove(detail);
                    detail = details.FirstOrDefault();
                    returnCount = returnCount - detail.SaleCount;
                }

                if (returnCount > 0)
                {
                    RmaConfig config = lstRmaConfigs.FirstOrDefault(t => t.SaleOrderNo == detail.SaleOrderNo);
                    if (config == null)
                    {
                        config = new RmaConfig(userId);
                        config.Reason = rma.Remark;
                        config.SaleOrderNo = detail.SaleOrderNo;
                        config.RefundAmount = (Decimal)(rma.RealRMASumMoney);
                        config.StoreFee = (Decimal)rma.StoreFee;
                        config.CustomFee = (Decimal)rma.CustomFee;
                        rma.RealRMASumMoney = 0;
                        rma.StoreFee = 0;
                        rma.CustomFee = 0;
                        config.OpcSale = sales.FirstOrDefault(t => t.SaleOrderNo == config.SaleOrderNo);
                        config.RmaNo = CreateRmaNo(rma.OrderNo, orderCount);
                        lstRmaConfigs.Add(config);
                    }
                    var subConfig = new SubRmaConfig();
                    subConfig.OpcSaleDetail = detail;
                    subConfig.OrderItem = oItem;
                    subConfig.OrderDetailID = kv.Key;
                    subConfig.ReturnCount = returnCount;
                    config.Details.Add(subConfig);
                    //lstRmaConfigs.Add(config);
                }
            }

            Save(lstRmaConfigs);
        }

        public PageResult<SaleRmaDto> GetByReturnGoodsInfo(ReturnGoodsInfoRequest request)
        {
            ISaleRMARepository rep = _repository as ISaleRMARepository;

           return   rep.GetAll(request.OrderNo, request.SaleOrderNo,request.PayType, request.RmaNo,
                request.StartDate, request.EndDate, request.RmaStatus, request.StoreID, "", request.pageIndex, request.pageSize);
        }

        public PageResult<SaleRmaDto> GetByReturnGoods(ReturnGoodsRequest request, int userId)
        {
       
            ISaleRMARepository rep = _repository as ISaleRMARepository;
            request.StartDate = request.StartDate.Date;
            request.EndDate = request.EndDate.Date.AddDays(1);
            rep.SetCurrentUser(_accountService.GetByUserID(userId));
            var lst= rep.GetAll(request.OrderNo, request.PayType, request.BandId, request.StartDate, request.EndDate,
                request.Telephone,request.pageIndex,request.pageSize);
            
            return lst;
        }

        public void AddComment(OPC_SaleRMAComment comment)
        {
            _saleRmaCommentRepository.Create(comment);
        }

        public IList<OPC_SaleRMAComment> GetCommentByRmaNo(string rmaNo)
        {
       
            return _saleRmaCommentRepository.GetByRmaID(rmaNo);
        }

        public PageResult<SaleRmaDto> GetByPack(PackageReceiveRequest dto)
        {
            dto.StartDate = dto.StartDate.Date;
            dto.EndDate = dto.EndDate.Date.AddDays(1);

            ISaleRMARepository rep = _repository as ISaleRMARepository;
            rep.SetCurrentUser(_accountService.GetByUserID(UserId));
            var lst = rep.GetAll(dto.OrderNo,dto.SaleOrderNo, "","", dto.StartDate, dto.EndDate,
                EnumRMAStatus.NoDelivery.AsID(),null,EnumReturnGoodsStatus.ServiceApprove.GetDescription(),dto.pageIndex,dto.pageSize);

            return lst;
        }

        #endregion


        /// <summary>
        /// 客服同意退货
        /// </summary>
        /// <param name="rmaNo">The rma no.</param>
        public void AgreeReturnGoods(string rmaNo)
        {
            var rep = (ISaleRMARepository)_repository;
            var saleRma = rep.GetByRmaNo(rmaNo);
            if (saleRma==null)
            {
                throw new Exception("快递单不存在,退货单号:"+rmaNo);
            }
            if (saleRma.RMAStatus != EnumReturnGoodsStatus.NoProcess.GetDescription())
            {
                throw new Exception("快递单已经确认过，退货单号:" + rmaNo);
            }
            if (saleRma.Status>EnumRMAStatus.NoDelivery.AsID())
            {
                throw new Exception("快递单已经确认过，退货单号:" + rmaNo);
            }
            saleRma.RMAStatus = EnumReturnGoodsStatus.ServiceApprove.GetDescription();
            saleRma.ServiceAgreeTime = DateTime.Now;
            rep.Update(saleRma);
        }

        public void ShippingReceiveGoods(string rmaNo)
        {
            var rep = (ISaleRMARepository)_repository;
            var saleRma = rep.GetByRmaNo(rmaNo);
            if (saleRma == null)
            {
                throw new Exception("快递单不存在,退货单号:" + rmaNo);
            }
            if (saleRma.RMAStatus.IsNull())
            {
                throw new Exception("客服未确认,退货单号:" + rmaNo);
            }
            if (saleRma.Status!=EnumRMAStatus.ShipReceive.AsID())
            {
                throw new Exception("该退货单已经确认或正在审核,退货单号:" + rmaNo);
            }
           
            saleRma.Status = EnumRMAStatus.ShipReceive.AsID();
            rep.Update(saleRma);
        }

        public PageResult<SaleRmaDto> GetByReturnGoodPay(ReturnGoodsPayRequest dto)
        {
            dto.StartDate = dto.StartDate.Date;
            dto.EndDate = dto.EndDate.Date.AddDays(1);

            ISaleRMARepository rep = _repository as ISaleRMARepository;
            rep.SetCurrentUser(_accountService.GetByUserID(UserId));
            var lst = rep.GetAll(dto.OrderNo,"", dto.PayType, "", dto.StartDate, dto.EndDate,
                EnumRMAStatus.NoDelivery.AsID(), null, EnumReturnGoodsStatus.ServiceApprove.GetDescription(),dto.pageIndex,dto.pageSize);

            return lst;
        }

        public void CompensateVerify(string rmaNo, decimal money)
        {
            var rep = (ISaleRMARepository)_repository;
            rep.SetCurrentUser(_accountService.GetByUserID(UserId));
            var saleRma = rep.GetByRmaNo(rmaNo);
            if (saleRma == null)
            {
                throw new Exception("快递单不存在,退货单号:" + rmaNo);
            }
            if (saleRma.RMAStatus.IsNull())
            {
                throw new Exception("客服未确认,退货单号:" + rmaNo);
            }
            if (saleRma.Status > EnumReturnGoodsStatus.PayVerify.AsID())
            {
                throw new Exception("该退货单已经确认,退货单号:" + rmaNo);
            }
           
            saleRma.Status = EnumReturnGoodsStatus.PayVerify.AsID();
            saleRma.RealRMASumMoney = money;
            rep.Update(saleRma);
        }

        public PageResult<SaleRmaDto> GetByRmaNo(string rmaNo, int pageIndex, int pageSize)
        {
            ISaleRMARepository rep = _repository as ISaleRMARepository;
            rep.SetCurrentUser(_accountService.GetByUserID(UserId));
            return rep.GetAll("", "", "", rmaNo, new DateTime(2000,1,1),DateTime.Now.Date.AddDays(1),
                EnumRMAStatus.NoDelivery.AsID(), null, EnumReturnGoodsStatus.ServiceApprove.GetDescription(),pageIndex,pageSize);
        }

        public void PackageVerify(string rmaNo,bool passed)
        {
            var rep = (ISaleRMARepository)_repository;
            var saleRma = rep.GetByRmaNo(rmaNo);
            if (saleRma == null)
            {
                throw new Exception("快递单不存在,退货单号:" + rmaNo);
            }
            if (saleRma.RMAStatus.IsNull())
            {
                throw new Exception("客服未确认,退货单号:" + rmaNo);
            }
            if (saleRma.RMAStatus !=EnumRMAStatus.ShipReceive.GetDescription())
            {
                throw new Exception("该退货单已经确认或正在财务审核,退货单号:" + rmaNo);
            }
            string  rmastaturs=passed?EnumRMAStatus.ShipVerifyPass.GetDescription():EnumRMAStatus.ShipVerifyNotPass.GetDescription();

            saleRma.RMAStatus = rmastaturs;
          
            rep.Update(saleRma);
        }

        public PageResult<SaleRmaDto> GetByFinaceDto(FinaceRequest dto)
        {
            dto.StartDate = dto.StartDate.Date;
            dto.EndDate = dto.EndDate.Date.AddDays(1);

            ISaleRMARepository rep = _repository as ISaleRMARepository;
            rep.SetCurrentUser(_accountService.GetByUserID(UserId));
            var lst = rep.GetAll(dto.OrderNo, dto.SaleOrderNo, "", "", dto.StartDate, dto.EndDate,
                EnumRMAStatus.NoDelivery.AsID(), null, EnumReturnGoodsStatus.CompensateVerify.GetDescription(),dto.pageIndex,dto.pageSize);

            return lst;
        }

        public void FinaceVerify(string rmaNo, bool pass)
        {
            string rmastaturs = pass ? EnumReturnGoodsStatus.CompensateVerifyPass.GetDescription() : EnumReturnGoodsStatus.CompensateVerifyFailed.GetDescription();

            var rep = (ISaleRMARepository)_repository;
            var saleRma = rep.GetByRmaNo(rmaNo);
            if (saleRma == null)
            {
                throw new Exception("快递单不存在,退货单号:" + rmaNo);
            }
 
            if (saleRma.RMAStatus == EnumReturnGoodsStatus.CompensateVerify.GetDescription())
            {
                saleRma.RMAStatus = rmastaturs;
                rep.Update(saleRma);
                return;
            }
            throw new Exception("该退货单已经确认或客服正在确认,退货单号:" + rmaNo);
            
           
        }

        private void Save(IList<RmaConfig> configs)
        {
            foreach (RmaConfig config in configs)
            {
                config.Create();
                _rmaRepository.Create(config.OpcRma);
                _repository.Create(config.OpcSaleRma);
                foreach (OPC_RMADetail rmaDetail in config.OpcRmaDetails)
                {
                    _rmaDetailRepository.Create(rmaDetail);
                }
            }
        }

        private string CreateRmaNo(string orderNo, int count)
        {
            return orderNo + count.ToString().PadLeft(3, '0');
        }


       
    }

    internal class RmaConfig
    {
        public RmaConfig(int userId)
        {
            UserId = userId;
            Details = new List<SubRmaConfig>();
            OpcRmaDetails = new List<OPC_RMADetail>();
        }

        public string Reason { get; set; }
        public decimal CustomFee { get; set; }
        public decimal StoreFee { get; set; }
        public decimal RefundAmount { get; set; }
        public int UserId { get; private set; }

        public int StoreID { get; set; }
        public string RmaNo { get; set; }

        public string SaleOrderNo { get; set; }


        public IList<SubRmaConfig> Details { get; set; }

        public OPC_Sale OpcSale { get; set; }

        public OPC_RMA OpcRma { get; private set; }
        public OPC_SaleRMA OpcSaleRma { get; private set; }

        public IList<OPC_RMADetail> OpcRmaDetails { get; private set; }

        public void Create()
        {
            foreach (SubRmaConfig subRmaConfig in Details)
            {
                OpcRmaDetails.Add(subRmaConfig.CreateRmaDetail(UserId, RmaNo));
            }

            OpcRma = CreateOpcRma(UserId);
            OpcSaleRma = createOpcSaleRma();
        }

        /// <summary>
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>OPC_RMA.</returns>
        private OPC_RMA CreateOpcRma(int userId)
        {
            var rma = new OPC_RMA();
            rma.UserId = userId;
            rma.CreatedDate = DateTime.Now;
            rma.RMANo = this.RmaNo;
            rma.CreatedUser = userId;
            rma.UpdatedDate = rma.CreatedDate;
            rma.UpdatedUser = userId;
            rma.StoreId = StoreID;
            rma.SaleOrderNo = SaleOrderNo;
            rma.OrderNo = OpcSale.OrderNo;
            rma.Status = EnumRMAStatus.NoDelivery.AsID();
            rma.RMAType = 1;

            

            rma.RefundAmount = RefundAmount;
            rma.RMAAmount = ComputeAccount();

            //_repository.Create(rma);
            return rma;
        }

        private OPC_SaleRMA createOpcSaleRma()
        {
            var rma = new OPC_SaleRMA();
            rma.CreatedDate = DateTime.Now;
            rma.CreatedUser = UserId;
            rma.UpdatedDate = rma.CreatedDate;
            rma.UpdatedUser = UserId;
            rma.SaleOrderNo = SaleOrderNo;
            rma.OrderNo = OpcSale.OrderNo;
            rma.StoreFee = StoreFee;
            rma.CustomFee = CustomFee;
            rma.SaleRMASource = "客服退货";
            rma.RealRMASumMoney = RefundAmount;
            rma.RMACount = Details.Sum(t => t.ReturnCount);
            rma.RMANo = RmaNo;
            rma.Reason = Reason;
            rma.CompensationFee = ComputeAccount();
            rma.BackDate = DateTime.Now;
            rma.StoreId = OpcRma.StoreId;
            rma.RecoverableSumMoney = RefundAmount - ComputeAccount();
            rma.RMACashStatus = EnumRMACashStatus.NoCash.GetDescription();

            if (rma.RecoverableSumMoney <= 0)
            {
                rma.Status = EnumRMAStatus.ShipNoReceive.AsID();
                rma.RMAStatus = EnumReturnGoodsStatus.PayVerify.GetDescription();
            }
            else
            {
                rma.RMAStatus = EnumReturnGoodsStatus.CompensateVerify.GetDescription();
            }
            
            return rma;
        }

        public IList<OPC_RMADetail> CreateRmaDetails()
        {
            var lstDetails = new List<OPC_RMADetail>();
            foreach (SubRmaConfig config in Details)
            {
            }

            return lstDetails;
        }

        public decimal ComputeAccount()
        {
            decimal m = 0.0m;
            foreach (SubRmaConfig config in Details)
            {
                m += config.ReturnCount*config.OrderItem.ItemPrice;
            }
            return m;
        }
    }

    internal class SubRmaConfig
    {
        public int OrderDetailID { get; set; }
        public int ReturnCount { get; set; }

        public OPC_SaleDetail OpcSaleDetail { get; set; }

        public OrderItem OrderItem { get; set; }

        /// <summary>
        ///     生成退货单明细 OPC_RMADetail
        /// </summary>
        /// <param name="detail">The detail.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="returnCount">The return count.</param>
        public OPC_RMADetail CreateRmaDetail(int userId, string rmaNo)
        {
            var rmaDetail = new OPC_RMADetail();
            rmaDetail.CreatedUser = userId;
            rmaDetail.CreatedDate = DateTime.Now;
            rmaDetail.UpdatedDate = rmaDetail.CreatedDate;
            rmaDetail.UpdatedUser = userId;
            rmaDetail.Price = OpcSaleDetail.Price;
            rmaDetail.ProdSaleCode = OpcSaleDetail.ProdSaleCode;
            rmaDetail.BackCount = ReturnCount;
            rmaDetail.Status = EnumRMAStatus.NoDelivery.AsID();
            rmaDetail.StockId = OpcSaleDetail.StockId;
            rmaDetail.RMANo = rmaNo;
            rmaDetail.Amount = rmaDetail.Price*rmaDetail.BackCount;
            rmaDetail.RefundDate = DateTime.Now;
            rmaDetail.OrderItemId = OrderDetailID;
            rmaDetail.SectionCode = OpcSaleDetail.SectionCode;
            return rmaDetail;
        }
    }
}