using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Yintai.Hangzhou.Cms.WebSiteV1.Models
{
    public class PMessageViewModel:BaseViewModel
    {
        [Display(Name="私信内容")]
        [StringLength(500,MinimumLength=1,ErrorMessage="长度介于1-500")]
        [Required]
        public string TextMsg { get; set; }
        public CustomerViewModel OtherUserModel { get {
            return ToUserModel;
        } }
        public int OtherUserId { get {
            return ToUser;
        } }
        public int StoreUserId { get {
            return FromUser;
        } }
        public CustomerViewModel FromUserModel { get; set; }
        public CustomerViewModel ToUserModel { get; set; }
        public int FromUser { get; set; }
        public int ToUser { get; set; }
        public DateTime CreateDate { get; set; }
    }
    public class PMessageSearchOption
    {
        [Display(Name="开始时间")]
        public DateTime? FromDate { get; set; }
        [Display(Name = "结束时间")]
        public DateTime? ToDate { get; set; }
    }
}