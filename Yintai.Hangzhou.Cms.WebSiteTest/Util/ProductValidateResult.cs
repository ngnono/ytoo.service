using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yintai.Hangzhou.Cms.WebSiteTest.Models;

namespace Yintai.Hangzhou.Cms.WebSiteTest.Util
{
    public class ProductValidateResult
    {
        public string ItemCode { get; set; }
        public string ValidateResult { get; set; }
        public ProUploadStatus ResultStatus { get; set; }
    }
}
