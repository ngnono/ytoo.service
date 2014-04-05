using System;
using System.Collections.Generic;
using System.Linq;
using Intime.OPC.Domain.Dto.Custom;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository;
using Intime.OPC.Service.Map;

namespace Intime.OPC.Service.Support
{
    public class SaleRMAService : BaseService<OPC_SaleRMA>, ISaleRMAService
    {
        private ISaleDetailRepository _saleDetailRepository;
        private ISaleRepository _saleRepository;
        private IOrderItemRepository _orderItemRepository;
        private IRmaDetailRepository _rmaDetailRepository;
        public SaleRMAService(ISaleRMARepository saleRmaRepository, ISaleDetailRepository saleDetailRepository, ISaleRepository saleRepository, IOrderItemRepository orderItemRepository, IRmaDetailRepository rmaDetailRepository) : base(saleRmaRepository)
        {
            _saleDetailRepository = saleDetailRepository;
            _saleRepository = saleRepository;
            _orderItemRepository = orderItemRepository;
            _rmaDetailRepository = rmaDetailRepository;
        }

        public void CreateSaleRMA(int userId, RMAPost rma)
        {
            var saleDetails = _saleDetailRepository.GetByOrderNo(rma.OrderNo).OrderByDescending(t=>t.SaleCount).ToList();
            var sales = _saleRepository.GetByOrderNo(rma.OrderNo, -1).OrderByDescending(t=>t.SalesCount).ToList();
            var orderItems =
                _orderItemRepository.GetByIDs(rma.ReturnProducts.Select<KeyValuePair<int, int>, int>(t => t.Key));

            IList<OPC_RMADetail> lstRmaDetails=new List<OPC_RMADetail>();//生成的退货单明细
            IList<OPC_SaleDetail> lstSaleDetails=new List<OPC_SaleDetail>();//需要退货的 销售单明细
            IDictionary<string,decimal> dicSaleOrderNoAcount=new Dictionary<string, decimal>();

            IList<RmaConfig> lstRmaConfigs=new List<RmaConfig>();
            foreach (var kv in rma.ReturnProducts)
            {
                var oItem = orderItems.FirstOrDefault(t => t.Id == kv.Key);
                var details = saleDetails.Where(t => t.StockId == kv.Key).OrderByDescending(t=>t.SaleCount).ToList();
                int returnCount = kv.Value;
                var detail = details.FirstOrDefault();
                while (returnCount>detail.SaleCount)
                {
                    var config = new RmaConfig();
                    //config.ReturnCount = detail.SaleCount;
                    //config.OpcSaleDetail = detail;
                    //config.OpcRmaDetail = CreateRmaDetail(detail, userId, detail.SaleCount);
                    config.SaleOrderNo = detail.SaleOrderNo;
                    //config.OrderDetailID = kv.Key;
                    //config.OrderItem = oItem;
                    config.OpcSale = sales.FirstOrDefault(t => t.SaleOrderNo == config.SaleOrderNo);
                    lstRmaConfigs.Add(config);

                    details.Remove(detail);
                    detail = details.FirstOrDefault();
                    returnCount = returnCount - detail.SaleCount;
                }

                if (returnCount>0)
                {
                    lstRmaDetails.Add(CreateRmaDetail(detail, userId, returnCount));
                    lstSaleDetails.Add(detail);
                    var config = new RmaConfig();
                    //config.ReturnCount = returnCount;
                    //config.OrderItem = oItem;
                    //config.OpcSaleDetail = detail;
                    //config.OpcRmaDetail = CreateRmaDetail(detail, userId, detail.SaleCount);
                    config.SaleOrderNo = detail.SaleOrderNo;
                    //config.OrderDetailID = kv.Key;
                    config.OpcSale = sales.FirstOrDefault(t => t.SaleOrderNo == config.SaleOrderNo);
                    lstRmaConfigs.Add(config);
                }
                



            }

            
           
        }

        private string CreateOpcRma(int userId, RmaConfig config)
        {
            var rma = new OPC_RMA();
            rma.UserId = userId;
            rma.CreatedDate = DateTime.Now;
            rma.CreatedUser = userId;
            rma.UpdatedDate = rma.CreatedDate;
            rma.UpdatedUser = userId;
            rma.SaleOrderNo = config.OpcSale.SaleOrderNo;
            rma.OrderNo = config.OpcSale.OrderNo;
            rma.RMANo = CreateRmaNo(rma.OrderNo);
            rma.RefundAmount = 0;
            rma.RMAAmount = config.ComputeAccount();

            //_repository.Create(rma);
            return rma.RMANo;
        }


        private void CreateOpcSaleRma(int userId,string orderNo, IList<RmaConfig> configs )
        {
            var rma = new OPC_SaleRMA();
            rma.CreatedDate = DateTime.Now;
            rma.CreatedUser = userId;
            rma.UpdatedDate = rma.CreatedDate;
            rma.UpdatedUser = userId;

            rma.OrderNo = orderNo;
            //rma.RMACount = rmaCount;
            //rma.RMANo = rmaNo;
            rma.BackDate = DateTime.Now;

        }


        /// <summary>
        /// 生成退货单明细 OPC_RMADetail
        /// </summary>
        /// <param name="detail">The detail.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="returnCount">The return count.</param>
        private OPC_RMADetail CreateRmaDetail(OPC_SaleDetail detail, int userId, int returnCount)
        {
            var opcRmaDetail = new OPC_RMADetail();
            opcRmaDetail.CreatedUser = userId;
            opcRmaDetail.CreatedDate = DateTime.Now;
            opcRmaDetail.UpdatedDate = opcRmaDetail.CreatedDate;
            opcRmaDetail.UpdatedUser = userId;
            opcRmaDetail.Price = detail.Price;
            opcRmaDetail.ProdSaleCode = detail.ProdSaleCode;
            opcRmaDetail.BackCount = detail.SaleCount;
            
            opcRmaDetail.StockId = detail.StockId;
            

            opcRmaDetail.BackCount = returnCount;
            return opcRmaDetail;
        }

        private string CreateRmaNo(string orderNo)
        {
            var count = ((ISaleRMARepository)_repository).Count(orderNo) + 1;
            return orderNo + count.ToString().PadLeft(3, '0');
        }
    }

    class RmaConfig
    {
        public RmaConfig()
        {
            Details=new List<SubRmaConfig>();
        }

        public string SaleOrderNo { get; set; }


        private IList<SubRmaConfig> Details { get; set; }

        public OPC_Sale OpcSale { get; set; }

        public decimal ComputeAccount()
        {
            decimal m = 0.0m;
            foreach (var config in Details)
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

        public OPC_RMADetail OpcRmaDetail { get; set; }

        public OPC_SaleDetail OpcSaleDetail { get; set; }

        public OrderItem OrderItem { get; set; }
    }
}