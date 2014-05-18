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
            CreateSaleRmaSub(userId, rma, "客服退货");
        }



        private void CreateSaleRmaSub(int userId, RMAPost rma,string saleSource)
        {
            List<OPC_SaleDetail> saleDetails =
                _saleDetailRepository.GetByOrderNo(rma.OrderNo, 1, 1000).Result.OrderByDescending(t => t.SaleCount).ToList();
            List<OPC_Sale> sales =
                _saleRepository.GetByOrderNo(rma.OrderNo, -1).OrderByDescending(t => t.SalesCount).ToList();
            IList<OrderItem> orderItems =
                _orderItemRepository.GetByIDs(rma.ReturnProducts.Select(t => t.Key));

            int orderCount = ((ISaleRMARepository)_repository).Count(rma.OrderNo) + 1;
            IList<RmaConfig> lstRmaConfigs = new List<RmaConfig>();
            foreach (var kv in rma.ReturnProducts)
            {
                OrderItem oItem = orderItems.FirstOrDefault(t => t.Id == kv.Key);

                List<OPC_SaleDetail> details =
                    saleDetails.Where(t => t.OrderItemId == kv.Key).OrderByDescending(t => t.SaleCount).ToList();
                int returnCount = kv.Value;
                OPC_SaleDetail detail = details.FirstOrDefault();
                if (detail==null)
                {
                    //没有销售明细
                    throw new Exception(string.Format("没有销售明细不存在,订单明细号:{0}", kv.Key));
                   
                }
                while (returnCount > detail.SaleCount)
                {
                    RmaConfig config = lstRmaConfigs.FirstOrDefault(t => t.SaleOrderNo == detail.SaleOrderNo);
                    if (config == null)
                    {
                        config = new RmaConfig(userId);
                        config.SaleRmaSource = saleSource;
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
                    
                    //更新退货数量
                    UpdateSaleDetailOfBackNum(detail, detail.SaleCount);
                    
                    returnCount = returnCount - detail.SaleCount;
                }

                if (returnCount > 0)
                {
                    RmaConfig config = lstRmaConfigs.FirstOrDefault(t => t.SaleOrderNo == detail.SaleOrderNo);
                    if (config == null)
                    {
                        config = new RmaConfig(userId);
                        config.SaleRmaSource = saleSource;
                        config.Reason = rma.Remark;
                        config.SaleOrderNo = detail.SaleOrderNo;
                        config.RefundAmount = (Decimal)(rma.RealRMASumMoney);
                        config.StoreFee = (Decimal)rma.StoreFee;
                        config.CustomFee = (Decimal)rma.CustomFee;

                        rma.RealRMASumMoney = 0;
                        rma.StoreFee = 0;
                        rma.CustomFee = 0;

                        config.OpcSale = sales.FirstOrDefault(t => t.SaleOrderNo == config.SaleOrderNo);
                        config.StoreID = _sectionRepository.GetByID(config.OpcSale.SectionId.Value).StoreId.Value;
                        config.RmaNo = CreateRmaNo(rma.OrderNo, orderCount);
                        lstRmaConfigs.Add(config);
                    }
                    var subConfig = new SubRmaConfig();
                    subConfig.OpcSaleDetail = detail;
                    subConfig.OrderItem = oItem;
                    subConfig.OrderDetailID = kv.Key;
                    subConfig.ReturnCount = returnCount;
                    config.Details.Add(subConfig);
                    //更新退货数量
                    UpdateSaleDetailOfBackNum(detail, returnCount);

                    //lstRmaConfigs.Add(config);
                }
            }

            Save(lstRmaConfigs);
        }

        private void UpdateSaleDetailOfBackNum(OPC_SaleDetail saleDetail,int backCount)
        {
            using (var db = new YintaiHZhouContext())
            {
                OPC_SaleDetail sd = db.OPC_SaleDetails.FirstOrDefault(x => x.Id == saleDetail.Id);
                sd.BackNumber = backCount;
                db.SaveChanges();
            }
        }
        public PageResult<SaleRmaDto> GetByReturnGoodsInfo(ReturnGoodsInfoRequest request)
        {
            ISaleRMARepository rep = _repository as ISaleRMARepository;

           return   rep.GetAll(request.OrderNo, request.SaleOrderNo,request.PayType, request.RmaNo,
                request.StartDate, request.EndDate, request.RmaStatus, request.StoreID,EnumReturnGoodsStatus.None, request.pageIndex, request.pageSize);
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
                EnumRMAStatus.ShipNoReceive.AsID(), null, EnumReturnGoodsStatus.None, dto.pageIndex, dto.pageSize);

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
            if (saleRma.RMAStatus != EnumReturnGoodsStatus.NoProcess.AsID())
            {
                throw new Exception("快递单已经确认过，退货单号:" + rmaNo);
            }
            if (saleRma.Status>EnumRMAStatus.NoDelivery.AsID())
            {
                throw new Exception("快递单已经确认过，退货单号:" + rmaNo);
            }
            saleRma.RMAStatus = EnumReturnGoodsStatus.ServiceApprove.AsID();
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
            if (saleRma.RMAStatus < (int)EnumReturnGoodsStatus.NoProcess)
            {
                throw new Exception("客服未确认,退货单号:" + rmaNo);
            }
            if (saleRma.Status>(int)EnumRMAStatus.ShipNoReceive)
            {
                throw new Exception("该退货单已经确认或正在审核,退货单号:" + rmaNo);
            }
           
            saleRma.Status = EnumRMAStatus.ShipReceive.AsID();
            UpdateRMAStatus(rmaNo, EnumRMAStatus.ShipReceive.AsID());

            rep.Update(saleRma);

        }

        public PageResult<SaleRmaDto> GetByReturnGoodPay(ReturnGoodsPayRequest dto)
        {
            dto.StartDate = dto.StartDate.Date;
            dto.EndDate = dto.EndDate.Date.AddDays(1);
            /*
             
             财务退款确认的查询状态条件，就要改为退货状态为已生效，且退货单状态为物流入库的
             zxy 物流入库+通知单品 2014-5-10
             */

            ISaleRMARepository rep = _repository as ISaleRMARepository;
            rep.SetCurrentUser(_accountService.GetByUserID(UserId));
            var lst = rep.GetByReturnGoodPay(dto.OrderNo, "", dto.PayType, "", dto.StartDate, dto.EndDate,
                 null, dto.pageIndex, dto.pageSize);


            return lst;
        }

        /// <summary>
        /// 财务确认
        /// </summary>
        /// <param name="rmaNo"></param>
        /// <param name="money"></param>
        public void CompensateVerify(string rmaNo, decimal money)
        {
            var rep = (ISaleRMARepository)_repository;
            rep.SetCurrentUser(_accountService.GetByUserID(UserId));
            var saleRma = rep.GetByRmaNo(rmaNo);
            if (saleRma == null)
            {
                throw new Exception("快递单不存在,退货单号:" + rmaNo);
            }
            if (saleRma.RMAStatus < 0)
            {
                throw new Exception("客服未确认,退货单号:" + rmaNo);
            }
            if (saleRma.Status > EnumRMAStatus.PayVerify.AsID())
            {
                throw new Exception("该退货单已经确认,退货单号:" + rmaNo);
            }
           
            
            saleRma.RealRMASumMoney = money;
            saleRma.RecoverableSumMoney = saleRma.RealRMASumMoney - saleRma.CompensationFee;
            saleRma.Status = EnumRMAStatus.PayVerify.AsID();
            
            rep.Update(saleRma);
            UpdateRMAStatus(rmaNo, EnumRMAStatus.PayVerify.AsID());

        }

        public PageResult<SaleRmaDto> GetByRmaNo(string rmaNo, int pageIndex, int pageSize)
        {
            ISaleRMARepository rep = _repository as ISaleRMARepository;
            rep.SetCurrentUser(_accountService.GetByUserID(UserId));
            return rep.GetAll("", "", "", rmaNo, new DateTime(2000,1,1),DateTime.Now.Date.AddDays(1),
                EnumRMAStatus.NoDelivery.AsID(), null, EnumReturnGoodsStatus.ServiceApprove,pageIndex,pageSize);
        }

        /// <summary>
        /// 包裹审核
        /// </summary>
        /// <param name="rmaNo"></param>
        /// <param name="passed"></param>
        public void PackageVerify(string rmaNo,bool passed)
        {
            var rep = (ISaleRMARepository)_repository;
            var saleRma = rep.GetByRmaNo(rmaNo);
            if (saleRma == null)
            {
                throw new Exception("快递单不存在,退货单号:" + rmaNo);
            }
            if (saleRma.RMAStatus < (int)EnumReturnGoodsStatus.NoProcess)
            {
                throw new Exception("客服未确认,退货单号:" + rmaNo);
            }
            if (saleRma.Status !=EnumRMAStatus.ShipReceive.AsID())
            {
                throw new Exception("该退货单已经确认或正在财务审核,退货单号:" + rmaNo);
            }
            var  rmastaturs=passed?EnumRMAStatus.ShipVerifyPass.AsID():EnumRMAStatus.ShipVerifyNotPass.AsID();

            saleRma.Status = rmastaturs;

          
            rep.Update(saleRma);

            UpdateRMAStatus(rmaNo, rmastaturs);



        }

        public PageResult<SaleRmaDto> GetByFinaceDto(FinaceRequest dto)
        {
            dto.StartDate = dto.StartDate.Date;
            dto.EndDate = dto.EndDate.Date.AddDays(1);

            ISaleRMARepository rep = _repository as ISaleRMARepository;
            rep.SetCurrentUser(_accountService.GetByUserID(UserId));
            var lst = rep.GetAll(dto.OrderNo, dto.SaleOrderNo, "", "", dto.StartDate, dto.EndDate,
                EnumRMAStatus.NoDelivery.AsID(), null, EnumReturnGoodsStatus.CompensateVerify,dto.pageIndex,dto.pageSize);

            return lst;
        }

        public void FinaceVerify(string rmaNo, bool pass)
        {

            var rep = (ISaleRMARepository)_repository;
            var saleRma = rep.GetByRmaNo(rmaNo);
            if (saleRma == null)
            {
                throw new Exception("快递单不存在,退货单号:" + rmaNo);
            }

            if (saleRma.RMAStatus == EnumReturnGoodsStatus.CompensateVerify.AsID())
            {
                saleRma.RMAStatus = (int)(pass?EnumReturnGoodsStatus.CompensateVerifyPass:EnumReturnGoodsStatus.CompensateVerifyFailed);
                if (pass)
                {
                    saleRma.Status = EnumRMAStatus.ShipNoReceive.AsID();
                    UpdateRMAStatus(rmaNo, EnumRMAStatus.ShipNoReceive.AsID());
                }
                rep.Update(saleRma);
                return;
            }
            throw new Exception("该退货单已经确认或客服正在确认,退货单号:" + rmaNo);
            
           
        }
                                                                
        private void UpdateRMAStatus(string rmaNo, int status)
        {

            var rma = _rmaRepository.GetByRmaNo2(rmaNo);
            rma.Status = status;
            _rmaRepository.Update(rma);

            //using (var db = new YintaiHZhouContext())
            //{
            //    var rma = db.OPC_RMAs.FirstOrDefault(t => t.RMANo == rmaNo);
            //    rma.Status = status;
            //    db.SaveChanges();
            //}




        }

        public PageResult<SaleRmaDto> GetByReturnGoodsCompensate(ReturnGoodsInfoRequest request)
        {
            ISaleRMARepository rep = _repository as ISaleRMARepository;

            //string status = EnumReturnGoodsStatus.CompensateVerifyFailed.GetDescription();
            rep.SetCurrentUser(_accountService.GetByUserID(UserId));
            return rep.GetAll(request.OrderNo, request.SaleOrderNo, request.PayType, request.RmaNo,
                 request.StartDate, request.EndDate, request.RmaStatus, request.StoreID, EnumReturnGoodsStatus.CompensateVerifyFailed, request.pageIndex, request.pageSize);
        }

        public void SetSaleRmaServiceAgreeGoodsBack(string rmaNo)
        {
             var rep = (ISaleRMARepository)_repository;
            var rma = rep.GetByRmaNo(rmaNo);
            if (rma == null)
            {
                throw new Exception(string.Format("退货收货单不存在，退货单号{0}",rmaNo));
            }
            rma.RMAStatus = EnumReturnGoodsStatus.ServiceAgreeGoodsBack.AsID();
            rep.Update(rma);
        }

        public PageResult<SaleRmaDto> GetOrderAutoBack(ReturnGoodsRequest request)
        {
            ISaleRMARepository rep = _repository as ISaleRMARepository;

            request.StartDate = request.StartDate.Date;
            request.EndDate = request.EndDate.Date.AddDays(1);

            rep.SetCurrentUser(_accountService.GetByUserID(UserId));
            var lst = rep.GetOrderAutoBack(request);

            return lst;
        }

        public void CreateSaleRmaAuto(int user, RMAPost request)
        {
            CreateSaleRmaSub(user, request, "网络自助退货");
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
            return orderNo + count.ToString().PadLeft(4, '0');
        }


       
    }

    internal class RmaConfig
    {
        public RmaConfig(int userId)
        {
            UserId = userId;
            Details = new List<SubRmaConfig>();
            OpcRmaDetails = new List<OPC_RMADetail>();
            SaleRmaSource = "客服退货";
        }

        public string Reason { get; set; }
        public decimal CustomFee { get; set; }
        public decimal StoreFee { get; set; }
        public decimal RefundAmount { get; set; }
        public int UserId { get; private set; }

        public int StoreID { get; set; }
        public string RmaNo { get; set; }

        public string SaleOrderNo { get; set; }

        public string SaleRmaSource { get; set; }
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
            rma.SaleRMASource = SaleRmaSource;
            rma.RealRMASumMoney = RefundAmount;
            rma.RMACount = Details.Sum(t => t.ReturnCount);
            rma.RMANo = RmaNo;
            rma.Reason = Reason;
            rma.CompensationFee = ComputeAccount();
            rma.BackDate = DateTime.Now;
            rma.StoreId = OpcRma.StoreId;
            rma.RecoverableSumMoney = RefundAmount - ComputeAccount();
            rma.RMACashStatus = EnumRMACashStatus.NoCash.AsID();
            rma.SectionId = OpcSale.SectionId;
            rma.Status = EnumRMAStatus.NoDelivery.AsID();
            //如果有赔偿的，在走财务赔偿审批
            if(RefundAmount - ComputeAccount()>0)
                rma.RMAStatus = EnumReturnGoodsStatus.CompensateVerify.AsID();
            else
                rma.RMAStatus = EnumReturnGoodsStatus.ServiceApprove.AsID();
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