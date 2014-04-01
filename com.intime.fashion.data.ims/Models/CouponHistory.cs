using System;
using System.Collections.Generic;

namespace Yintai.Hangzhou.Data.Models
{
    public partial class CouponHistoryEntity : Yintai.Architecture.Common.Models.BaseEntity
    {
        public int Id { get; set; }
        public string CouponId { get; set; }
        public int User_Id { get; set; }
        public int CreatedUser { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int Status { get; set; }
        public int FromStore { get; set; }
        public int FromUser { get; set; }
        public int FromProduct { get; set; }
        public int FromPromotion { get; set; }
        public System.DateTime ValidStartDate { get; set; }
        public System.DateTime ValidEndDate { get; set; }
        public Nullable<bool> IsLimitOnce { get; set; }

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
