using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security;

namespace Intime.OPC.Domain.Dto
{
    [DataContract]
    public class SaleDto
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string OrderNo { get; set; }

        [DataMember]
        public string SaleOrderNo { get; set; }

        [DataMember]
        public int SalesType { get; set; }

        [DataMember]
        public int? ShipViaId { get; set; }

        [DataMember]
        public int Status { get; set; }

        [DataMember]
        public string ShippingCode { get; set; }

        [DataMember]
        public decimal ShippingFee { get; set; }

        [DataMember]
        public int? ShippingStatus { get; set; }

        [DataMember]
        public string ShippingStatusName { get; set; }

        [DataMember]
        public string ShippingRemark { get; set; }

        [DataMember]
        public DateTime SellDate { get; set; }

        [DataMember]
        public string IfTrans { get; set; }

        [DataMember]
        public string TransStatus { get; set; }

        [DataMember]
        public decimal SalesAmount { get; set; }

        [DataMember]
        public int? SalesCount { get; set; }

        [DataMember]
        public int? CashStatus { get; set; }

        [DataMember]
        public string CashNum { get; set; }

        [DataMember]
        public DateTime? CashDate { get; set; }

        [DataMember]
        public int? SectionId { get; set; }

        [DataMember]
        public int? PrintTimes { get; set; }

        [DataMember]
        public string Remark { get; set; }

        [DataMember]
        public DateTime? RemarkDate { get; set; }

        [DataMember]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        public int CreatedUser { get; set; }

        [DataMember]
        public DateTime UpdatedDate { get; set; }

        [DataMember]
        public int UpdatedUser { get; set; }

        [DataMember]
        public string StatusName { get; set; }

        [DataMember]
        public string CashStatusName { get; set; }

        [DataMember]
        public string StoreName { get; set; }

        [DataMember]
        public string InvoiceSubject { get; set; }

        [DataMember]
        public string PayType { get; set; }

        [DataMember]
        public string SectionName { get; set; }

        [DataMember]
        public string Invoice { get; set; }

        [DataMember]
        public string StoreTelephone { get; set; }

        [DataMember]
        public string StoreAddress { get; set; }

        [DataMember]
        public string TransNo { get; set; }

        [DataMember]
        public string OrderSource { get; set; }

        [DataMember]
        public string ReceivePerson { get; set; }

        /// <summary>
        /// 订单
        /// </summary>
        public OrderDto Order { get; set; }

        /// <summary>
        /// 出库单
        /// </summary>
        public ShippingSaleDto ShippingOrder { get; set; }
    }
}