using System;
using System.Collections.Generic;
using Intime.OPC.Domain.Base;

namespace Intime.OPC.Domain.Models
{
    public partial class OPC_OrgInfo:IEntity
    {
        public int Id { get; set; }
        public string OrgName { get; set; }
        public string ParentID { get; set; }
        public Nullable<int> StoreID { get; set; }
        public string StoreName { get; set; }
        public Nullable<int> OrgType { get; set; }
        public Nullable<bool> IsDel { get; set; }
    }
}
