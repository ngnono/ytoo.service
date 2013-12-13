using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Yintai.Hangzhou.Cms.WebSiteV1.Models
{
    public class CategoryViewModel:BaseViewModel
    {

        [Display(Name="商品分类码")]
        public int ExCatId { get; set; }
         [Display(Name = "商品分类")]
        public string Name { get; set; }
        public string ExCatCode { get; set; }
         [Display(Name = "状态")]
        public int Status { get; set; }
        public System.DateTime UpdateDate { get; set; }

        public List<ShowCategoryViewModel> ShowCategories { get; set; }

        public string Channels { get { 
            if (ShowCategories == null || ShowCategories.Count()<=0)
                return string.Empty;
            return ShowCategories.Aggregate(new StringBuilder(),(s,m)=>s.AppendFormat("_{0}",m.ShowChannel),s=>s.ToString());
        } }
    }
    public class ShowCategoryViewModel:BaseViewModel
    {
        [Key]
        public int Id { get; set; }
        [Display(Name="显示分类id")]
        public int ShowCategoryId { get; set; }
        [Display(Name = "显示分类名")]
        public string ShowCategoryName { get; set; }
        [Display(Name = "渠道")]
        public string ShowChannel { get; set; }

        public int Status { get; set; }
    }
    public class CategorySearchOption
    {
        public string Name { get; set; }
        public int? PId { get; set; }
    }
}