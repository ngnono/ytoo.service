using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yintai.Hangzhou.Cms.WebSiteTest.Util
{
    public class ProductUploadInfo
    {
        public string ItemCode { get; set; }
        public string Title { get; set; }
        public string Descrip { get; set; }
        public string DescripOfPromotion { get; set; }
        public DateTime? DescripOfPromotionBeginDate { get; set; }
        public DateTime? DescripOfPromotionEndDate { get; set; }
        public string Brand { get; set; }
        public string Tag { get; set; }
        public decimal? Price { get; set; }
        public string Store { get; set; }
        public string PromotionIds { get; set; }
        public string SubjectIds { get; set; }
        public int SessionU { get; set; }

    }
}
