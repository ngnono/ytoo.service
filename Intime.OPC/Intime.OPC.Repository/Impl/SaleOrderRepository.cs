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
using Intime.OPC.Repository.Base;

namespace Intime.OPC.Repository.Impl
{
    public class SaleOrderRepository : OPCBaseRepository<int, OPC_Sale>, ISaleOrderRepository
    {
        #region methods

        private static Expression<Func<OPC_Sale, bool>> Filler(SaleOrderFilter filter)
        {
            var query = PredicateBuilder.True<OPC_Sale>();

            if (filter != null)
            {
                if (!String.IsNullOrWhiteSpace(filter.SaleOrderNo))
                    query = query.And(v => v.SaleOrderNo == filter.SaleOrderNo);

                if (!String.IsNullOrWhiteSpace(filter.OrderNo))
                    query = query.And(v => v.OrderNo == filter.OrderNo);

                if (filter.Status != null)
                {
                    query = query.And(v => v.Status == filter.Status.Value);
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
                    //orderBy = v => v.OrderByDescending(s => s);
                    break;
            }

            return orderBy;
        }

        #endregion

        public override IEnumerable<OPC_Sale> AutoComplete(string query)
        {
            throw new NotImplementedException();
        }

        public List<OPC_Sale> GetPagedList(PagerRequest pagerRequest, out int totalCount, SaleOrderFilter filter,
            SaleOrderSortOrder sortOrder)
        {
            var where = Filler(filter);

            using (var db = GetYintaiHZhouContext())
            {
                var q1 = db.OPC_Sales.Where(where)
                    .Join(db.Sections.Where(Section4Filler(filter))
                        .Join(db.Stores, s => s.StoreId, x => x.Id, (section, store) => new {section, store}),
                        o => o.SectionId, s => s.section.Id, (o, s) => new {o, store = s});

                    q1.Join(db.OrderTransactions, o => o.o.OrderNo, ot => ot.OrderNo, (o, ot) => new { o, ot })
                    .Join(db.PaymentMethods, ot => ot.ot.PaymentCode, pm => pm.Code, (ot, pm) => new SaleDto()
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
                        ShippingStatusName = ot.o.o.ShippingStatus.HasValue ? ((EnumSaleOrderStatus)ot.o.o.ShippingStatus.Value).GetDescription() : String.Empty,
                        ShippingRemark = ot.o.o.ShippingRemark,
                        SellDate = ot.ot.CreateDate,
                        IfTrans = ot.o.o.IfTrans.HasValue && ot.o.o.IfTrans.Value ? "是" : "否",
                        TransStatus = ot.o.o.TransStatus.HasValue && ot.o.o.TransStatus.Value == 1 ? "调拨" : String.Empty,
                        SalesAmount = ot.o.o.SalesAmount,
                        SalesCount = ot.o.o.SalesCount,
                        CashStatus = ot.o.o.CashStatus,
                        CashNum = ot.o.o.CashNum,
                        CashDate = ot.o.o.CashDate,
                        SectionId = ot.o.o.SectionId,
                        PrintTimes = ot.o.o.PrintTimes,
                        Remark = ot.o.o.Remark,
                        RemarkDate = ot.o.o.RemarkDate,
                        //CreatedDate = ot.o.CreatedDate,
                        //CreatedUser = ot.o.CreatedUser,
                        //UpdatedDate = ot.o.UpdatedDate.Value,
                        //UpdatedUser = ot.o.UpdatedUser.Value,
                        StatusName = ((EnumSaleOrderStatus)ot.o.o.Status).GetDescription(),
                        CashStatusName = ot.o.o.CashStatus.HasValue ? ((EnumCashStatus)ot.o.o.CashStatus).GetDescription() : string.Empty,
                        StoreName = ot.o.store.store.Name,
                        //InvoiceSubject = ot.o.o.
                        SectionName = ot.o.store.section.Name,
                        TransNo = ot.ot.TransNo,
                    });



            }
            totalCount = 0;

            return new List<OPC_Sale>(0);
        }
    }
}
