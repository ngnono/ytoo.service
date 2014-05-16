namespace Intime.OPC.Domain.Models
{
    public partial class OPC_Sale
    {
        public string ShippingStatusName { get; set; }

        public virtual Store Store { get; set; }

        public virtual Section Section { get; set; }

        public virtual OrderTransaction OrderTransaction { get; set; }
    }
}