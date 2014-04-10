using System;

namespace Intime.OPC.Domain.Dto
{
    public class SaleRmaDto
    {
        public int Id { get; set; }
        public string SaleOrderNo { get; set; }
        public string PaymentMethodName { get; set; }
        public double MustPayTotal { get; set; }
        public Nullable<decimal> RealRMASumMoney { get; set; }
        public string OrderNo { get; set; }
        public string TransMemo { get; set; }
        public DateTime BuyDate { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerRemark { get; set; }
        public bool IfReceipt { get; set; }
        public string ReceiptHead { get; set; }
        public string ReceiptContent { get; set; }
        /// <summary>
        /// 公司应付
        /// </summary>
        /// <value>The store fee.</value>
        public decimal? StoreFee { get; set; }
        /// <summary>
        /// 客户应付
        /// </summary>
        /// <value>The custom fee.</value>
        public decimal? CustomFee { get; set; }

        /// <summary>
        /// 退貨日期
        /// </summary>
        /// <value>The create date.</value>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 客服同意时间
        /// </summary>
        /// <value>The service agree date.</value>
        public DateTime ServiceAgreeDate { get; set; }
    }
}