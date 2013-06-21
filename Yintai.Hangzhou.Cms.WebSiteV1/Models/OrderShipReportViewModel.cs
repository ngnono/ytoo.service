using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yintai.Hangzhou.Cms.WebSiteV1.Models
{
    public class OrderShipReportViewModel:BaseViewModel
    {
        public string ShippingContactPerson { get; set; }
        public string ShippingContactPhone { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingZipCode { get; set; }
        public string OutboundNo { get; set; }
        public string NeedInvoice { get; set; }
        public string InvoiceSubject { get; set; }
        public string InvoiceDetail {get;set;}
        public decimal InvoiceAmount {get;set;}
        public string OrderNo { get; set; }
        public string SectionName { get; set; }
        public string SectionPhone { get; set; }
        public string SecionPerson { get; set; }
    }
    public class OrderShipItemReportViewModel:BaseViewModel
    {
        public string OutboundNo { get; set; }
        public string ProductDesc { get; set; }
        public string StoreItemNo { get; set; }
        public string StoreItemDesc { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal ItemPrice { get; set; }
        public decimal ExtendPrice { get; set; }
    }
}