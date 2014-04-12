using System;
using System.Collections.Generic;

namespace Intime.OPC.Domain.Models
{
    public partial class PaymentMethod
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int Status { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public int UpdateUser { get; set; }
        public bool IsCOD { get; set; }
        public Nullable<int> AvailChannels { get; set; }
    }
}
