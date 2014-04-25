using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Yintai.Hangzhou.Cms.WebSiteV1.Models
{

    public class WxMenuViewModel
    {
        [JsonProperty("items")]
        public IEnumerable<WxMenuItemViewModel> Items { get; set; }
       
    }

    public class WxMenuItemViewModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("text")]
        [Required(ErrorMessage="菜单名必须")]
        [MaxLength(8,ErrorMessage = "菜单名必须")]
        public string Text { get; set; }
        [JsonProperty("data")]
        public WxMenuDataViewModel Data { get; set; }
        [JsonProperty("children")]
        public IEnumerable<WxMenuItemViewModel> Children { get; set; } 
    }
    public class WxMenuDataViewModel
    {
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}