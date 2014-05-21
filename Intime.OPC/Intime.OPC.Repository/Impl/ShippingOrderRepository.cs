using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Intime.OPC.Domain;
using Intime.OPC.Domain.BusinessModel;
using Intime.OPC.Domain.Enums.SortOrder;
using Intime.OPC.Domain.Models;
using Intime.OPC.Domain.Partials.Models;
using Intime.OPC.Repository.Base;
using LinqKit;
using log4net.Appender;
using PredicateBuilder = LinqKit.PredicateBuilder;

namespace Intime.OPC.Repository.Impl
{

    /// <summary>
    /// 出库单（物流单）
    /// </summary>
    public class ShippingOrderRepository : OPCBaseRepository<int, OPC_ShippingSale>, IShippingOrderRepository
    {
        #region methods

        private static Expression<Func<OPC_ShippingSale, bool>> Filter(ShippingOrderFilter filter)
        {
            var query = PredicateBuilder.True<OPC_ShippingSale>();

            if (filter != null)
            {
                if (filter.Status != null)
                {
                    query = PredicateBuilder.And(query, v => v.ShippingStatus == (int)filter.Status);
                }
            }

            return query;
        }

        private static Expression<Func<OPC_Sale, bool>> SaleOrder4Filter(ShippingOrderFilter filter)
        {
            var query = PredicateBuilder.True<OPC_Sale>();

            if (filter != null)
            {
                //if (filter.Status != null)
                //{
                //    query = PredicateBuilder.And(query, v => v.stat == (int)filter.Status);
                //}
            }

            return query;
        }

        private static Expression<Func<OrderItem, bool>> OrderItem4Filter(ShippingOrderFilter filter)
        {
            var query = PredicateBuilder.True<OrderItem>();

            if (filter != null)
            {
            }

            return query;
        }

        private static Func<IQueryable<OPC_ShippingSale>, IOrderedQueryable<OPC_ShippingSale>> OrderBy(ShippingOrderSortOrder sortOrder)
        {
            Func<IQueryable<OPC_ShippingSale>, IOrderedQueryable<OPC_ShippingSale>> orderBy = null;

            switch (sortOrder)
            {
                default:
                    orderBy = v => v.OrderByDescending(s => s.CreateDate).ThenBy(s => s.OrderNo);
                    break;
            }

            return orderBy;
        }

        #endregion

        public override IEnumerable<OPC_ShippingSale> AutoComplete(string query)
        {
            throw new NotImplementedException();
        }

        public List<ShippingOrderModel> GetPagedList(PagerRequest pagerRequest, out int totalCount,
            ShippingOrderFilter filter,
            ShippingOrderSortOrder sortOrder)
        {
            var shippingOrderFilter = Filter(filter);
            //var saleorderFilter = SaleOrder4Filter(filter);

            var rst = Func(db =>
            {
                var q1 = from ss in db.Set<OPC_ShippingSale>().AsExpandable().Where(shippingOrderFilter)
                         //let sale_let = (from sale in db.Set<OPC_Sale>().AsExpandable().Where(saleorderFilter)
                         //                where ss.Id == sale.ShippingSaleId
                         //                select sale
                         //    )
                         select new
                         {

                             ShippingSale = ss,
                             //SaleOrders = sale_let
                         };

                var t = q1.Count();

                var q =
                    q1.OrderByDescending(v => v.ShippingSale.CreateDate)
                        .Skip(pagerRequest.SkipCount)
                        .Take(pagerRequest.PageSize).Select(v => new ShippingOrderModel
                        {
                            Id = v.ShippingSale.Id,
                            CustomerAddress = v.ShippingSale.ShippingAddress,
                            CustomerName = v.ShippingSale.ShippingContactPerson,
                            CustomerPhone = v.ShippingSale.ShippingContactPhone,
                            ExpressCode = v.ShippingSale.ShippingCode,
                            ExpressFee = v.ShippingSale.ShippingFee ?? 0,
                            GoodsOutCode = v.ShippingSale.ShippingCode,
                            GoodsOutDate = v.ShippingSale.CreateDate,
                            GoodsOutType = String.Empty,//v.ShippingSale.
                            OrderNo = v.ShippingSale.OrderNo,
                            PrintTimes = v.ShippingSale.PrintTimes,
                            RmaNo = v.ShippingSale.RmaNo,
                            //SaleOrderNo
                            ShipCompanyName = v.ShippingSale.ShipViaName,
                            ShipManName = String.Empty,
                            ShippingMethod = String.Empty,
                            ShippingStatus = v.ShippingSale.ShippingStatus,
                            ShippingZipCode = v.ShippingSale.ShippingZipCode,
                            ShipViaExpressFee = v.ShippingSale.ShippingFee ?? 0
                        }).ToList();

                return new
                {
                    total = t,
                    data = q
                };
            });

            totalCount = rst.total;
            return rst.data;
        }

        public ShippingOrderModel GetItemModel(int id)
        {
            return Func(db =>
            {
                var q1 = from ss in db.Set<OPC_ShippingSale>().Where(v => v.Id == id)
                         //let sale_let = (from sale in db.Set<OPC_Sale>()
                         //                where ss.Id == sale.ShippingSaleId
                         //                select sale
                         //    )
                         select new
                         {

                             ShippingSale = ss,
                             //SaleOrders = sale_let
                         };

                var q =
                    q1.Select(v => new ShippingOrderModel
                    {
                        Id = v.ShippingSale.Id,
                        CustomerAddress = v.ShippingSale.ShippingAddress,
                        CustomerName = v.ShippingSale.ShippingContactPerson,
                        CustomerPhone = v.ShippingSale.ShippingContactPhone,
                        ExpressCode = v.ShippingSale.ShippingCode,
                        ExpressFee = v.ShippingSale.ShippingFee ?? 0,
                        GoodsOutCode = v.ShippingSale.ShippingCode,
                        GoodsOutDate = v.ShippingSale.CreateDate,
                        GoodsOutType = String.Empty, //v.ShippingSale.
                        OrderNo = v.ShippingSale.OrderNo,
                        PrintTimes = v.ShippingSale.PrintTimes,
                        RmaNo = v.ShippingSale.RmaNo,
                        //SaleOrderNo
                        ShipCompanyName = v.ShippingSale.ShipViaName,
                        ShipManName = String.Empty,
                        ShippingMethod = String.Empty,
                        ShippingStatus = v.ShippingSale.ShippingStatus,
                        ShippingZipCode = v.ShippingSale.ShippingZipCode,
                        ShipViaExpressFee = v.ShippingSale.ShippingFee ?? 0,
                        StoreId = v.ShippingSale.StoreId ?? 0
                    }).FirstOrDefault();

                return q;
            });
        }

        public void Update4ShippingCode(OPC_ShippingSale entity, int userId)
        {
            Action(db =>
           {
               EFHelper.UpdateEntityFields(db, entity, new List<string> { "ShipViaId", "ShipViaName", "ShippingCode", "ShippingFee" });

               /// return entity;
           });
        }

        /// <summary>
        /// 创建出库单，
        /// </summary>
        /// <param name="entity">shipping</param>
        /// <param name="saleOrderModels">销售单</param>
        /// <param name="userId">操作人</param>
        /// <returns></returns>
        public ShippingOrderModel CreateBySaleOrder(OPC_ShippingSale entity, List<OPC_Sale> saleOrderModels, int userId, string shippingRemark)
        {
            return Func(db =>
            {
                using (var trans = new TransactionScope())
                {
                    entity = EFHelper.Insert(db, entity);
                    if (!String.IsNullOrWhiteSpace(shippingRemark))
                    {
                        var remark = new OPC_ShippingSaleComment()
                        {
                            Content = shippingRemark,
                            CreateDate = DateTime.Now,
                            CreateUser = userId,
                            ShippingCode = String.Empty,
                            ShippingSaleId = entity.Id,
                            UpdateDate = DateTime.Now,
                            UpdateUser = userId
                        };
                        EFHelper.Insert(db, remark);
                    }

                    foreach (var sale in saleOrderModels)
                    {
                        sale.ShippingSaleId = entity.Id;
                        sale.UpdatedUser = userId;
                        sale.UpdatedDate = DateTime.Now;
                        sale.ShippingRemark = shippingRemark ?? String.Empty;

                        EFHelper.UpdateEntityFields(db, sale, new List<string> { "ShippingSaleId", "UpdatedUser", "UpdatedDate", "ShippingRemark" });
                    }

                    trans.Complete();

                    return GetItemModel(entity.Id);
                }
            });
        }

        public OPC_ShippingSaleComment CreateComment(OPC_ShippingSaleComment entity, int userId)
        {
            return Func(db =>

                EFHelper.Insert(db, entity)
            );
        }

        public void UpdateComment(OPC_ShippingSaleComment entity, int userId)
        {
            Action(db =>
   EFHelper.Update(db, entity)
);
        }

        public void Update4Times(ShippingOrderModel model, int times, int userId)
        {
            throw new NotImplementedException();
        }
    }
}
