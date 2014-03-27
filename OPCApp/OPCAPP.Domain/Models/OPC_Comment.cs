using System;

namespace Intime.OPC.Domain.Models
{
    public class OPC_Comment
    {
        public int Id { get; set; }
        public string RelationId { get; set; }
        public string Content { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateUser { get; set; }
    }
}