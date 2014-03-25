namespace com.intime.fashion.data.ims
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("OrderItem")]
    public partial class OrderItem
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string OrderNo { get; set; }

        public int ProductId { get; set; }

        [Required]
        [StringLength(200)]
        public string ProductDesc { get; set; }

        [StringLength(20)]
        public string StoreItemNo { get; set; }

        [StringLength(200)]
        public string StoreItemDesc { get; set; }

        public int Quantity { get; set; }

        public decimal? UnitPrice { get; set; }

        public decimal ItemPrice { get; set; }

        public decimal ExtendPrice { get; set; }

        public int BrandId { get; set; }

        public int StoreId { get; set; }

        public int CreateUser { get; set; }

        public DateTime CreateDate { get; set; }

        public int UpdateUser { get; set; }

        public DateTime UpdateDate { get; set; }

        public int Status { get; set; }

        [StringLength(200)]
        public string ProductName { get; set; }

        public int? Points { get; set; }

        [StringLength(20)]
        public string SalesPerson { get; set; }

        public int? SizeId { get; set; }

        public int? ColorId { get; set; }

        public int? SizeValueId { get; set; }

        public int? ColorValueId { get; set; }

        [StringLength(50)]
        public string ColorValueName { get; set; }

        [StringLength(50)]
        public string SizeValueName { get; set; }

        public int? ProductType { get; set; }
    }
}
