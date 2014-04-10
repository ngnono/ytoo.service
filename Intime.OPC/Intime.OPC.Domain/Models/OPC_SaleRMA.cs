using System;
using System.Collections.Generic;
using Intime.OPC.Domain.Base;

namespace Intime.OPC.Domain.Models
{
    public partial class OPC_SaleRMA:IEntity
    {
        public int Id { get; set; }
        public string SaleOrderNo { get; set; }
        public string Reason { get; set; }
        public System.DateTime BackDate { get; set; }
        public int Status { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int CreatedUser { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public int UpdatedUser { get; set; }
        public Nullable<decimal> StoreFee { get; set; }
        public Nullable<decimal> CustomFee { get; set; }
        public string RMAMemo { get; set; }
        public Nullable<decimal> CompensationFee { get; set; }
        public Nullable<int> RMACount { get; set; }
        public Nullable<int> SectionId { get; set; }
        public string OrderNo { get; set; }
        public string SaleRMASource { get; set; }
        public string RMAStatus { get; set; }
        public string RMACashStatus { get; set; }
        public string RMANo { get; set; }

        public int StoreId { get; set; }
        public Nullable<decimal> RealRMASumMoney { get; set; }

        public DateTime? ServiceAgreeTime { get; set; }
    }
}
