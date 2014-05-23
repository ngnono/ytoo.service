using Intime.OPC.Domain;
using Intime.OPC.Domain.BusinessModel;
using Intime.OPC.Domain.Enums.SortOrder;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository.Base;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using PredicateBuilder = LinqKit.PredicateBuilder;

namespace Intime.OPC.Repository.Impl
{
    public class SalesOrderRepository : OPCBaseRepository<int, OPC_Sale>, ISaleOrderRepository
    {
        #region methods

        /// <summary>
        /// 销售单 筛选
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        private static Expression<Func<OPC_Sale, bool>> Filler(SaleOrderFilter filter)
        {
            var query = PredicateBuilder.True<OPC_Sale>();

            if (filter != null)
            {

                if (filter.Status != null)
                {
                    query = PredicateBuilder.And(query, v => v.Status == (int)filter.Status);
                }

                //if (filter.if != null)
                //{
                //    if (filter.HasDeliveryOrderGenerated.Value)
                //    {
                //        //已经生成发货单的
                //        query = PredicateBuilder.And(query, v => v.ShippingSaleId > 0);
                //    }
                //    else
                //    {
                //        //未生成发货单的
                //        query = PredicateBuilder.And(query, v => (!v.ShippingSaleId.HasValue) || v.ShippingSaleId < 1);
                //    }
                //}

                if (!String.IsNullOrWhiteSpace(filter.SalesOrderNo))
                    query = PredicateBuilder.And(query, v => v.SaleOrderNo == filter.SalesOrderNo);

                if (!String.IsNullOrWhiteSpace(filter.OrderNo))
                    query = PredicateBuilder.And(query, v => v.OrderNo == filter.OrderNo);

                if (filter.ShippingOrderId != null)
                {
                    query = PredicateBuilder.And(query, v => v.ShippingSaleId == filter.ShippingOrderId);
                }

                if (filter.DateRange != null)
                {
                    if (filter.DateRange.StartDateTime != null)
                    {
                        query = PredicateBuilder.And(query, v => v.CreatedDate >= filter.DateRange.StartDateTime.Value);
                    }

                    if (filter.DateRange.EndDateTime != null)
                    {
                        query = PredicateBuilder.And(query, v => v.CreatedDate < filter.DateRange.EndDateTime.Value);
                    }
                }
            }

            return query;
        }

        /// <summary>
        /// 专柜 筛选
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        private static Expression<Func<Section, bool>> Section4Filler(SaleOrderFilter filter)
        {
            var query = PredicateBuilder.True<Section>();

            if (filter != null)
            {
                if (!filter.IsAllStoreIds)
                {
                    if (filter.StoreIds.Count == 1)
                    {
                        var t = filter.StoreIds[0];
                        query = PredicateBuilder.And(query, v => v.StoreId.Value == t);
                    }
                    else
                    {
                        query = PredicateBuilder.And(query, v => filter.StoreIds.Contains(v.StoreId.Value));
                    }

                }

            }

            return query;
        }


        private static Func<IQueryable<OPC_Sale>, IOrderedQueryable<OPC_Sale>> OrderBy(SaleOrderSortOrder sortOrder)
        {
            Func<IQueryable<OPC_Sale>, IOrderedQueryable<OPC_Sale>> orderBy = null;

            switch (sortOrder)
            {
                default:
                    orderBy = v => v.OrderByDescending(s => s.CreatedDate).ThenBy(s => s.OrderNo);
                    break;
            }

            return orderBy;
        }
        #endregion

        public override IEnumerable<OPC_Sale> AutoComplete(string query)
        {
            throw new NotImplementedException();
        }

        public List<SalesOrderModel> GetPagedList(PagerRequest pagerRequest, out int totalCount, SaleOrderFilter filter,
            SaleOrderSortOrder sortOrder)
        {
            var where = Filler(filter);
            var sectionFilter = Section4Filler(filter);

            using (var db = GetYintaiHZhouContext())
            {
                var q1 = db.OPC_Sales.AsExpandable().Where(where)
                    .Join(db.Sections.AsExpandable().Where(sectionFilter)
                        .Join(db.Stores, s => s.StoreId, x => x.Id, (section, store) => new { section, store }),
                        o => o.SectionId, s => s.section.Id, (o, s) => new { o, store = s })
                    .Join(db.OrderTransactions, o => o.o.OrderNo, ot => ot.OrderNo, (o, ot) => new { o, ot });
                var q = from ot in q1
                        //join ss in db.OPC_ShippingSales on ot.o.o.ShippingSaleId equals ss.Id into tmp1
                        //from ss in tmp1.DefaultIfEmpty()
                        join p in db.PaymentMethods on ot.ot.PaymentCode equals p.Code
                        join oo in db.Orders on ot.o.o.OrderNo equals oo.OrderNo
                        select new //SalesOrderModel
                  {
                      ot,
                      oo
                  };

                totalCount = q.Count();

                var rst = q.OrderByDescending(v => v.ot.o.o.CreatedDate).ThenBy(v => v.ot.o.o.OrderNo).Skip(pagerRequest.SkipCount).Take(pagerRequest.PageSize)
                    .Select(v =>

                        new SalesOrderModel
                  {
                      Id = v.ot.o.o.Id,
                      OrderNo = v.ot.o.o.OrderNo,
                      SaleOrderNo = v.ot.o.o.SaleOrderNo,
                      SalesType = v.ot.o.o.SalesType,
                      ShipViaId = v.ot.o.o.ShipViaId,
                      Status = v.ot.o.o.Status,
                      ShippingCode = v.ot.o.o.ShippingCode,
                      ShippingFee = v.ot.o.o.ShippingFee.HasValue ? v.ot.o.o.ShippingFee.Value : 0m,
                      ShippingStatus = v.ot.o.o.ShippingStatus,
                      //ShippingStatusName = v.ot.o.o.ShippingStatus.HasValue ? ((EnumSaleOrderStatus)v.ot.o.o.ShippingStatus.Value).GetDescription() : String.Empty,
                      ShippingRemark = v.ot.o.o.ShippingRemark,
                      SellDate = v.ot.ot.CreateDate,
                      IfTrans = v.ot.o.o.IfTrans,
                      TransStatus = v.ot.o.o.TransStatus,
                      SalesAmount = v.ot.o.o.SalesAmount,
                      SalesCount = v.ot.o.o.SalesCount,
                      CashStatus = v.ot.o.o.CashStatus,
                      CashNum = v.ot.o.o.CashNum,
                      CashDate = v.ot.o.o.CashDate,
                      //SectionId = v.ot.o.o.SectionId??0,
                      PrintTimes = v.ot.o.o.PrintTimes ?? 0,
                      Remark = v.ot.o.o.Remark,
                      RemarkDate = v.ot.o.o.RemarkDate,
                      //store
                      StoreId = v.ot.o.store.store.Id,
                      StoreName = v.ot.o.store.store.Name,
                      //section
                      SectionId = v.ot.o.store.section.Id,
                      SectionName = v.ot.o.store.section.Name,
                      SectionCode = v.ot.o.store.section.SectionCode,

                      CreatedDate = v.ot.o.o.CreatedDate,
                      CreatedUser = v.ot.o.o.CreatedUser,
                      UpdatedDate = v.ot.o.o.UpdatedDate,
                      UpdatedUser = v.ot.o.o.UpdatedUser ?? 0,
                      //ot.ot
                      //ShippingSale
                      //ShipViaId = shipp_let.FirstOrDefault()
                      //ShipViaName = ss!=null? ss.ShipViaName:String.Empty,
                      //ShippingCode = ss!=null? ss.ShippingCode:String.Empty,
                      //ShippingFee = ss!=null? ss.ShippingFee:String.Empty,
                      //ShippingStatus == ss!=null? ss.ShippingStatus:String.Empty,

                      //Order
                      CustomerName = v.oo.ShippingContactPerson,
                      ReceivePerson = v.oo.ShippingContactPerson,
                      CustomerPhone = v.oo.ShippingContactPhone,
                      //CustomerAddress = v.oo.ShippingAddress,
                      //CustomerRemark = v.oo.Memo,
                      Invoice = v.oo.NeedInvoice,
                      InvoiceSubject = v.oo.InvoiceSubject,
                      //IfReceipt = v.oo.NeedInvoice,
                      //ReceiptHead = v.oo.InvoiceSubject,
                      //ReceiptContent = v.oo.InvoiceDetail,

                      ////OT
                      TransNo = v.ot.ot.TransNo,

                      ShippingSaleId = v.ot.o.o.ShippingSaleId


                  }).ToList();

                return rst;
                //return AutoMapper.Mapper.Map<List<OPC_SaleClone>, List<OPC_Sale>>(rst);
            }
        }

        public List<OPC_Sale> GetListByNos(List<string> salesOrderNos, SaleOrderFilter filter)
        {
            var query = Filler(filter);

            query = PredicateBuilder.And(query, v => salesOrderNos.Contains(v.SaleOrderNo));
            return Func(db => EFHelper.Get(db, query).ToList());
        }


        public SalesOrderModel GetItemModel(string salesorderno)
        {
            using (var db = GetYintaiHZhouContext())
            {
                var q1 = db.OPC_Sales.Where(v => v.SaleOrderNo == salesorderno)
                    .Join(db.Sections
                        .Join(db.Stores, s => s.StoreId, x => x.Id, (section, store) => new { section, store }),
                        o => o.SectionId, s => s.section.Id, (o, s) => new { o, store = s })
                    .Join(db.OrderTransactions, o => o.o.OrderNo, ot => ot.OrderNo, (o, ot) => new { o, ot });
                var q = from ot in q1
                        //join ss in db.OPC_ShippingSales on ot.o.o.ShippingSaleId equals ss.Id into tmp1
                        //from ss in tmp1.DefaultIfEmpty()
                        join p in db.PaymentMethods on ot.ot.PaymentCode equals p.Code
                        join oo in db.Orders on ot.o.o.OrderNo equals oo.OrderNo
                        select new //SalesOrderModel
                        {
                            ot,
                            oo
                        };

                var rst = q
                    .Select(v =>

                        new SalesOrderModel
                        {
                            Id = v.ot.o.o.Id,
                            OrderNo = v.ot.o.o.OrderNo,
                            SaleOrderNo = v.ot.o.o.SaleOrderNo,
                            SalesType = v.ot.o.o.SalesType,
                            ShipViaId = v.ot.o.o.ShipViaId,
                            Status = v.ot.o.o.Status,
                            ShippingCode = v.ot.o.o.ShippingCode,
                            ShippingFee = v.ot.o.o.ShippingFee.HasValue ? v.ot.o.o.ShippingFee.Value : 0m,
                            ShippingStatus = v.ot.o.o.ShippingStatus,
                            //ShippingStatusName = v.ot.o.o.ShippingStatus.HasValue ? ((EnumSaleOrderStatus)v.ot.o.o.ShippingStatus.Value).GetDescription() : String.Empty,
                            ShippingRemark = v.ot.o.o.ShippingRemark,
                            SellDate = v.ot.ot.CreateDate,
                            IfTrans = v.ot.o.o.IfTrans,
                            TransStatus = v.ot.o.o.TransStatus,
                            SalesAmount = v.ot.o.o.SalesAmount,
                            SalesCount = v.ot.o.o.SalesCount,
                            CashStatus = v.ot.o.o.CashStatus,
                            CashNum = v.ot.o.o.CashNum,
                            CashDate = v.ot.o.o.CashDate,
                            //SectionId = v.ot.o.o.SectionId??0,
                            PrintTimes = v.ot.o.o.PrintTimes ?? 0,
                            Remark = v.ot.o.o.Remark,
                            RemarkDate = v.ot.o.o.RemarkDate,
                            //store
                            StoreId = v.ot.o.store.store.Id,
                            StoreName = v.ot.o.store.store.Name,
                            //section
                            SectionId = v.ot.o.store.section.Id,
                            SectionName = v.ot.o.store.section.Name,
                            SectionCode = v.ot.o.store.section.SectionCode,

                            CreatedDate = v.ot.o.o.CreatedDate,
                            CreatedUser = v.ot.o.o.CreatedUser,
                            UpdatedDate = v.ot.o.o.UpdatedDate,
                            UpdatedUser = v.ot.o.o.UpdatedUser ?? 0,
                            //ot.ot
                            //ShippingSale
                            //ShipViaId = shipp_let.FirstOrDefault()
                            //ShipViaName = ss!=null? ss.ShipViaName:String.Empty,
                            //ShippingCode = ss!=null? ss.ShippingCode:String.Empty,
                            //ShippingFee = ss!=null? ss.ShippingFee:String.Empty,
                            //ShippingStatus == ss!=null? ss.ShippingStatus:String.Empty,

                            //Order
                            CustomerName = v.oo.ShippingContactPerson,
                            ReceivePerson = v.oo.ShippingContactPerson,
                            CustomerPhone = v.oo.ShippingContactPhone,
                            //CustomerAddress = v.oo.ShippingAddress,
                            //CustomerRemark = v.oo.Memo,
                            Invoice = v.oo.NeedInvoice,
                            InvoiceSubject = v.oo.InvoiceSubject,
                            //IfReceipt = v.oo.NeedInvoice,
                            //ReceiptHead = v.oo.InvoiceSubject,
                            //ReceiptContent = v.oo.InvoiceDetail,

                            ////OT
                            TransNo = v.ot.ot.TransNo,

                            ShippingSaleId = v.ot.o.o.ShippingSaleId
                        }).FirstOrDefault();

                return rst;
                //return AutoMapper.Mapper.Map<List<OPC_SaleClone>, List<OPC_Sale>>(rst);
            }
        }
    }
}
