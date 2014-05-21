using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intime.OPC.Domain.Enums;

namespace Intime.OPC.Domain.BusinessModel
{
    public class ShippingOrderFilter
    {
        /// <summary>
        /// 销售单 no
        /// </summary>
        public string SaleOrderNo { get; set; }

        /// <summary>
        /// 销售单状态
        /// </summary>
        public EnumSaleOrderStatus? Status { get; set; }

        /// <summary>
        /// 是否生成发货单
        /// </summary>
        public bool HasDeliveryOrderGenerated { get; set; }

        /// <summary>
        /// 查询指定门店
        /// </summary>
        public List<int> StoreIds { get; set; }

        /// <summary>
        /// 是否查询所有门店
        /// </summary>
        public bool IsAllStoreIds { get; set; }

        /// <summary>
        /// 日期范围
        /// </summary>
        public DateRangeFilter DateRange { get; set; }

        public int? OrderNo { get; set; }
    }
}
