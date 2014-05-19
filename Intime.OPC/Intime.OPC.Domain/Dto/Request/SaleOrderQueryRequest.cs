using System;
using System.Runtime.Serialization;
using Intime.OPC.Domain.Enums;

namespace Intime.OPC.Domain.Dto.Request
{
    [DataContract]
    public class SaleOrderQueryRequest : DateRangeRequest
    {
        [DataMember(Name = "saleorderno")]
        public string SaleOrderNo { get; set; }

        [DataMember(Name = "orderno")]
        public string OrderNo { get; set; }

        /// <summary>
        /// 发货单号
        /// </summary>
        public string ShippingOrderNo { get; set; }

        public int? Page { get; set; }

        public int? PageSize { get; set; }

        public int? SortOrder { get; set; }


        /// <summary>
        /// 销售单状态
        /// </summary>
        public EnumSaleOrderStatus? Status { get; set; }

        /// <summary>
        /// 是否生成发货单
        /// </summary>
        public bool HasDeliveryOrderGenerated { get; set; }
    }

    [DataContract]
    public class DateRangeRequest
    {
        private DateTime? _beginDate;
        private DateTime? _endDate;

        [DataMember(Name = "startdate")]
        public DateTime? StartDate
        {
            get { return _beginDate == null ? DateTime.Now.Date : _beginDate.Value.Date; }
            set { _beginDate = value; }
        }

        [DataMember(Name = "enddate")]
        public DateTime? EndDate { get { return _endDate == null ? DateTime.Now.AddDays(1).Date : _endDate.Value.Date.AddDays(1); } set { _endDate = value; } }
    }

    public interface PagerRequest
    {

    }
}
