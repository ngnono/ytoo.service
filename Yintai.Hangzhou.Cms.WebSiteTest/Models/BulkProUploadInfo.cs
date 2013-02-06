using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yintai.Hangzhou.Cms.WebSiteTest.Models
{
    public enum ProUploadStatus
    { 
       ProductsOnDisk= 0,
       ProductsOnStage = 1,
       ProductsValidateFailed = 2,
       ProductsValidateNoImageSuccess = 3,
       ProductsValidateWithImageSuccess = 4,
       ProductsOnLive = 5
    }
    public class BulkProUploadInfoModel
    {
        public int uploadId { get; set; }
        public int status { get; set; }
        
    }
}