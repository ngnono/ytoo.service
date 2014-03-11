using com.intime.fashion.common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.Service.Manager;

namespace Yintai.Hangzhou.Cms.WebSiteCoreV1.Models
{
    public class ResourceCollectionViewModel : PagerInfo, IViewModel
    {
        public ResourceCollectionViewModel(PagerRequest request)
            : base(request)
        {
        }

        public ResourceCollectionViewModel(PagerRequest request, int totalCount)
            : base(request, totalCount)
        {
        }

        public List<ResourceViewModel> Resources { get; set; }
    }

    public class ResourceViewModel : BaseViewModel
    {
        private string _domain;
        [Key]
        [Display(Name="资源代码")]
        public int Id { get; set; }

        [Required]
        [Range(0, Int32.MaxValue)]
        [Display(Name="来源代码")]
        public int SourceId { get; set; }

        [Required]
       // [Range(0, Int32.MaxValue)]
        [Display(Name = "来源类型")]
        public int SourceType { get; set; }

        [Required]
        [Display(Name = "文件名")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "扩展名")]
        public string ExtName { get; set; }

        [Required]
        [Display(Name = "域")]
        public string Domain
        {
            get
            {
                if (String.IsNullOrWhiteSpace(_domain))
                {
                    switch ((ResourceType)this.Type)
                    {
                        case ResourceType.Image:
                            _domain = ConfigManager.GetHttpApiImagePath();
                            break;
                        case ResourceType.Sound:
                            _domain = ConfigManager.GetHttpApiSoundPath();
                            break;
                        case ResourceType.Video:
                            _domain = ConfigManager.GetHttpApivideoPath();
                            break;
                        case ResourceType.Default:
                            _domain = ConfigManager.GetHttpApidefPath();
                            break;
                    }
                }
                return _domain;
            }
            set
            {
                _domain = value;
            }
        }

        [Display(Name = "是否默认图片")]
        public bool IsDefault { get; set; }

        [Range(0, Int32.MaxValue)]
        [Display(Name = "排序值")]
        public int SortOrder { get; set; }

        [Required]
        [Display(Name = "文件类型")]
        public int Type { get; set; }

        [Display(Name = "尺寸")]
        public string Size { get; set; }

        [Display(Name = "宽(px)")]
        public int Width { get; set; }
        [Display(Name = "高(px)")]
        public int Height { get; set; }

        [Display(Name = "文件大小")]
        public int ContentSize { get; set; }



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

        public string AudioUrl { get {
            return Path.Combine(Domain,Name+".mp3");
        }
        }
      

    }
}
