using System;

namespace Intime.OPC.Domain.Models
{
    public class OPC_StorePriority
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
        public int Priority { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateUser { get; set; }
        public DateTime UpdateDate { get; set; }
        public int UpdateUser { get; set; }
    }
}