using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Intime.OPC.Domain;
using Intime.OPC.Domain.BusinessModel;
using Intime.OPC.Domain.Enums.SortOrder;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository.Base;

namespace Intime.OPC.Repository.Impl
{
    public class ShippingOrderRepository : OPCBaseRepository<int, OPC_ShippingSale>, IShippingOrderRepository
    {
        #region methods

        private static Expression<Func<OPC_ShippingSale, bool>> Filter(ShippingOrderFilter filter)
        {
            var query = PredicateBuilder.True<OPC_ShippingSale>();

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

        public List<OPC_ShippingSale> GetPagedList(PagerRequest pagerRequest, out int totalCount, ShippingOrderFilter filter,
            ShippingOrderSortOrder sortOrder)
        {
            throw new NotImplementedException();
        }

        public OPC_ShippingSale Update4ShippingCode(OPC_ShippingSale entity, int userId)
        {
            return Func(db =>
            {
                EFHelper.UpdateEntityFields(db, entity, new List<string> { "ShipViaId", "ShipViaName", "ShippingCode", "ShippingFee" });

                return entity;
            });
        }

        /// <summary>
        /// 创建出库单，
        /// </summary>
        /// <param name="entity">shipping</param>
        /// <param name="saleOrderModels">销售单</param>
        /// <param name="userId">操作人</param>
        /// <returns></returns>
        public OPC_ShippingSale CreateBySaleOrder(OPC_ShippingSale entity, List<SaleOrderModel> saleOrderModels, int userId)
        {
            return Func(db =>
            {
                using (var trans = new TransactionScope())
                {
                    entity = EFHelper.Insert(db, entity);

                    //update saleordermodel
                    var ids = saleOrderModels.Select(v => v.Id);

                    var sales = EFHelper.Get<OPC_Sale>(db, v => ids.Contains(v.Id)).ToList();
                    if (sales.Count != ids.Count())
                    {
                        throw new ArgumentException("销售单未能找到，且与提供的数量不一致。");
                    }

                    foreach (var sale in sales)
                    {
                        sale.ShippingSaleId = entity.Id;
                        sale.UpdatedUser = userId;
                        sale.UpdatedDate = DateTime.Now;

                        EFHelper.UpdateEntityFields(db, sale, new List<string> { "ShippingSaleId", "UpdatedUser", "UpdatedDate" });
                    }

                    trans.Complete();

                    return entity;
                }
            });
        }
    }
}
