using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yintai.Architecture.Common.Models;

namespace Yintai.Hangzhou.Cms.WebSiteCoreV1.Models
{
    public class CommentCollectionViewModel : PagerInfo, IViewModel
    {
        public CommentCollectionViewModel(PagerRequest request)
            : base(request)
        {
        }

        public CommentCollectionViewModel(PagerRequest request, int totalCount)
            : base(request, totalCount)
        {
        }

        public List<CommentViewModel> Comments { get; set; }
    }

    public class CommentViewModel : BaseViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(256, MinimumLength = 0)]
        [Display(Name = "评论内容")]
        public string Content { get; set; }

        [Range(0, Int32.MaxValue)]
        [Display(Name = "发表评论人")]
        public int User_Id { get; set; }

        [Range(0, Int32.MaxValue)]
        [Display(Name = "来源Id")]
        public int SourceId { get; set; }

        [Range(0, Int32.MaxValue)]
        [Display(Name = "来源类型")]
        public int SourceType { get; set; }

        [Range(0, Int32.MaxValue)]
        [Display(Name = "被回复人Id")]
        public int ReplyUser { get; set; }

        [Display(Name = "被回复Id")]
        public int ReplyId { get; set; }

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

        public ResourceViewModel SourceResource { get; set; }
        public ResourceViewModel CommentResource { get; set; }
        public CustomerViewModel CommentUser { get; set; }

    }
   
}
