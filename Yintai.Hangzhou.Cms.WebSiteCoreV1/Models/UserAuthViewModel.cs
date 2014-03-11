using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Yintai.Hangzhou.Cms.WebSiteCoreV1.Models
{
    public class UserAuthViewModel:BaseViewModel
    {
        [Key]
        [Display(Name="授权编码")]
        public int Id { get; set; }
        
        [Display(Name = "用户代码")]
        [UIHint("Association")]
        [AdditionalMetadata("controller", "customer")]
        [AdditionalMetadata("displayfield", "Name")]
        [AdditionalMetadata("searchfield", "name")]
        [AdditionalMetadata("valuefield", "Id")]
        public int UserId { get; set; }
        public string UserNick { get; set; }
        [Display(Name = "门店代码")]
        [UIHint("Association")]
        [AdditionalMetadata("controller", "store")]
        [AdditionalMetadata("displayfield", "Name")]
        [AdditionalMetadata("searchfield", "name")]
        [AdditionalMetadata("valuefield", "Id")]
        public int StoreId { get; set; }
        public string StoreName { get; set; }
       [Display(Name = "授权数据")]
        public int Type { get; set; }
        [Display(Name = "品牌代码")]
       [UIHint("Association")]
       [AdditionalMetadata("controller", "brand")]
       [AdditionalMetadata("displayfield", "Name")]
       [AdditionalMetadata("searchfield", "name")]
       [AdditionalMetadata("valuefield", "Id")]
        public int? BrandId { get; set; }
        public string BrandName { get; set; }
        public int Status { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int CreatedUser { get; set; }
    }
}
