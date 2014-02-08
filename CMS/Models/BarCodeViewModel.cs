using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Yintai.Hangzhou.Cms.WebSiteV1.Models
{
    public class BarCodeViewModel
    {
        [DisplayName("门店")]
        public int StoreId { get; set; }
        [DisplayName("专柜")]
        public int SectionId { get; set; }
        [DisplayName("二维码类型")]
        public int PackageType { get; set; }
        [DisplayName("跳转目标")]
        public int SourceId { get; set; }

      
    }
    public class SectionSearchViewModel
    {
        public int StoreId { get; set; }
    }
    public class ProductSearchViewModel
    {
        public string SkuCode { get; set; }
        public int PackageType { get; set; }
    }
}