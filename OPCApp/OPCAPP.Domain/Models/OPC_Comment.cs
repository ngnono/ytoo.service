using System;
using System.Collections.Generic;


namespace Intime.OPC.Domain.Models
{
    public partial class OPC_Comment
    {
        public int Id { get; set; }
        public string RelationId { get; set; }
        public string Content { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int CreateUser { get; set; }
    }
}
