using System;
using System.Collections.Generic;

namespace Yintai.Hangzhou.Data.Models
{
    public partial class InventoryBackupEntity : Yintai.Architecture.Common.Models.BaseEntity
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int PColorId { get; set; }
        public int PSizeId { get; set; }
        public int Amount { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public int UpdateUser { get; set; }
        public int ChannelInventoryId { get; set; }

        #region Overrides of BaseEntity

        /// <summary>
        /// KeyMemberId
        /// </summary>
        public override object EntityId
        {       
                get { return new Dictionary<string, object> (8){
                {"Id",Id}, {"ProductId",ProductId}, {"PColorId",PColorId}, {"PSizeId",PSizeId}, {"Amount",Amount}, {"UpdateDate",UpdateDate}, {"UpdateUser",UpdateUser}, {"ChannelInventoryId",ChannelInventoryId} 
                };}
 
        }

        #endregion
    }
}
