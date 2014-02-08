using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yintai.Hangzhou.Cms.WebSiteV1.Models
{
    public class ReportByProductBrandOption:BaseViewModel,IValidatableObject
    {
        [Display(Name="统计开始时间")]
        public DateTime? FromDate { get; set; }
        [Display(Name="统计结束时间")]
        public DateTime? ToDate { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!FromDate.HasValue && !ToDate.HasValue)
                yield return new ValidationResult("必须选择一个时间！") ;
        }
    }
}
