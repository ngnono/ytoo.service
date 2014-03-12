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

        [DisplayName("商品编码")]
        public string ItemCode { get; set; }
        [DisplayName("商品名称")]
        public string Title { get; set; }
        [DisplayName("商品描述")]
        public string Descrip { get; set; }
        [DisplayName("促销描述")]
        public string DescripOfPromotion { get; set; }
        [DisplayName("促销开始时间")]
        public DateTime? DescripOfPromotionBeginDate { get; set; }
        [DisplayName("促销结束时间")]
        public DateTime? DescripOfPromotionEndDate { get; set; }
        [DisplayName("品牌名")]
        public string Brand { get; set; }
        [DisplayName("Tag名")]
        public string Tag { get; set; }
        [DisplayName("价格")]
        public decimal? Price { get; set; }
        [DisplayName("实体店名")]
        public string Store { get; set; }
        [DisplayName("绑定促销IDs")]
        public string PromotionIds { get; set; }
        [DisplayName("绑定主题IDs")]
        public string SubjectIds { get; set; }
        public int SessionU { get; set; }
        public int Id { get; set; }
        public int GroupId { get; set; }

    }
    public class ProductUploadJob
    {
        public int JobId { get; set; }
        public DateTime? InDate { get; set; }

        public ProUploadStatus Status { get; set; }
    }
}
