using System;

namespace OPCApp.Domain.Models
{
    public class OPC_Sale
    {
        public bool IsSelected { get; set; }
       /// <summary>
       /// 渠道号
       /// </summary>
        public string TransNo { get; set; }

        public int Id { get; set; }
        public string OrderNo { get; set; }
        public string SaleOrderNo { get; set; }
        public string SalesType { get; set; }
        public int? ShipViaId { get; set; }
        public int Status { get; set; }
        public string ShippingCode { get; set; }
        public decimal ShippingFee { get; set; }
        public int? ShippingStatus { get; set; }
        public string ShippingRemark { get; set; }
        public DateTime SellDate { get; set; }
        public bool? IfTrans { get; set; }
        public int? TransStatus { get; set; }
        public decimal SalesAmount { get; set; }
        public int? SalesCount { get; set; }
        public string CashStatusName { get; set; }
        public string CashNum { get; set; }
        public DateTime? CashDate { get; set; }
        public int? SectionId { get; set; }
        public string SectionName { get; set; }
        public int? PrintTimes { get; set; }
        public string Remark { get; set; }
        public DateTime? RemarkDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int UpdatedUser { get; set; }
        public string StatusName { get; set; }

        /// <summary>
        /// 门店
        /// </summary>
        public string StoreName { get; set; }
        /// <summary>
        /// 发票抬头
        /// </summary>

        public string InvoiceSubject { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public string PayType { get; set; }
        /// <summary>
        /// 发票内容
        /// </summary>
        public string Invoice { get; set; }
        /// <summary>
        /// 门店电话
        /// </summary>
        public string StoreTelephone { get; set; }
        /// <summary>
        /// 门店地址
        /// </summary>
        public string StoreAddress { get; set; }
    }
}