using System;


namespace OPCApp.Domain.Customer
{
    public partial class OPC_SaleRMA
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
        public string  ReceiptContent { get; set; }
        public Nullable<decimal> StoreFee { get; set; }
        public Nullable<decimal> CustomFee { get; set; }

    }
}
