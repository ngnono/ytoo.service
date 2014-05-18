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
using LinqKit;
using PredicateBuilder = LinqKit.PredicateBuilder;

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
                    query = PredicateBuilder.And(query, v => v.Status == (int)filter.Status);
                }

                if (filter.HasDeliveryOrderGenerated)
                {
                    //已经生成发货单的
                    query = PredicateBuilder.And(query, v => v.ShippingSaleId > 0);
                }
                else
                {
                    //未生成发货单的
                    query = PredicateBuilder.And(query, v => !v.ShippingSaleId.HasValue);
                }

                if (!String.IsNullOrWhiteSpace(filter.SaleOrderNo))
                    query = PredicateBuilder.And(query, v => v.SaleOrderNo == filter.SaleOrderNo);

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
                if (filter.StoreId != null)
                    query = PredicateBuilder.And(query, v => v.StoreId == filter.StoreId.Value);
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

        public List<OPC_Sale> GetPagedList(PagerRequest pagerRequest, out int totalCount, SaleOrderFilter filter,
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
                        let shipp_let = (from s in db.OPC_ShippingSales
                                         where ot.o.o.ShippingSaleId == s.Id
                                         select new OPC_ShippingSaleClone
                                         {
                                             Id = s.Id,
                                             BrandId = s.BrandId,
                                             CreateDate = s.CreateDate,
                                             CreateUser = s.CreateUser,
                                             OrderNo = s.OrderNo,
                                             PrintTimes = s.PrintTimes,
                                             RmaNo = s.RmaNo,
                                             ShippingAddress = s.ShippingAddress,
                                             ShippingCode = s.ShippingCode,
                                             ShippingStatus = s.ShippingStatus,
                                             ShippingContactPerson = s.ShippingContactPerson,
                                             ShippingContactPhone = s.ShippingContactPhone,
                                             ShipViaId = s.ShipViaId,
                                             ShipViaName = s.ShipViaName,
                                             ShippingFee = s.ShippingFee,
                                             ShippingRemark = s.ShippingRemark,
                                             ShippingZipCode = s.ShippingZipCode,
                                             StoreId = s.StoreId,
                                             UpdateDate = s.UpdateDate,
                                             UpdateUser = s.UpdateUser
                                         })
                        join p in db.PaymentMethods on ot.ot.PaymentCode equals p.Code
                        join oo in db.Orders on ot.o.o.OrderNo equals oo.OrderNo 
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

                      Store = new StoreClone
            {
                Id = ot.o.store.store.Id,
                Name = ot.o.store.store.Name,
                ExStoreId = ot.o.store.store.ExStoreId,
                GpsAlt = ot.o.store.store.GpsAlt,
                GpsLng = ot.o.store.store.GpsLng,
                GpsLat = ot.o.store.store.GpsLat,
                Group_Id = ot.o.store.store.Group_Id,
                Latitude = ot.o.store.store.Latitude,
                Location = ot.o.store.store.Location,
                Longitude = ot.o.store.store.Longitude,
                Region_Id = ot.o.store.store.Region_Id,
                RMAAddress = ot.o.store.store.RMAAddress,
                RMAZipCode = ot.o.store.store.RMAZipCode,
                RMAPerson = ot.o.store.store.RMAPerson,
                RMAPhone = ot.o.store.store.RMAPhone,
                Tel = ot.o.store.store.Tel,
                StoreLevel = ot.o.store.store.StoreLevel,

                CreatedDate = ot.o.store.store.CreatedDate,
                CreatedUser = ot.o.store.store.CreatedUser,
                Description = ot.o.store.store.Description,
                Status = ot.o.store.store.Status,
                UpdatedDate = ot.o.store.store.UpdatedDate,
                UpdatedUser = ot.o.store.store.CreatedUser
            },


                      Section = new SectionClone
                      {
                          BrandId = ot.o.store.section.BrandId,
                          ChannelSectionId = ot.o.store.section.ChannelSectionId,
                          ContactPerson = ot.o.store.section.ContactPerson,
                          ContactPhone = ot.o.store.section.ContactPhone,
                          CreateDate = ot.o.store.section.CreateDate,
                          CreateUser = ot.o.store.section.CreateUser,
                          Location = ot.o.store.section.Location,
                          Name = ot.o.store.section.Name,
                          Id = ot.o.store.section.Id,
                          Status = ot.o.store.section.Status,
                          StoreCode = ot.o.store.section.StoreCode,
                          StoreId = ot.o.store.section.StoreId,
                          UpdateDate = ot.o.store.section.UpdateDate,
                          UpdateUser = ot.o.store.section.CreateUser,
                          SectionCode = ot.o.store.section.SectionCode
                      },
                      //SectionClone.Convert2Section(ot.o.store.section),

                      OrderTransaction = new OrderTransactionClone
                      {
                          Amount = ot.ot.Amount,
                          CanSync = ot.ot.CanSync,
                          CreateDate = ot.ot.CreateDate,
                          Id = ot.ot.Id,
                          IsSynced = ot.ot.IsSynced,
                          OrderNo = ot.ot.OrderNo,
                          OrderType = ot.ot.OrderType,
                          OutsiteType = ot.ot.OutsiteType,
                          OutsiteUId = ot.ot.OutsiteUId,
                          PaymentCode = ot.ot.PaymentCode,
                          SyncDate = ot.ot.SyncDate,
                          TransNo = ot.ot.TransNo
                      }, //OrderTransactionClone.Convert2OorderTransaction(ot.ot),

                      ShippingSale = shipp_let.FirstOrDefault(),
                      Order = new OrderClone
                      {
                          
                      }
                      //OPC_ShippingSaleClone.Convert2ShippingSale(shipp_let.FirstOrDefault())
                  };

                totalCount = q.Count();

                var rst = q.OrderByDescending(v => v.CreatedDate).ThenBy(v => v.OrderNo).Skip(pagerRequest.SkipCount).Take(pagerRequest.PageSize).ToList();

                return AutoMapper.Mapper.Map<List<OPC_SaleClone>, List<OPC_Sale>>(rst);
            }
        }
    }
}
