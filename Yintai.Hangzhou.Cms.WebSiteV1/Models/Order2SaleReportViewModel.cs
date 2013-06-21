using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yintai.Hangzhou.Cms.WebSiteV1.Models
{
    public class Order2SaleReportViewModel:BaseViewModel
    {
        public string OrderNo { get; set; }
        public string SectionName { get; set; }
        public string SectionPerson { get; set; }
        public string VipCard { get; set; }
        public string StoreItemNo { get; set; }
        public string StoreItemDesc { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal ItemPrice { get; set; }
        public decimal ExtendPrice { get; set; }
        public DateTime RecDate { get; set; }
    }
}