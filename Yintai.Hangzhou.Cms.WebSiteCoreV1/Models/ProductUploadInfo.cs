using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yintai.Hangzhou.Cms.WebSiteCoreV1.Models
{
    public enum ProUploadStatus
    {
        ProductsOnDisk = 0,
        ProductsOnStage = 1,
        ProductsValidateFailed = 2,
        ProductsValidateNoImageSuccess = 3,
        ProductsValidateWithImageSuccess = 4,
        ProductsOnLive = 5
    }
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
