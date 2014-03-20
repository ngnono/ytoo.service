using System;
using System.Collections.Generic;
using Intime.OPC.Domain.Base;

namespace Intime.OPC.Domain.Models
{
    public partial class OPC_AuthRole:IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsValid { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int CreateUserId { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public Nullable<int> UpdateUserId { get; set; }
    }
}
