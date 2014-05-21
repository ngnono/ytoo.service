
using System.Collections.Generic;
using Intime.OPC.Domain.Enums;

namespace Intime.OPC.Domain.BusinessModel
{
    public class SaleOrderFilter
    {
        /// <summary>
        /// 销售单 NO
        /// </summary>
        public string SalesOrderNo { get; set; }

        /// <summary>
        /// 订单NO
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 发货单 Id
        /// </summary>
        public int? ShippingOrderId { get; set; }

        /// <summary>
        /// 日期范围
        /// </summary>
        public DateRangeFilter DateRange { get; set; }

        /// <summary>
        /// 销售单状态
        /// </summary>
        public EnumSaleOrderStatus? Status { get; set; }

        /// <summary>
        /// 是否生成发货单
        /// </summary>
        public bool? HasDeliveryOrderGenerated { get; set; }

        /// <summary>
        /// 查询指定门店
        /// </summary>
        public List<int> StoreIds { get; set; }

        /// <summary>
        /// 是否查询所有门店
        /// </summary>
        public bool IsAllStoreIds { get; set; }
    }
}
