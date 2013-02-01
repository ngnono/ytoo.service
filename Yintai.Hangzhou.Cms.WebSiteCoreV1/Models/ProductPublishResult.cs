using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yintai.Hangzhou.Cms.WebSiteCoreV1.Models;

namespace Yintai.Hangzhou.Cms.WebSiteCoreV1.Models
{
    public class ProductPublishResult
    {
        public string ItemCode { get; set; }
        public ProUploadStatus Status { get; set; }
        public string PublishMemo { get; set; }
    }
}
