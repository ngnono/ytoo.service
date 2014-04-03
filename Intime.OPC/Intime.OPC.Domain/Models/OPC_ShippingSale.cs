
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
        public string OrderNo { get; set; }
        public int Id { get; set; }
        public int? StoreId { get; set; }
        public int? BrandId { get; set; }
        public string ShippingZipCode { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingContactPerson { get; set; }
        public string ShippingContactPhone { get; set; }
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