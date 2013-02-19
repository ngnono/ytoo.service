using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yintai.Hangzhou.Cms.WebSiteCoreV1.Models;
using Yintai.Hangzhou.Model.Enums;

namespace Yintai.Hangzhou.Cms.WebSiteCoreV1.Dto.Resource
{
    public abstract class DtoBase
    {
    }

    public class ListDto : DtoBase
    {
        public int? Sort { get; set; }
        public SourceType? SourceType { get; set; }
        public int? SourceId { get; set; }

        public ResourceCollectionViewModel ResourceCollectionViewModel { get; set; }
    }
}
