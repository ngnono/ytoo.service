using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intime.OPC.Domain.Models
{
    [NotMapped]
    public partial class SaleOrderModel : OPC_Sale
    {
        public virtual Store Store { get; set; }

        public virtual Section Section { get; set; }

        public virtual OrderTransaction OrderTransaction { get; set; }

        public virtual OPC_ShippingSale ShippingSale { get; set; }
    }
}