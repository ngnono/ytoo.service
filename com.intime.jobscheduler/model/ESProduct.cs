using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.intime.jobscheduler.Job
{
    class ESProduct
    {
        public int Id { get; set; }
        public string Name {get;set;}
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal Price { get; set; }
        public string RecommendedReason { get; set; }
        public int Status { get; set; }
        public ESStore Store { get; set; }  
        public ESTag Tag { get; set; }
        public ESBrand Brand { get; set; }
        public int SortOrder { get; set; }
        public virtual IEnumerable<ESResource> Resource { get; set; }
        public IEnumerable<ESPromotion> Promotion { get; set; }
        public IEnumerable<ESSpecialTopic> SpecialTopic { get; set; }
        public int CreateUserId { get; set; }
    }
}
