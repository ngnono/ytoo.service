using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.Model.Enums;

namespace Yintai.Hangzhou.Cms.WebSiteCoreV1.Models
{
    public class SpecialTopicCollectionViewModel : PagerInfo, IViewModel
    {
        public SpecialTopicCollectionViewModel(PagerRequest request)
            : base(request)
        {
        }

        public SpecialTopicCollectionViewModel(PagerRequest request, int totalCount)
            : base(request, totalCount)
        {
        }

        public List<SpecialTopicViewModel> SpecialTopics { get; set; }
    }

    public class SpecialTopicViewModel : BaseViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(16, MinimumLength = 1)]
        [Display(Name = "名称")]
        public string Name { get; set; }

        [StringLength(256, MinimumLength = 0)]
        [Display(Name = "说明")]
        public string Description { get; set; }

        [Range(0, Int32.MaxValue)]
        [Display(Name = "类型")]
        public int Type { get; set; }


        public List<ResourceViewModel> Resources { get; set; }

        [Display(Name = "状态")]
        public int Status { get; set; }
        [Display(Name = "创建人")]
        public int CreatedUser { get; set; }
        [Display(Name = "创建日期")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }
        [Display(Name = "修改日期")]
        [DataType(DataType.DateTime)]
        public DateTime UpdatedDate { get; set; }
        [Display(Name = "修改人")]
        public int UpdatedUser { get; set; }
    }
    public class SpecialTopicListSearchOption
    {
        [Display(Name="专题代码")]
        public int? PId { get; set; }
        [Display(Name="专题名")]
        public string Name { get; set; }
        [Display(Name="状态")]
        public DataStatus? Status { get; set; }
        [Display(Name="排序")]
        public GenricOrder? OrderBy { get; set; }
    }

   
}
