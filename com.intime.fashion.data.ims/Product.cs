namespace com.intime.fashion.data.ims
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Product")]
    public partial class Product
    {
        public int Id { get; set; }

        [Required]
        [StringLength(64)]
        public string Name { get; set; }

        public int Brand_Id { get; set; }

        [Required]
        public string Description { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedUser { get; set; }

        public DateTime UpdatedDate { get; set; }

        public int UpdatedUser { get; set; }

        public decimal Price { get; set; }

        [Required]
        public string RecommendedReason { get; set; }

        [Required]
        public string Favorable { get; set; }

        public int RecommendUser { get; set; }

        public int Status { get; set; }

        public int Store_Id { get; set; }

        public int Tag_Id { get; set; }

        public int FavoriteCount { get; set; }

        public int ShareCount { get; set; }

        public int InvolvedCount { get; set; }

        public int RecommendSourceId { get; set; }

        public int RecommendSourceType { get; set; }

        public int SortOrder { get; set; }

        public bool IsHasImage { get; set; }

        public decimal? UnitPrice { get; set; }

        public bool? Is4Sale { get; set; }

        [StringLength(50)]
        public string SkuCode { get; set; }

        [StringLength(100)]
        public string BarCode { get; set; }

        [StringLength(1000)]
        public string MoreDesc { get; set; }

        public int? ProductType { get; set; }
    }
}
