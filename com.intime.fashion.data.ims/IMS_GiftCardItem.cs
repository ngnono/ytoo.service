namespace com.intime.fashion.data.ims
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class IMS_GiftCardItem
    {
        public int Id { get; set; }

        public int GiftCardId { get; set; }

        public decimal Price { get; set; }

        public decimal UnitPrice { get; set; }

        public int MaxLimit { get; set; }

        public int Status { get; set; }
    }
}
