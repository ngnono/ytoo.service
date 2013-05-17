using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;

namespace Yintai.Hangzhou.Cms.WebSiteCoreV1.Models
{
    public class HotwordViewModel:BaseViewModel
    {
        [Key]
        [Display(Name = "编码")]
        public int Id { get; set; }

        [Display(Name = "优先级")]
        public int SortOrder { get; set; }

        [Display(Name = "类型")]
        public int Type { get; set; }

        [Display(Name = "关键词")]
        public string Word { get; set; }

        [Display(Name = "品牌代码")]
        [UIHint("Association")]
        [AdditionalMetadata("controller", "brand")]
        [AdditionalMetadata("displayfield", "Name")]
        [AdditionalMetadata("searchfield", "name")]
        [AdditionalMetadata("valuefield", "Id")]
        public int? BrandId { get; set; }

        [Display(Name = "品牌名")]
        public string BrandName { get; set; }

        [Display(Name = "门店代码")]
        [UIHint("Association")]
        [AdditionalMetadata("controller", "store")]
        [AdditionalMetadata("displayfield", "Name")]
        [AdditionalMetadata("searchfield", "name")]
        [AdditionalMetadata("valuefield", "Id")]
        public int? StoreId { get; set; }

        [Display(Name = "门店")]
        public string StoreName { get; set; }

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

        public override T FromEntity<T>(dynamic entity)
        {         
            HotwordViewModel viewModel = base.FromEntity<T>(entity as HotWordEntity) as HotwordViewModel;
            if (viewModel.Type == (int)HotWordType.BrandStruct)
            {
                dynamic brandEntity =JsonConvert.DeserializeObject<dynamic>(viewModel.Word); 
                viewModel.BrandId = brandEntity.id;
                viewModel.BrandName = brandEntity.name;
                viewModel.Word = string.Empty;
            }
            else if (viewModel.Type == (int)HotWordType.Stores)
            {
                dynamic storeEntity = JsonConvert.DeserializeObject<dynamic>(viewModel.Word);
                viewModel.StoreId = storeEntity.id;
                viewModel.StoreName = storeEntity.name;
                viewModel.Word = string.Empty;
            }
            return viewModel as T;
        }
        public override T ToEntity<T>()
        {
            HotWordEntity entity = base.ToEntity<T>() as HotWordEntity;
            if (this.Type == (int)HotWordType.BrandStruct)
            {
                entity.Word = BrandString;
            }
            else if (this.Type == (int)HotWordType.Stores)
            {
                entity.Word = StoreString;
            }
            return entity as T;
        }
       
        public string BrandString {
            get {
                return JsonConvert.SerializeObject(new { id = this.BrandId, name = this.BrandName });
            }
        }
        public string StoreString
        {
            get
            {
                return JsonConvert.SerializeObject(new { id = this.StoreId, name = this.StoreName });
            }
        }
    }

}

