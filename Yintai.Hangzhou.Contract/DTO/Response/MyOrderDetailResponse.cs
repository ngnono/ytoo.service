using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Yintai.Hangzhou.Contract.DTO.Response.Resources;
using Yintai.Hangzhou.Contract.Response;
using Yintai.Hangzhou.Model.Enums;
using com.intime.fashion.common;

namespace Yintai.Hangzhou.Contract.DTO.Response
{
    public class MyOrderDetailResponse:BaseResponse
    {
        [DataMember(Name="orderno")]
        public string OrderNo { get; set; }
        [DataMember(Name = "totalamount")]
        public decimal TotalAmount { get; set; }
        [IgnoreDataMember]
        public int Status { get; set; }
         [DataMember(Name = "createdate")]
        public System.DateTime CreateDate { get; set; }
        [DataMember(Name="status")]
         public string StatusF { get {
             return ((OrderStatus)Status).ToFriendlyString();
         } }
         [DataMember(Name="paymentname")]
         public string PaymentMethodName { get; set; }
        [DataMember(Name = "shippingzipcode")]
         public string ShippingZipCode { get; set; }
        [DataMember(Name = "shippingaddress")]
         public string ShippingAddress { get; set; }
        [DataMember(Name = "shippingcontactperson")]
         public string ShippingContactPerson { get; set; }
        [DataMember(Name = "shippingcontactphone")]
         public string ShippingContactPhone { get; set; }
        [DataMember(Name = "needinvoice")]
         public Nullable<bool> NeedInvoice { get; set; }
        [DataMember(Name = "invoicesubject")]
         public string InvoiceSubject { get; set; }
        [DataMember(Name = "invoicedetail")]
         public string InvoiceDetail { get; set; }
        [DataMember(Name = "shippingfee")]
         public Nullable<decimal> ShippingFee { get; set; }
         [DataMember(Name = "shippingno")]
         public string ShippingNo { get; set; }
        [DataMember(Name = "shippingvianame")]
         public string ShippingViaName { get; set; }
        [DataMember(Name = "memo")]
         public string Memo { get; set; }
        
        [DataMember(Name = "resource")]
         public ResourceInfoResponse ProductResource { get; set; }
        [DataMember(Name="product")]
        public MyOrderItemDetailResponse Product { get; set; }
        [DataMember(Name = "rmas")]
        public IEnumerable<MyRMAResponse> RMAs { get; set; }
        [DataMember(Name = "totalquantity")]
        public int TotalQuantity { get {
            if (Product == null)
                return 0;
            return Product.Quantity;
        } }
        [DataMember(Name="canvoid")]
        public bool CanVoid { get {
           return new int[]{(int)OrderStatus.Create,
                                        (int)OrderStatus.AgentConfirmed,
                                        (int)OrderStatus.CustomerConfirmed,
                                        (int)OrderStatus.CustomerReceived,
                                        (int)OrderStatus.OrderPrinted}.Any(status => status == Status);
        } }
          [DataMember(Name = "canrma")]
        public bool CanRMA {
            get {
                return Status == (int)OrderStatus.Convert2Sales;
            }
        }


    }
    [DataContract]
    public class MyOrderItemDetailResponse : BaseResponse
    {

        [DataMember(Name = "productid")]
        public string ProductId { get; set; }
        [DataMember(Name = "productname")]
        public string ProductName { get; set; }
        [DataMember(Name = "properties")]
        public string ProductDesc { get; set; }
        [DataMember(Name = "itemno")]
        public string StoreItemNo { get; set; }
        [DataMember(Name = "itemdesc")]
        public string StoreItemDesc { get; set; }
        [DataMember(Name = "quantity")]
        public int Quantity { get; set; }
        [DataMember(Name = "price")]
        public decimal ItemPrice { get; set; }
        [DataMember(Name="resource")]
         public ResourceInfoResponse ProductResource { get; set; }
    }
     [DataContract]
    public class MyRMAResponse : BaseResponse
    {
        [DataMember(Name = "rmano")]
        public string RMANo { get; set; }
        [IgnoreDataMember]
        public int RMAType { get; set; }
        [DataMember(Name = "rmatype")]
        public string RMATypeF
        {
            get
            {
                return ((Yintai.Hangzhou.Model.Enums.RMAType)RMAType).ToFriendlyString();
            }
        }
        [IgnoreDataMember]
        public int Status { get; set; }
        [DataMember(Name = "status")]
        public string RMAF
        {
            get
            {
                return ((RMAStatus)Status).ToFriendlyString();
            }
        }
        [DataMember(Name = "rmaamount")]
        public decimal RMAAmount { get; set; }
        [DataMember(Name = "createdate")]
        public System.DateTime CreateDate { get; set; }
        public int UpdateUser { get; set; }
        public System.DateTime UpdateDate { get; set; }
        [DataMember(Name = "bankname")]
        public string BankName { get; set; }
        [DataMember(Name = "bankaccount")]
        public string BankAccount { get; set; }
        [DataMember(Name = "bankcard")]
        public string BankCard { get; set; }
        [DataMember(Name = "rejectreason")]
        public string RejectReason { get; set; }
        [DataMember(Name = "rebatepostfee")]
        public Nullable<decimal> rebatepostfee { get; set; }
        [DataMember(Name = "chargepostfee")]
        public Nullable<decimal> chargepostfee { get; set; }
        [DataMember(Name = "actualamount")]
        public Nullable<decimal> ActualAmount { get; set; }
        [DataMember(Name = "chargegiftfee")]
        public decimal? ChargeGiftFee { get; set; }

    }
}
