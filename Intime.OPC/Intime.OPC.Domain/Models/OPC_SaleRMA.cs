using System;

namespace Intime.OPC.Domain.Models
{
    public class OPC_SaleRMA
    {
        public int Id { get; set; }
        public int SaleId { get; set; }
        public string Reason { get; set; }
        public DateTime BackDate { get; set; }
        public int Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int UpdatedUser { get; set; }
    }
}