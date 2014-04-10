using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Dto.Custom;
using Intime.OPC.Domain.Enums;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository;

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

        public SaleRMAService(ISaleRMARepository saleRmaRepository, ISaleDetailRepository saleDetailRepository,
            ISaleRepository saleRepository, IOrderItemRepository orderItemRepository,
            IRmaDetailRepository rmaDetailRepository, ISectionRepository sectionRepository, IRMARepository rmaRepository, ISaleRmaCommentRepository saleRmaCommentRepository)
            : base(saleRmaRepository)
        {
            _saleDetailRepository = saleDetailRepository;
            _saleRepository = saleRepository;
            _orderItemRepository = orderItemRepository;
            _rmaDetailRepository = rmaDetailRepository;
            _sectionRepository = sectionRepository;
            _rmaRepository = rmaRepository;
            _saleRmaCommentRepository = saleRmaCommentRepository;
        }

        #region ISaleRMAService Members

        public void CreateSaleRMA(int userId, RMAPost rma)
        {
            List<OPC_SaleDetail> saleDetails =
                _saleDetailRepository.GetByOrderNo(rma.OrderNo).OrderByDescending(t => t.SaleCount).ToList();
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

        public IList<SaleRmaDto> GetByReturnGoodsInfo(ReturnGoodsInfoGet request)
        {
            ISaleRMARepository rep = _repository as ISaleRMARepository;
           return   rep.GetAll(request.OrderNo, request.SaleOrderNo,request.PayType, request.RmaNo,
                request.StartDate, request.EndDate, request.RmaStatus, request.StoreID);
        }

        public IList<SaleRmaDto> GetByReturnGoods(ReturnGoodsGet request)
        {
            ISaleRMARepository rep = _repository as ISaleRMARepository;
            request.StartDate = request.StartDate.Date;
            request.EndDate = request.EndDate.Date.AddDays(1);

            var lst= rep.GetAll(request.OrderNo, request.PayType, request.BandId, request.StartDate, request.EndDate,
                request.Telephone);
            
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

        public IList<SaleRmaDto> GetByPack(PackageReceiveDto dto)
        {
            dto.StartDate = dto.StartDate.Date;
            dto.EndDate = dto.EndDate.Date.AddDays(1);

            ISaleRMARepository rep = _repository as ISaleRMARepository;


            var lst = rep.GetAll(dto.OrderNo,dto.SaleOrderNo, "","", dto.StartDate, dto.EndDate,
                null,null);

            return lst;
        }

        #endregion

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
            rma.RealRMASumMoney = ComputeAccount();
            rma.RMACount = Details.Sum(t => t.ReturnCount);
            rma.RMANo = RmaNo;
            rma.Reason = Reason;
            rma.BackDate = DateTime.Now;
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

            return rmaDetail;
        }
    }
}