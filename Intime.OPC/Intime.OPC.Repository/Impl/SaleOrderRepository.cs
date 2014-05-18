using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Intime.OPC.Domain;
using Intime.OPC.Domain.BusinessModel;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Enums;
using Intime.OPC.Domain.Enums.SortOrder;
using Intime.OPC.Domain.Models;
using Intime.OPC.Domain.Partials.Models;
using Intime.OPC.Repository.Base;

namespace Intime.OPC.Repository.Impl
{
    public class SaleOrderRepository : OPCBaseRepository<int, OPC_Sale>, ISaleOrderRepository
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
                    query = query.And(v => v.Status == (int)filter.Status);
                }

                if (filter.HasDeliveryOrderGenerated)
                {
                    //已经生成发货单的
                    query = query.And(v => v.ShippingSaleId > 0);
                }
                else
                {
                    //未生成发货单的
                    query = query.And(v => v.ShippingSaleId == 0);
                }

                if (!String.IsNullOrWhiteSpace(filter.SaleOrderNo))
                    query = query.And(v => v.SaleOrderNo == filter.SaleOrderNo);

                if (!String.IsNullOrWhiteSpace(filter.OrderNo))
                    query = query.And(v => v.OrderNo == filter.OrderNo);

                if (filter.ShippingOrderId != null)
                {
                    query = query.And(v => v.ShippingSaleId == filter.ShippingOrderId);
                }

                if (filter.DateRange != null)
                {
                    if (filter.DateRange.StartDateTime != null)
                    {
                        query = query.And(v => v.CreatedDate >= filter.DateRange.StartDateTime.Value);
                    }

                    if (filter.DateRange.EndDateTime != null)
                    {
                        query = query.And(v => v.CreatedDate < filter.DateRange.EndDateTime.Value);
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
                if (filter.StoreId != null)
                    query = query.And(v => v.StoreId == filter.StoreId.Value);
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

        public List<SaleOrderModel> GetPagedList(PagerRequest pagerRequest, out int totalCount, SaleOrderFilter filter,
            SaleOrderSortOrder sortOrder)
        {
            var where = Filler(filter);
            var sectionFilter = Section4Filler(filter);

            using (var db = GetYintaiHZhouContext())
            {
                var q1 = db.OPC_Sales.Where(where)
                    .Join(db.Sections.Where(sectionFilter)
                        .Join(db.Stores, s => s.StoreId, x => x.Id, (section, store) => new { section, store }),
                        o => o.SectionId, s => s.section.Id, (o, s) => new { o, store = s })
                    .Join(db.OrderTransactions, o => o.o.OrderNo, ot => ot.OrderNo, (o, ot) => new { o, ot });
                var q = from ot in q1
                        let shipp_let = (from s in db.OPC_ShippingSales
                                         where ot.o.o.ShippingSaleId == s.Id
                                         select new
                                         {
                                             s
                                         })
                        join p in db.PaymentMethods on ot.ot.PaymentCode equals p.Code
                        select new OPC_SaleClone()
                  {
                      Id = ot.o.o.Id,
                      OrderNo = ot.o.o.OrderNo,
                      SaleOrderNo = ot.o.o.SaleOrderNo,
                      SalesType = ot.o.o.SalesType,
                      ShipViaId = ot.o.o.ShipViaId,
                      Status = ot.o.o.Status,
                      ShippingCode = ot.o.o.ShippingCode,
                      ShippingFee = ot.o.o.ShippingFee.HasValue ? ot.o.o.ShippingFee.Value : 0m,
                      ShippingStatus = ot.o.o.ShippingStatus,
                      //ShippingStatusName = ot.o.o.ShippingStatus.HasValue ? ((EnumSaleOrderStatus)ot.o.o.ShippingStatus.Value).GetDescription() : String.Empty,
                      ShippingRemark = ot.o.o.ShippingRemark,
                      SellDate = ot.ot.CreateDate,
                      IfTrans = ot.o.o.IfTrans,
                      TransStatus = ot.o.o.TransStatus,
                      SalesAmount = ot.o.o.SalesAmount,
                      SalesCount = ot.o.o.SalesCount,
                      CashStatus = ot.o.o.CashStatus,
                      CashNum = ot.o.o.CashNum,
                      CashDate = ot.o.o.CashDate,
                      SectionId = ot.o.o.SectionId,
                      PrintTimes = ot.o.o.PrintTimes,
                      Remark = ot.o.o.Remark,
                      RemarkDate = ot.o.o.RemarkDate,

                      CreatedDate = ot.o.o.CreatedDate,
                      CreatedUser = ot.o.o.CreatedUser,
                      UpdatedDate = ot.o.o.UpdatedDate,
                      UpdatedUser = ot.o.o.UpdatedUser,

                      Store = StoreClone.Convert2Store(ot.o.store.store),
                      Section = SectionClone.Convert2Section(ot.o.store.section),
                      OrderTransaction = OrderTransactionClone.Convert2OorderTransaction(ot.ot),
                      ShippingSale = OPC_ShippingSaleClone.Convert2ShippingSale(shipp_let.FirstOrDefault())
                  };

                totalCount = q.Count();

                var rst = q.OrderByDescending(v => v.CreatedDate).ThenBy(v => v.OrderNo).Skip(pagerRequest.SkipCount).Take(pagerRequest.PageSize).ToList();

                return AutoMapper.Mapper.Map<List<OPC_SaleClone>, List<SaleOrderModel>>(rst);
            }
        }
    }
}
