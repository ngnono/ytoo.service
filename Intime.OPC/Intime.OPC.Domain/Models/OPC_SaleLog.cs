using System;

namespace Intime.OPC.Domain.Models
{
    public class OPC_SaleLog
    {
        public int Id { get; set; }
        public string SaleId { get; set; }
        public int Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedUser { get; set; }
    }
}