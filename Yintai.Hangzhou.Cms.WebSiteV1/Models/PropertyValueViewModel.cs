using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yintai.Hangzhou.Cms.WebSiteV1.Models
{
    public class PropertyValueViewModel:BaseViewModel
    {
        public int CategoryId { get; set; }
        public string PropertyDesc { get; set; }
        public int SortOrder { get; set; }
        public int Status { get; set; }
        public Nullable<bool> IsVisible { get; set; }

    }
}