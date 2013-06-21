using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Yintai.Hangzhou.Cms.WebSiteV1.Models
{
    public class WXReplyViewModel:BaseViewModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name="关键词")]
        public string MatchKey { get; set; }
        [Required]
        [Display(Name = "自动回复")]
        public string ReplyMsg { get; set; }
        [Display(Name = "状态")]
        public int Status { get; set; }
    }
}