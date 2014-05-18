namespace Intime.OPC.Domain.Models
{
<<<<<<< HEAD
    public partial class SaleOrderModel : OPC_Sale
=======
    public partial class OPC_Sale_Partial
>>>>>>> d5b656a06e4c5c27bdaf90677938daa89e5bb86b
    {
        public virtual Store Store { get; set; }

        public virtual Section Section { get; set; }

        public virtual OrderTransaction OrderTransaction { get; set; }
    }
}