using System;
using System.Collections.Generic;
using Intime.OPC.Domain.Base;

namespace Intime.OPC.Domain.Models
{
    public partial class OPC_ChannelProduct:IEntity
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int ColorId { get; set; }
        public int ChannelId { get; set; }
        public decimal Price { get; set; }
        public int Status { get; set; }
        public Nullable<System.DateTime> UpDateTime { get; set; }
        public Nullable<System.DateTime> DownDateTime { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int CreatedUser { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public int UpdatedUser { get; set; }
    }
}
