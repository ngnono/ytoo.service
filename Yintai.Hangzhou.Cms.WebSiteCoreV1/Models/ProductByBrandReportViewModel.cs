using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yintai.Hangzhou.Cms.WebSiteCoreV1.Models
{
    public class ProductByBrandReportViewModel:BaseViewModel
    {
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public int Products { get; set; }
    }
}
