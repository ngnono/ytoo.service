using System;
using System.Collections.Generic;

namespace Yintai.Hangzhou.Data.Models
{
    public partial class OPC_StockPropertyValueEntity : Yintai.Architecture.Common.Models.BaseEntity
    {
        public int Id { get; set; }
       public int InventoryId { get; set; }
        public int StockPropertyId { get; set; }
        public string ValueDesc { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public int Status { get; set; }
        public int ChannelValueId { get; set; }

        #region Overrides of BaseEntity

        /// <summary>
        /// KeyMemberId
        /// </summary>
        public override object EntityId
        {       
                get { return Id; }
 
        }

        #endregion
    }
}
