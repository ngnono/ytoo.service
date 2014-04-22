using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yintai.Hangzhou.Model.ES
{
    public class ESProduct
    {
        private DateTime _createdDate;
        public int Id { get; set; }
        public string Name {get;set;}
        public string Description { get; set; }
        public DateTime CreatedDate
        {
            get {
                return _createdDate.ToUniversalTime();
            }
            set
            {
            _createdDate = value;
        } }
        public decimal Price { get; set; }
        public string RecommendedReason { get; set; }
        public int Status { get; set; }
        public ESStore Store { get; set; }  
        public ESTag Tag { get; set; }
        public ESBrand Brand { get; set; }
        public int SortOrder { get; set; }
        public IEnumerable<ESResource> Resource { get; set; }
        public IEnumerable<ESPromotion> Promotion { get; set; }
        public IEnumerable<ESSpecialTopic> SpecialTopic { get; set; }
        public int CreateUserId { get; set; }
        public bool Is4Sale { get; set; }
        public decimal? UnitPrice { get; set; }
        public int FavoriteCount { get; set; }
        public int ShareCount { get; set; }
        public int InvolvedCount { get; set; }
        public int RecommendUserId { get; set; }
        public ESSection Section { get; set; }
        public string UpcCode { get; set; }
        public bool IsSystem { get; set; }
        public decimal DiscountRate { get{
            return UnitPrice.HasValue ? (int)(Price / UnitPrice * 100) : 100;
        } }
    }
}
