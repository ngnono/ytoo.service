using Intime.OPC.Domain;
using Intime.OPC.Domain.BusinessModel;
using Intime.OPC.Domain.Enums.SortOrder;
using Intime.OPC.Domain.Models;
using System.Collections.Generic;

namespace Intime.OPC.Repository
{
    public interface IShippingOrderRepository : IOPCRepository<int, OPC_ShippingSale>
    {
        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="pagerRequest">分页请求参数</param>
        /// <param name="totalCount">记录总数</param>
        /// <param name="filter">筛选项</param>
        /// <param name="sortOrder">排序项</param>
        /// <returns></returns>
        List<OPC_ShippingSale> GetPagedList(PagerRequest pagerRequest, out int totalCount, ShippingOrderFilter filter,
                                   ShippingOrderSortOrder sortOrder);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userId">操作人</param>
        /// <returns></returns>
        OPC_ShippingSale Update4ShippingCode(OPC_ShippingSale entity, int userId);

        /// <summary>
        /// 创建出库单，
        /// </summary>
        /// <param name="entity">shipping</param>
        /// <param name="saleOrderModels">销售单</param>
        /// <param name="userId">操作人</param>
        /// <returns></returns>
        OPC_ShippingSale CreateBySaleOrder(OPC_ShippingSale entity, List<OPC_Sale> saleOrderModels, int userId);
    }
}
