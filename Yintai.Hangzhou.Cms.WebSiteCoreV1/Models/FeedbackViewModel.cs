using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yintai.Architecture.Common.Models;

namespace Yintai.Hangzhou.Cms.WebSiteCoreV1.Models
{
    public class FeedbackCollectionViewModel : PagerInfo, IViewModel
    {
        public FeedbackCollectionViewModel(PagerRequest request)
            : base(request)
        {
        }

        public FeedbackCollectionViewModel(PagerRequest request, int totalCount)
            : base(request, totalCount)
        {
        }

        public List<FeedbackViewModel> Feedbacks { get; set; }
    }

    public class FeedbackViewModel : BaseViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(4096, MinimumLength = 1)]
        [Display(Name = "反馈内容")]
        public string Content { get; set; }

        [Required]
        [StringLength(64, MinimumLength = 1)]
        [Display(Name = "联系方式")]
        public string Contact { get; set; }

        [Range(0, Int32.MaxValue)]
        [Required]
        [Display(Name = "用户Id")]
        public int User_Id { get; set; }


        [Display(Name = "状态")]
        public int Status { get; set; }
        [Display(Name = "创建人")]
        public int CreatedUser { get; set; }
        [Display(Name = "创建日期")]
        [DataType(DataType.DateTime)]
        public System.DateTime CreatedDate { get; set; }
        [Display(Name = "修改日期")]
        [DataType(DataType.DateTime)]
        public System.DateTime UpdatedDate { get; set; }
        [Display(Name = "修改人")]
        public int UpdatedUser { get; set; }
    }
}
