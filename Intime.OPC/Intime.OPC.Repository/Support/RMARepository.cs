using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Intime.OPC.Domain;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Dto.Custom;
using Intime.OPC.Domain.Enums;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository.Base;

namespace Intime.OPC.Repository.Support
{
    public class RMARepository : BaseRepository<OPC_RMA>, IRMARepository
    {
        public PageResult<OPC_RMA> GetByReturnGoods(ReturnGoodsInfoRequest request)
        {
            //todo 为实现
            return null;

            //using (var db = new YintaiHZhouContext())
            //{

            //}
        }

        public PageResult<RMADto> GetAll(string orderNo, string saleOrderNo, DateTime startTime, DateTime endTime,
            int? rmaStatus, string returnGoodsStatus, int pageIdex, int pageSize)
        {

            using (var db = new YintaiHZhouContext())
            {
                var query = db.OPC_RMA.Where(t => t.CreatedDate >= startTime && t.CreatedDate < endTime);
                var saleQuery = db.OPC_SaleRMA.Where(t => t.CreatedDate >= startTime && t.CreatedDate < endTime);
                if (rmaStatus.HasValue)
                {
                    saleQuery = saleQuery.Where(t => t.Status == rmaStatus);
                }

                if (returnGoodsStatus.IsNotNull())
                {
                    saleQuery = saleQuery.Where(t => t.RMAStatus == returnGoodsStatus);
                }

                if (orderNo.IsNotNull())
                {
                    query = query.Where(t => t.OrderNo.Contains(orderNo));
                }
                if (saleOrderNo.IsNotNull())
                {
                    query = query.Where(t => t.SaleOrderNo.Contains(saleOrderNo));
                }


                var lst2 = query.Join(saleQuery, o => o.RMANo,
                    t => t.RMANo, (t, o) => new {Rma = t, SaleRma = o})
                    .Join(db.Stores, t => t.Rma.StoreId, o => o.Id,
                        (t, o) => new {Rma = t.Rma, StoreName = o.Name, SaleRma = t.SaleRma})
                    .Join(db.OPC_Sale, t => t.Rma.SaleOrderNo, o => o.SaleOrderNo,
                        (t, o) => new {Rma = t.Rma, StoreName = t.StoreName, SaleRma = t.SaleRma, Sale = o})
                    .Join(db.Orders, t => t.Rma.OrderNo, o => o.OrderNo,
                        (t, o) =>
                            new
                            {
                                Rma = t.Rma,
                                StoreName = t.StoreName,
                                SaleRma = t.SaleRma,
                                Sale = t.Sale,
                                payTyp = o.PaymentMethodName
                            })
                    .OrderByDescending(t => t.Rma.CreatedDate);

                var lst = lst2.ToPageResult(pageIdex, pageSize);
                var lstSaleRma = new List<RMADto>();
                foreach (var t in lst.Result)
                {
                    var o = new RMADto();
                    o.Id = t.Rma.Id;
                    o.OrderNo = t.Rma.OrderNo;
                    o.SaleOrderNo = t.Rma.SaleOrderNo;
                    o.CashDate = t.Sale.CashDate;
                    o.CashNum = t.Sale.CashNum;
                    o.Count = t.SaleRma.RMACount;
                    o.CreatedDate = t.Rma.CreatedDate;
                    o.RMAAmount = t.Rma.RMAAmount;
                    o.RMANo = t.Rma.RMANo;
                    o.BackDate = t.SaleRma.BackDate;

                    o.RMAReason = t.SaleRma.Reason;
                    o.RMAType = t.Rma.RMAType;
                    o.RefundAmount = t.Rma.RefundAmount;
                    o.RmaCashDate = t.Rma.RmaCashDate;
                    o.PayType = t.payTyp;
                    o.RmaCashStatusName = t.SaleRma.RMACashStatus;
                    EnumRMAStatus status2 = (EnumRMAStatus) (t.SaleRma.Status);
                    o.StatusName = status2.GetDescription();
                    o.SourceDesc = t.Rma.SourceDesc;
                    o.RmaStatusName = t.SaleRma.RMAStatus;
                    o.StoreName = t.StoreName;
                    o.专柜码 = "";
                    

                    lstSaleRma.Add(o);
                }

                return new PageResult<RMADto>(lstSaleRma, lst.TotalCount);
            }
        }

        public PageResult<RMADto> GetByRmaNo(string rmaNo)
        {
            
            using (var db = new YintaiHZhouContext())
            {
                var query = db.OPC_RMA.Where(t => t.RMANo == rmaNo);

                var lst2 = query.Join(db.OPC_SaleRMA.Where(e => e.RMANo == rmaNo), o => o.RMANo, t => t.RMANo,
                    (t, o) => new {Rma = t, SaleRma = o})
                    .Join(db.Stores, t => t.Rma.StoreId, o => o.Id,
                        (t, o) => new {Rma = t.Rma, StoreName = o.Name, SaleRma = t.SaleRma})
                    .Join(db.OPC_Sale, t => t.Rma.SaleOrderNo, o => o.SaleOrderNo,
                        (t, o) => new {Rma = t.Rma, StoreName = t.StoreName, SaleRma = t.SaleRma, Sale = o})
                    .Join(db.Orders, t => t.Rma.OrderNo, o => o.OrderNo,
                        (t, o) =>
                            new
                            {
                                Rma = t.Rma,
                                StoreName = t.StoreName,
                                SaleRma = t.SaleRma,
                                Sale = t.Sale,
                                payTyp = o.PaymentMethodName
                            })
                    .OrderByDescending(t => t.Rma.CreatedDate);

                var lst = lst2.ToList();
                var lstSaleRma = new List<RMADto>();
                foreach (var t in lst)
                {
                    var o = new RMADto();
                    o.Id = t.Rma.Id;
                    o.OrderNo = t.Rma.OrderNo;
                    o.SaleOrderNo = t.Rma.SaleOrderNo;
                    o.CashDate = t.Sale.CashDate;
                    o.CashNum = t.Sale.CashNum;
                    o.Count = t.SaleRma.RMACount;
                    o.CreatedDate = t.Rma.CreatedDate;
                    o.RMAAmount = t.Rma.RMAAmount;
                    o.RMANo = t.Rma.RMANo;
                    o.BackDate = t.SaleRma.BackDate;

                    o.RMAReason = t.SaleRma.Reason;
                    o.RMAType = t.Rma.RMAType;
                    o.RefundAmount = t.Rma.RefundAmount;
                    o.RmaCashDate = t.Rma.RmaCashDate;
                    o.PayType = t.payTyp;
                    o.RmaCashStatusName = t.SaleRma.RMACashStatus;
                    EnumRMAStatus status2 = (EnumRMAStatus) (t.Rma.Status);
                    o.StatusName = status2.GetDescription();
                    o.SourceDesc = t.Rma.SourceDesc;
                    o.RmaStatusName = t.SaleRma.RMAStatus;
                    o.StoreName = t.StoreName;
                    o.专柜码 = "";
                   
                   

                    lstSaleRma.Add(o);
                }

                return new PageResult<RMADto>(lstSaleRma, 1);
            }
        }

        public PageResult<RMADto> GetByPackPrintPress(string orderNo, string saleOrderNo, DateTime startTime, DateTime endTime,
            int? rmaStatus,  int pageIdex, int pageSize)
        {
       
            string[] lstDes = { EnumReturnGoodsStatus.PayVerify.GetDescription(), EnumReturnGoodsStatus.CompensateVerifyPass.GetDescription() };
           ;
            using (var db = new YintaiHZhouContext())
            {
                var query = db.OPC_RMA.Where(t => t.CreatedDate >= startTime && t.CreatedDate < endTime);
                var saleQuery = db.OPC_SaleRMA.Where(t => t.CreatedDate >= startTime && t.CreatedDate < endTime && lstDes.Contains(t.RMAStatus));
                if (rmaStatus.HasValue)
                {
                    saleQuery = saleQuery.Where(t => t.Status == rmaStatus);
                }
                if (orderNo.IsNotNull())
                {
                    query = query.Where(t => t.OrderNo.Contains(orderNo));
                }
                if (saleOrderNo.IsNotNull())
                {
                    query = query.Where(t => t.SaleOrderNo.Contains(saleOrderNo));
                }


                var lst2 = query.Join(saleQuery, o => o.RMANo,
                    t => t.RMANo, (t, o) => new { Rma = t, SaleRma = o })
                    .Join(db.Stores, t => t.Rma.StoreId, o => o.Id,
                        (t, o) => new { Rma = t.Rma, StoreName = o.Name, SaleRma = t.SaleRma })
                    .Join(db.OPC_Sale, t => t.Rma.SaleOrderNo, o => o.SaleOrderNo,
                        (t, o) => new { Rma = t.Rma, StoreName = t.StoreName, SaleRma = t.SaleRma, Sale = o })
                    .Join(db.Orders, t => t.Rma.OrderNo, o => o.OrderNo,
                        (t, o) =>
                            new
                            {
                                Rma = t.Rma,
                                StoreName = t.StoreName,
                                SaleRma = t.SaleRma,
                                Sale = t.Sale,
                                payTyp = o.PaymentMethodName
                            })
                    .OrderByDescending(t => t.Rma.CreatedDate);

                var lst = lst2.ToPageResult(pageIdex, pageSize);
                var lstSaleRma = new List<RMADto>();
                foreach (var t in lst.Result)
                {
                    var o = new RMADto();
                    o.Id = t.Rma.Id;
                    o.OrderNo = t.Rma.OrderNo;
                    o.SaleOrderNo = t.Rma.SaleOrderNo;
                    o.CashDate = t.Sale.CashDate;
                    o.CashNum = t.Sale.CashNum;
                    o.Count = t.SaleRma.RMACount;
                    o.CreatedDate = t.Rma.CreatedDate;
                    o.RMAAmount = t.Rma.RMAAmount;
                    o.RMANo = t.Rma.RMANo;
                    o.BackDate = t.SaleRma.BackDate;

                    o.RMAReason = t.SaleRma.Reason;
                    o.RMAType = t.Rma.RMAType;
                    o.RefundAmount = t.Rma.RefundAmount;
                    o.RmaCashDate = t.Rma.RmaCashDate;
                    o.RmaCashNum = t.Rma.RmaCashNum;
                    o.PayType = t.payTyp;
                    o.RmaCashStatusName = t.SaleRma.RMACashStatus;
                    EnumRMAStatus status2 = (EnumRMAStatus)(t.Rma.Status);
                    o.StatusName = status2.GetDescription();
                    o.SourceDesc = t.Rma.SourceDesc;
                    o.RmaStatusName = t.SaleRma.RMAStatus;
                    o.StoreName = t.StoreName;
                    o.专柜码 = "";


                    lstSaleRma.Add(o);
                }

                return new PageResult<RMADto>(lstSaleRma, lst.TotalCount);
            }
        }

        public PageResult<RMADto> GetRmaReturnByExpress(string orderNo, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            /*改为包裹审核通过的*/
            int stat = EnumRMAStatus.ShipVerifyPass.AsID();
            //var rmaCashStatus = EnumCashStatus.CashOver.GetDescription();
            using (var db = new YintaiHZhouContext())
            {
                var query = db.OPC_RMA.Where(t => t.CreatedDate >= startTime && t.CreatedDate < endTime);
                var saleQuery =
                    db.OPC_SaleRMA.Where(
                        t => t.CreatedDate >= startTime && t.CreatedDate < endTime  && t.Status==stat);
                
                if (orderNo.IsNotNull())
                {
                    query = query.Where(t => t.OrderNo.Contains(orderNo));
                }
                


                var lst2 = query.Join(saleQuery, o => o.RMANo,
                    t => t.RMANo, (t, o) => new {Rma = t, SaleRma = o})
                    .Join(db.Stores, t => t.Rma.StoreId, o => o.Id,
                        (t, o) => new {Rma = t.Rma, StoreName = o.Name, SaleRma = t.SaleRma})
                    .Join(db.OPC_Sale, t => t.Rma.SaleOrderNo, o => o.SaleOrderNo,
                        (t, o) => new {Rma = t.Rma, StoreName = t.StoreName, SaleRma = t.SaleRma, Sale = o})
                    .Join(db.Orders, t => t.Rma.OrderNo, o => o.OrderNo,
                        (t, o) =>
                            new
                            {
                                Rma = t.Rma,
                                StoreName = t.StoreName,
                                SaleRma = t.SaleRma,
                                Sale = t.Sale,
                                payTyp = o.PaymentMethodName
                            })
                    .OrderByDescending(t => t.Rma.CreatedDate);

                var lst = lst2.ToPageResult(pageIndex, pageSize);
                var lstSaleRma = new List<RMADto>();
                foreach (var t in lst.Result)
                {
                    var o = new RMADto();
                    o.Id = t.Rma.Id;
                    o.OrderNo = t.Rma.OrderNo;
                    o.SaleOrderNo = t.Rma.SaleOrderNo;
                    o.CashDate = t.Sale.CashDate;
                    o.CashNum = t.Sale.CashNum;
                    o.Count = t.SaleRma.RMACount;
                    o.CreatedDate = t.Rma.CreatedDate;
                    o.RMAAmount = t.Rma.RMAAmount;
                    o.RMANo = t.Rma.RMANo;
                    o.BackDate = t.SaleRma.BackDate;

                    o.RMAReason = t.SaleRma.Reason;
                    o.RMAType = t.Rma.RMAType;
                    o.RefundAmount = t.Rma.RefundAmount;
                    o.RmaCashDate = t.Rma.RmaCashDate;
                    o.RmaCashNum = t.Rma.RmaCashNum;
                    o.PayType = t.payTyp;
                    o.RmaCashStatusName = t.SaleRma.RMACashStatus;
                    EnumRMAStatus status2 = (EnumRMAStatus) (t.Rma.Status);
                    o.StatusName = status2.GetDescription();
                    o.SourceDesc = t.Rma.SourceDesc;
                    o.RmaStatusName = t.SaleRma.RMAStatus;
                    o.StoreName = t.StoreName;
                    o.专柜码 = "";


                    lstSaleRma.Add(o);
                }

                return new PageResult<RMADto>(lstSaleRma, lst.TotalCount);
            }
        }

        public PageResult<RMADto> GetRmaPrintByExpress(string orderNo, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            int stat = EnumRMAStatus.ShipInStorage.AsID();
            var rmaCashStatus = EnumCashStatus.CashOver.GetDescription();
            string rmaStatus = EnumReturnGoodsStatus.Valid.GetDescription();
            using (var db = new YintaiHZhouContext())
            {
                var query = db.OPC_RMA.Where(t => t.CreatedDate >= startTime && t.CreatedDate < endTime);
                var saleQuery =
                    db.OPC_SaleRMA.Where(
                        t => t.CreatedDate >= startTime && t.CreatedDate < endTime && t.RMACashStatus == rmaCashStatus && t.Status == stat && t.RMAStatus == rmaStatus);

                if (orderNo.IsNotNull())
                {
                    query = query.Where(t => t.OrderNo.Contains(orderNo));
                }

                var lst2 = query.Join(saleQuery, o => o.RMANo,
                    t => t.RMANo, (t, o) => new { Rma = t, SaleRma = o })
                    .Join(db.Stores, t => t.Rma.StoreId, o => o.Id,
                        (t, o) => new { Rma = t.Rma, StoreName = o.Name, SaleRma = t.SaleRma })
                    .Join(db.OPC_Sale, t => t.Rma.SaleOrderNo, o => o.SaleOrderNo,
                        (t, o) => new { Rma = t.Rma, StoreName = t.StoreName, SaleRma = t.SaleRma, Sale = o })
                    .Join(db.Orders, t => t.Rma.OrderNo, o => o.OrderNo,
                        (t, o) =>
                            new
                            {
                                Rma = t.Rma,
                                StoreName = t.StoreName,
                                SaleRma = t.SaleRma,
                                Sale = t.Sale,
                                payTyp = o.PaymentMethodName
                            })
                    .OrderByDescending(t => t.Rma.CreatedDate);

                var lst = lst2.ToPageResult(pageIndex, pageSize);
                var lstSaleRma = new List<RMADto>();
                foreach (var t in lst.Result)
                {
                    var o = new RMADto();
                    o.Id = t.Rma.Id;
                    o.OrderNo = t.Rma.OrderNo;
                    o.SaleOrderNo = t.Rma.SaleOrderNo;
                    o.CashDate = t.Sale.CashDate;
                    o.CashNum = t.Sale.CashNum;
                    o.Count = t.SaleRma.RMACount;
                    o.CreatedDate = t.Rma.CreatedDate;
                    o.RMAAmount = t.Rma.RMAAmount;
                    o.RMANo = t.Rma.RMANo;
                    o.BackDate = t.SaleRma.BackDate;

                    o.RMAReason = t.SaleRma.Reason;
                    o.RMAType = t.Rma.RMAType;
                    o.RefundAmount = t.Rma.RefundAmount;
                    o.RmaCashDate = t.Rma.RmaCashDate;
                    o.RmaCashNum = t.Rma.RmaCashNum;
                    o.PayType = t.payTyp;
                    o.RmaCashStatusName = t.SaleRma.RMACashStatus;
                    EnumRMAStatus status2 = (EnumRMAStatus)(t.Rma.Status);
                    o.StatusName = status2.GetDescription();
                    o.SourceDesc = t.Rma.SourceDesc;
                    o.RmaStatusName = t.SaleRma.RMAStatus;
                    o.StoreName = t.StoreName;
                    o.专柜码 = "";


                    lstSaleRma.Add(o);
                }

                return new PageResult<RMADto>(lstSaleRma, lst.TotalCount);
            }
        }

        public PageResult<RMADto> GetRmaByShoppingGuide(string orderNo, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            CheckUser();
            var lstSection = CurrentUser.SectionID;
            string rmaStatus = EnumReturnGoodsStatus.Valid.GetDescription();
            using (var db = new YintaiHZhouContext())
            {
                var query = db.OPC_RMA.Where(t => t.CreatedDate >= startTime && t.CreatedDate < endTime && CurrentUser.StoreIDs.Contains(t.StoreId));
                var saleQuery =
                    db.OPC_SaleRMA.Where(
                        t => t.CreatedDate >= startTime && t.CreatedDate < endTime && t.RMAStatus == rmaStatus && CurrentUser.StoreIDs.Contains(t.StoreId));

                if (orderNo.IsNotNull())
                {
                    query = query.Where(t => t.OrderNo.Contains(orderNo));
                }

                var lst2 = query.Join(saleQuery, o => o.RMANo,
                    t => t.RMANo, (t, o) => new { Rma = t, SaleRma = o })
                    .Join(db.Stores, t => t.Rma.StoreId, o => o.Id,
                        (t, o) => new { Rma = t.Rma, StoreName = o.Name, SaleRma = t.SaleRma })
                    .Join(db.OPC_Sale, t => t.Rma.SaleOrderNo, o => o.SaleOrderNo,
                        (t, o) => new { Rma = t.Rma, StoreName = t.StoreName, SaleRma = t.SaleRma, Sale = o })
                    .Join(db.Orders, t => t.Rma.OrderNo, o => o.OrderNo,
                        (t, o) =>
                            new
                            {
                                Rma = t.Rma,
                                StoreName = t.StoreName,
                                SaleRma = t.SaleRma,
                                Sale = t.Sale,
                                payTyp = o.PaymentMethodName
                            })
                    .OrderByDescending(t => t.Rma.CreatedDate);

                var lst = lst2.ToPageResult(pageIndex, pageSize);
                var lstSaleRma = new List<RMADto>();
                foreach (var t in lst.Result)
                {
                    var o = new RMADto();
                    o.Id = t.Rma.Id;
                    o.OrderNo = t.Rma.OrderNo;
                    o.SaleOrderNo = t.Rma.SaleOrderNo;
                    o.CashDate = t.Sale.CashDate;
                    o.CashNum = t.Sale.CashNum;
                    o.Count = t.SaleRma.RMACount;
                    o.CreatedDate = t.Rma.CreatedDate;
                    o.RMAAmount = t.Rma.RMAAmount;
                    o.RMANo = t.Rma.RMANo;
                    o.BackDate = t.SaleRma.BackDate;

                    o.RMAReason = t.SaleRma.Reason;
                    o.RMAType = t.Rma.RMAType;
                    o.RefundAmount = t.Rma.RefundAmount;
                    o.RmaCashDate = t.Rma.RmaCashDate;
                    o.RmaCashNum = t.Rma.RmaCashNum;
                    o.PayType = t.payTyp;
                    o.RmaCashStatusName = t.SaleRma.RMACashStatus;
                    EnumRMAStatus status2 = (EnumRMAStatus)(t.Rma.Status);
                    o.StatusName = status2.GetDescription();
                    o.SourceDesc = t.Rma.SourceDesc;
                    o.RmaStatusName = t.SaleRma.RMAStatus;
                    o.StoreName = t.StoreName;
                    o.专柜码 = "";


                    lstSaleRma.Add(o);
                }

                return new PageResult<RMADto>(lstSaleRma, lst.TotalCount);
            }
        }

        public PageResult<RMADto> GetRmaByAllOver(string orderNo, DateTime startTime, DateTime endTime, int pageIndex, int pageSize)
        {
            CheckUser();
            //var lstSection = CurrentUser.SectionIDs;
           
            int rma = EnumRMAStatus.ShoppingGuideReceive.AsID();
            using (var db = new YintaiHZhouContext())
            {
                var query = db.OPC_RMA.Where(t => t.CreatedDate >= startTime && t.CreatedDate < endTime && CurrentUser.StoreIDs.Contains(t.StoreId));
                var saleQuery =
                    db.OPC_SaleRMA.Where(
                        t => t.CreatedDate >= startTime && t.CreatedDate < endTime && t.Status==rma && CurrentUser.StoreIDs.Contains(t.StoreId));

                if (orderNo.IsNotNull())
                {
                    query = query.Where(t => t.OrderNo.Contains(orderNo));
                }

                var lst2 = query.Join(saleQuery, o => o.RMANo,
                    t => t.RMANo, (t, o) => new { Rma = t, SaleRma = o })
                    .Join(db.Stores, t => t.Rma.StoreId, o => o.Id,
                        (t, o) => new { Rma = t.Rma, StoreName = o.Name, SaleRma = t.SaleRma })
                    .Join(db.OPC_Sale, t => t.Rma.SaleOrderNo, o => o.SaleOrderNo,
                        (t, o) => new { Rma = t.Rma, StoreName = t.StoreName, SaleRma = t.SaleRma, Sale = o })
                    .Join(db.Orders, t => t.Rma.OrderNo, o => o.OrderNo,
                        (t, o) =>
                            new
                            {
                                Rma = t.Rma,
                                StoreName = t.StoreName,
                                SaleRma = t.SaleRma,
                                Sale = t.Sale,
                                payTyp = o.PaymentMethodName
                            })
                    .OrderByDescending(t => t.Rma.CreatedDate);

                var lst = lst2.ToPageResult(pageIndex, pageSize);
                var lstSaleRma = new List<RMADto>();
                foreach (var t in lst.Result)
                {
                    var o = new RMADto();
                    o.Id = t.Rma.Id;
                    o.OrderNo = t.Rma.OrderNo;
                    o.SaleOrderNo = t.Rma.SaleOrderNo;
                    o.CashDate = t.Sale.CashDate;
                    o.CashNum = t.Sale.CashNum;
                    o.Count = t.SaleRma.RMACount;
                    o.CreatedDate = t.Rma.CreatedDate;
                    o.RMAAmount = t.Rma.RMAAmount;
                    o.RMANo = t.Rma.RMANo;
                    o.BackDate = t.SaleRma.BackDate;

                    o.RMAReason = t.SaleRma.Reason;
                    o.RMAType = t.Rma.RMAType;
                    o.RefundAmount = t.Rma.RefundAmount;
                    o.RmaCashDate = t.Rma.RmaCashDate;
                    o.RmaCashNum = t.Rma.RmaCashNum;
                    o.PayType = t.payTyp;
                    o.RmaCashStatusName = t.SaleRma.RMACashStatus;
                    EnumRMAStatus status2 = (EnumRMAStatus)(t.Rma.Status);
                    o.StatusName = status2.GetDescription();
                    o.SourceDesc = t.Rma.SourceDesc;
                    o.RmaStatusName = t.SaleRma.RMAStatus;
                    o.StoreName = t.StoreName;
                    o.专柜码 = "";


                    lstSaleRma.Add(o);
                }

                return new PageResult<RMADto>(lstSaleRma, lst.TotalCount);
            }
        }

        public OPC_RMA GetByRmaNo2(string rmaNo)
        {
            return Select(t => t.RMANo == rmaNo).FirstOrDefault();
        }
    }
}