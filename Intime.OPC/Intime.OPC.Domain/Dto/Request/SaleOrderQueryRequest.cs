using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Intime.OPC.Domain.Enums;
using Newtonsoft.Json;

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
        [JsonProperty(PropertyName = "DeliveryOrderId")]
        [DataMember(Name = "DeliveryOrderId")]
        public int? ShippingOrderId { get; set; }

        [DataMember]
        public int? Page { get; set; }

        [DataMember]
        public int? PageSize { get; set; }

        [DataMember]
        public int? SortOrder { get; set; }


        /// <summary>
        /// 销售单状态
        /// </summary>
        [DataMember]
        public EnumSaleOrderStatus? Status { get; set; }

        ///// <summary>
        ///// 是否生成发货单
        ///// </summary>
        //[DataMember]
        //public bool? HasDeliveryOrderGenerated { get; set; }

        /// <summary>
        /// 查询指定门店
        /// </summary>
        public List<int> StoreIds { get; set; }

        /// <summary>
        /// 是否查询所有门店
        /// </summary>
        public bool IsAllStoreIds { get; set; }
    }

    [DataContract]
    public class DateRangeRequest
    {
        private DateTime? _beginDate;
        private DateTime? _endDate;

        [DataMember(Name = "startdate")]
        public DateTime? StartDate
        {
            get { return _beginDate; }
            set { _beginDate = value; }
        }

        [DataMember(Name = "enddate")]
        public DateTime? EndDate { get { return _endDate == null ? _endDate : _endDate.Value.Date.AddDays(1); } set { _endDate = value; } }
    }

    public interface PagerRequest
    {

    }
}
