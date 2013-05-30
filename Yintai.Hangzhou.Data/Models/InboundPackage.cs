using System;
using System.Collections.Generic;

namespace Yintai.Hangzhou.Data.Models
{
    public partial class InboundPackageEntity : Yintai.Architecture.Common.Models.BaseEntity
    {
        public int Id { get; set; }
        public string SourceNo { get; set; }
        public int SourceType { get; set; }
        public int ShippingVia { get; set; }
        public string ShippingNo { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int CreateUser { get; set; }

        #region Overrides of BaseEntity

        /// <summary>
        /// KeyMemberId
        /// </summary>
        public override object EntityId
        {       
                get { return new Dictionary<string, object> (7){
                {"Id",Id}, {"SourceNo",SourceNo}, {"SourceType",SourceType}, {"ShippingVia",ShippingVia}, {"ShippingNo",ShippingNo}, {"CreateDate",CreateDate}, {"CreateUser",CreateUser} 
                };}
 
        }

        #endregion
    }
}
