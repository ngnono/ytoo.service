using Intime.OPC.Domain;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Dto.Custom;
using Intime.OPC.Domain.Enums;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Intime.OPC.Repository.Support
{
    public class RMARepository : BaseRepository<OPC_RMA>, IRMARepository
    {
        private readonly DateTime _benchTime = new DateTime(2014, 4, 22);
        public PageResult<OPC_RMA> GetByReturnGoods(ReturnGoodsInfoRequest request)
        {
            //todo 为实现
            return null;
        }

        public PageResult<RMADto> GetAll(string orderNo, string saleOrderNo, DateTime startTime, DateTime endTime,
            int? rmaStatus, EnumReturnGoodsStatus returnGoodsStatus, int pageIndex, int pageSize)
        {

            using (var db = new YintaiHZhouContext())
            {
                var query = db.OPC_RMAs.Where(t => t.CreatedDate >= startTime && t.CreatedDate < endTime);
                ;
                if (rmaStatus.HasValue && rmaStatus != -1)
                {
                    query = query.Where(t => t.Status == rmaStatus);
                }

                if (returnGoodsStatus != EnumReturnGoodsStatus.None)
                {
                    query = query.Where(t => t.RMAStatus == (int)returnGoodsStatus);
                }

                if (orderNo.IsNotNull())
                {
                    query = query.Where(t => t.OrderNo == orderNo);
                }
                if (saleOrderNo.IsNotNull())
                {
                    query = query.Where(t => t.SaleOrderNo == saleOrderNo);
                }

                var lst2 = query
                  .Join(db.Stores.Where(t => t.CreatedDate > _benchTime), t => t.StoreId, o => o.Id,
                      (t, o) => new { Rma = t, StoreName = o.Name })
                  .Join(db.OPC_Sales, t => t.Rma.SaleOrderNo, o => o.SaleOrderNo,
                      (t, o) => new {t.Rma, t.StoreName, Sale = o })
                  .Join(db.Orders, t => t.Rma.OrderNo, o => o.OrderNo,
                      (t, o) =>
                          new
                          {
                              t.Rma, t.StoreName, t.Sale,
                              payType = o.PaymentMethodName
                          }).Join(db.Sections.Where(s => s.CreateDate > _benchTime), x => x.Rma.SectionId, s => s.Id, (x, s) => new { x.Rma, x.StoreName, x.Sale, x.payType, s.SectionCode })
                  .OrderByDescending(t => t.Rma.CreatedDate);

                var lst = lst2.ToPageResult(pageIndex, pageSize);
                var lstSaleRma = lst.Result.Select(t => CreateRMADto(t)).ToList();

                return new PageResult<RMADto>(lstSaleRma, lst.TotalCount);
            }
        }

        public PageResult<RMADto> GetByRmaNo(string rmaNo)
        {
            var listRmas = new List<RMADto>();
            using (var db = new YintaiHZhouContext())
            {
                var lst2 = db.OPC_RMAs.Where(r => r.RMANo == rmaNo)
                 .Join(db.Stores.Where(t => t.CreatedDate > _benchTime), t => t.StoreId, o => o.Id,
                     (t, o) => new { Rma = t, StoreName = o.Name })
                 .Join(db.OPC_Sales, t => t.Rma.SaleOrderNo, o => o.SaleOrderNo,
                     (t, o) => new {t.Rma, t.StoreName, Sale = o })
                 .Join(db.Orders, t => t.Rma.OrderNo, o => o.OrderNo,
                     (t, o) =>
                         new
                         {
                             t.Rma, t.StoreName, t.Sale,
                             payType = o.PaymentMethodName
                         }).Join(db.Sections.Where(s => s.CreateDate > _benchTime), x => x.Rma.SectionId, s => s.Id, (x, s) => new { x.Rma, x.StoreName, x.Sale, x.payType, s.SectionCode })
                 .OrderByDescending(t => t.Rma.CreatedDate);

                var lst = lst2.ToList();

                listRmas.AddRange(lst.Select(t => CreateRMADto(t)));
            }

            return new PageResult<RMADto>(listRmas, 1);
        }

        public PageResult<RMADto> GetByPackPrintPress(string orderNo, string saleOrderNo, DateTime startTime, DateTime endTime,
            int? rmaStatus, int pageIdex, int pageSize)
        {
            using (var db = new YintaiHZhouContext())
            {
                var query =
                    db.OPC_RMAs.Where(
                        t =>
                            t.CreatedDate >= startTime && t.CreatedDate < endTime &&
                            (t.RMAStatus == (int) EnumReturnGoodsStatus.PayVerify ||
                             t.RMAStatus == (int) EnumReturnGoodsStatus.CompensateVerifyPass));
                if (rmaStatus.HasValue)
                {
                    query = query.Where(t => t.Status == rmaStatus.Value);
                }
                if (orderNo.IsNotNull())
                {
                    query = query.Where(t => t.OrderNo.Contains(orderNo));
                }
                if (saleOrderNo.IsNotNull())
                {
                    query = query.Where(t => t.SaleOrderNo.Contains(saleOrderNo));
                }

                var lst2 = query
                 .Join(db.Stores.Where(t => t.CreatedDate > _benchTime), t => t.StoreId, o => o.Id,
                     (t, o) => new { Rma = t, StoreName = o.Name })
                 .Join(db.OPC_Sales, t => t.Rma.SaleOrderNo, o => o.SaleOrderNo,
                     (t, o) => new {t.Rma, t.StoreName, Sale = o })
                 .Join(db.Orders, t => t.Rma.OrderNo, o => o.OrderNo,
                     (t, o) =>
                         new
                         {
                             t.Rma, t.StoreName, t.Sale,
                             payType = o.PaymentMethodName
                         }).Join(db.Sections.Where(s => s.CreateDate > _benchTime), x => x.Rma.SectionId, s => s.Id, (x, s) => new { x.Rma, x.StoreName, x.Sale, x.payType, s.SectionCode })
                 .OrderByDescending(t => t.Rma.CreatedDate);

                var lst = lst2.ToPageResult(pageIdex, pageSize);
                var lstSaleRma = lst.Result.Select(t => CreateRMADto(t)).ToList();
                return new PageResult<RMADto>(lstSaleRma, lst.TotalCount);
            }
        }

        public PageResult<RMADto> GetRmaReturnByExpress(string orderNo, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            /*改为包裹审核通过的*/
            using (var db = new YintaiHZhouContext())
            {
                var query = db.OPC_RMAs.Where(t => t.CreatedDate >= startTime && t.CreatedDate < endTime && t.Status == (int)EnumRMAStatus.ShipVerifyPass);

                if (orderNo.IsNotNull())
                {
                    query = query.Where(t => t.OrderNo.Contains(orderNo));
                }

                var lst2 = query
                 .Join(db.Stores.Where(t => t.CreatedDate > _benchTime), t => t.StoreId, o => o.Id,
                     (t, o) => new { Rma = t, StoreName = o.Name })
                 .Join(db.OPC_Sales, t => t.Rma.SaleOrderNo, o => o.SaleOrderNo,
                     (t, o) => new {t.Rma, t.StoreName, Sale = o })
                 .Join(db.Orders, t => t.Rma.OrderNo, o => o.OrderNo,
                     (t, o) =>
                         new
                         {
                             t.Rma, t.StoreName, t.Sale,
                             payType = o.PaymentMethodName
                         }).Join(db.Sections.Where(s => s.CreateDate > _benchTime), x => x.Rma.SectionId, s => s.Id, (x, s) => new { x.Rma, x.StoreName, x.Sale, x.payType, s.SectionCode })
                 .OrderByDescending(t => t.Rma.CreatedDate);

                var lst = lst2.ToPageResult(pageIndex, pageSize);
                var lstSaleRma = lst.Result.Select(t => CreateRMADto(t)).ToList();
                return new PageResult<RMADto>(lstSaleRma, lst.TotalCount);
            }
        }

        public PageResult<RMADto> GetRmaPrintByExpress(string orderNo, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            //（物流入库+通知单品+付款确认）
            using (var db = new YintaiHZhouContext())
            {
                var query = db.OPC_RMAs.Where(t => t.CreatedDate >= startTime && t.CreatedDate < endTime && (t.Status == (int)EnumRMAStatus.ShipInStorage || t.Status == (int)EnumRMAStatus.NotifyProduct || t.Status == (int)EnumRMAStatus.PayVerify));

                if (orderNo.IsNotNull())
                {
                    query = query.Where(t => t.OrderNo.Contains(orderNo));
                }

                var lst2 = query
                 .Join(db.Stores.Where(t => t.CreatedDate > _benchTime), t => t.StoreId, o => o.Id,
                     (t, o) => new { Rma = t, StoreName = o.Name })
                 .Join(db.OPC_Sales, t => t.Rma.SaleOrderNo, o => o.SaleOrderNo,
                     (t, o) => new { Rma = t.Rma, StoreName = t.StoreName, Sale = o })
                 .Join(db.Orders, t => t.Rma.OrderNo, o => o.OrderNo,
                     (t, o) =>
                         new
                         {
                             t.Rma, t.StoreName, t.Sale,
                             payType = o.PaymentMethodName
                         }).Join(db.Sections.Where(s => s.CreateDate > _benchTime), x => x.Rma.SectionId, s => s.Id, (x, s) => new { x.Rma, x.StoreName, x.Sale, x.payType, s.SectionCode })
                 .OrderByDescending(t => t.Rma.CreatedDate);

                var lst = lst2.ToPageResult(pageIndex, pageSize);
                var lstSaleRma = lst.Result.Select(t => CreateRMADto(t)).ToList();

                return new PageResult<RMADto>(lstSaleRma, lst.TotalCount);
            }
        }

        public PageResult<RMADto> GetRmaByShoppingGuide(string orderNo, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            CheckUser();
            using (var db = new YintaiHZhouContext())
            {
                var query = db.OPC_RMAs.Where(t => t.CreatedDate >= startTime && t.CreatedDate < endTime && t.RMAStatus == (int)EnumReturnGoodsStatus.Valid && CurrentUser.StoreIds.Contains(t.StoreId));

                if (orderNo.IsNotNull())
                {
                    query = query.Where(t => t.OrderNo.Contains(orderNo));
                }

                var lst2 = query
                 .Join(db.Stores.Where(t => t.CreatedDate > _benchTime), t => t.StoreId, o => o.Id,
                     (t, o) => new { Rma = t, StoreName = o.Name })
                 .Join(db.OPC_Sales, t => t.Rma.SaleOrderNo, o => o.SaleOrderNo,
                     (t, o) => new {t.Rma, t.StoreName, Sale = o })
                 .Join(db.Orders, t => t.Rma.OrderNo, o => o.OrderNo,
                     (t, o) =>
                         new
                         {
                             t.Rma, t.StoreName, t.Sale,
                             payType = o.PaymentMethodName
                         }).Join(db.Sections.Where(s => s.CreateDate > _benchTime), x => x.Rma.SectionId, s => s.Id, (x, s) => new { x.Rma, x.StoreName, x.Sale, x.payType, s.SectionCode })
                 .OrderByDescending(t => t.Rma.CreatedDate);

                var lst = lst2.ToPageResult(pageIndex, pageSize);
                var lstSaleRma = lst.Result.Select(t => CreateRMADto(t)).ToList();

                return new PageResult<RMADto>(lstSaleRma, lst.TotalCount);
            }
        }

        public PageResult<RMADto> GetRmaByAllOver(string orderNo, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            CheckUser();

            int rma = EnumRMAStatus.ShoppingGuideReceive.AsID();
            using (var db = new YintaiHZhouContext())
            {
                var query = db.OPC_RMAs.Where(t => t.CreatedDate >= startTime && t.CreatedDate < endTime && t.Status == (int)EnumRMAStatus.ShoppingGuideReceive && CurrentUser.StoreIds.Contains(t.StoreId));

                if (orderNo.IsNotNull())
                {
                    query = query.Where(t => t.OrderNo.Contains(orderNo));
                }

                var lst2 = query
                 .Join(db.Stores.Where(t => t.CreatedDate > _benchTime), t => t.StoreId, o => o.Id,
                     (t, o) => new { Rma = t, StoreName = o.Name })
                 .Join(db.OPC_Sales, t => t.Rma.SaleOrderNo, o => o.SaleOrderNo,
                     (t, o) => new {t.Rma, t.StoreName, Sale = o })
                 .Join(db.Orders, t => t.Rma.OrderNo, o => o.OrderNo,
                     (t, o) =>
                         new
                         {
                             t.Rma, t.StoreName, t.Sale,
                             payType = o.PaymentMethodName
                         }).Join(db.Sections.Where(s => s.CreateDate > _benchTime), x => x.Rma.SectionId, s => s.Id, (x, s) => new { x.Rma, x.StoreName, x.Sale, x.payType, s.SectionCode })
                 .OrderByDescending(t => t.Rma.CreatedDate);

                var lst = lst2.ToPageResult(pageIndex, pageSize);
                var lstSaleRma = lst.Result.Select(t => CreateRMADto(t)).ToList();

                return new PageResult<RMADto>(lstSaleRma, lst.TotalCount);
            }
        }

        private RMADto CreateRMADto(dynamic t)
        {
            var o = new RMADto
            {
                Id = t.Rma.Id,
                OrderNo = t.Rma.OrderNo,
                SaleOrderNo = t.Rma.SaleOrderNo,
                CashDate = t.Sale.CashDate,
                CashNum = t.Sale.CashNum,
                Count = t.Rma.Count,
                CreatedDate = t.Rma.CreatedDate,
                RMAAmount = t.Rma.RMAAmount,
                RMANo = t.Rma.RMANo,
                BackDate = t.Rma.BackDate,
                CompensationFee = t.Rma.CompensationFee,
                RecoverableSumMoney = t.Rma.RecoverableSumMoney,
                RMAReason = t.Rma.Reason,
                RMAType = t.Rma.RMAType,
                RefundAmount = t.Rma.RefundAmount,
                RmaCashDate = t.Rma.RmaCashDate,
                PayType = t.payType,
                RmaCashStatusName = ((EnumRMACashStatus)t.Rma.RMACashStatus).GetDescription()
            };

            var status2 = (EnumRMAStatus)(t.Rma.Status);
            o.StatusName = status2.GetDescription();
            o.SourceDesc = t.Rma.SaleRMASource;
            o.RmaStatusName = ((EnumReturnGoodsStatus)t.Rma.RMAStatus).GetDescription();
            o.StoreName = t.StoreName;
            o.SectionCode = t.SectionCode;
            return o;
        }

        public OPC_RMA GetByRmaNo2(string rmaNo)
        {
            return Select(t => t.RMANo == rmaNo).FirstOrDefault();
        }
    }
}