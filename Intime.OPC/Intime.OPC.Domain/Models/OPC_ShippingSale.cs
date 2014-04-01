
using System;
using System.Collections.Generic;
using System.Text;
using Intime.OPC.Domain.Base;

namespace Intime.OPC.Domain.Models
{

    public partial class OPC_ShippingSale:IEntity
    {
        public OPC_ShippingSale()
        {
        }

        public int Id { get; set; }
        public string SaleOrderNo { get; set; }
        public int? ShipViaId { get; set; }
        public string ShipViaName { get; set; }
        public string ShippingCode { get; set; }
        public decimal? ShippingFee { get; set; }
        public int? ShippingStatus { get; set; }
        public string ShippingRemark { get; set; }
        public System.DateTime? CreateDate { get; set; }
        public int? CreateUser { get; set; }
        public System.DateTime? UpdateDate { get; set; }
        public int? UpdateUser { get; set; }

    }
}