using System;
using System.Collections.Generic;
using Intime.OPC.Domain.Base;

namespace OPCApp.Domain.Models
{
    public partial class OPC_RMAComment
    {
        public int Id { get; set; }
        public string RMANo { get; set; }
        public string Content { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int CreateUser { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public int UpdateUser { get; set; }
    }
}
