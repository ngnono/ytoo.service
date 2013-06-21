using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Yintai.Architecture.Common.Models;
using Yintai.Architecture.Framework.ServiceLocation;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Repository.Contract;

namespace Yintai.Hangzhou.Cms.WebSiteV1.Models
{
    public class TagViewModel : BaseViewModel
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
        [Display(Name = "排序值")]
        public int SortOrder { get; set; }

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

        public static IEnumerable<SelectListItem> ToSelectItems()
        {
            var tagRepo = ServiceLocator.Current.Resolve<ITagRepository>();
            foreach (var tag in tagRepo.Get(t=>t.Status!=(int)DataStatus.Deleted))
            {
                yield return new SelectListItem
                {
                    Text =tag.Name
                    ,
                    Value = (tag.Id).ToString()
                };
            }
        }
    }

    public class TagCollectionViewModel : PagerInfo, IViewModel
    {
        public TagCollectionViewModel(PagerRequest request)
            : base(request)
        {
        }

        public TagCollectionViewModel(PagerRequest request, int totalCount)
            : base(request, totalCount)
        {
        }

        public List<TagViewModel> Tags { get; set; }
    }
}
