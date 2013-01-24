using System;
using System.Collections.Generic;

namespace Yintai.Hangzhou.Data.Models
{
    public partial class NotificationLogEntity : Yintai.Architecture.Common.Models.BaseEntity
    {
        public int Id { get; set; }
        public string DeviceToken { get; set; }
        public Nullable<System.DateTime> NotifyDate { get; set; }
        public string Message { get; set; }
        public Nullable<System.DateTime> InDate { get; set; }
        public Nullable<int> Status { get; set; }

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
