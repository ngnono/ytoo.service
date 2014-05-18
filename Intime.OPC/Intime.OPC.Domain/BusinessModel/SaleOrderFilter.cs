
using Intime.OPC.Domain.Enums;

namespace Intime.OPC.Domain.BusinessModel
{
    public class SaleOrderFilter
    {
        /// <summary>
        /// 店铺ID
        /// </summary>
        public int? StoreId { get;set; }

        /// <summary>
        /// 销售单 NO
        /// </summary>
        public string SaleOrderNo { get; set; }

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
    }
}
