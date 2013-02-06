using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Yintai.Hangzhou.Cms.WebSiteCoreV1.Models
{
    public enum ProUploadStatus
    {
        [Description("已上传到服务器本地")]
        ProductsOnDisk = 0,
        [Description("已上传到数据库")]
        ProductsOnStage = 1,
        [Description("数据校验失败")]
        ProductsValidateFailed = 2,
        [Description("数据校验通过(无图片)")]
        ProductsValidateNoImageSuccess = 3,
        [Description("数据校验通过")]
        ProductsValidateWithImageSuccess = 4,
        [Description("发布成功")]
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
        public int Id { get; set; }

    }
    public class ProductUploadJob
    {
        public int JobId { get; set; }
        public DateTime? InDate { get; set; }
    }
}
