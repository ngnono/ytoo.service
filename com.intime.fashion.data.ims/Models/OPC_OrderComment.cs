using System;
using System.Collections.Generic;

namespace Yintai.Hangzhou.Data.Models
{
    public partial class OPC_OrderCommentEntity : Yintai.Architecture.Common.Models.BaseEntity
    {
        public int Id { get; set; }
        public string OrderNo { get; set; }
        public string Content { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int CreateUser { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public int UpdateUser { get; set; }

        #region Overrides of BaseEntity

        /// <summary>
        /// KeyMemberId
        /// </summary>
        public override object EntityId
        {       
                get { return new Dictionary<string, object> (7){
                {"Id",Id}, {"OrderNo",OrderNo}, {"Content",Content}, {"CreateDate",CreateDate}, {"CreateUser",CreateUser}, {"UpdateDate",UpdateDate}, {"UpdateUser",UpdateUser} 
                };}
 
        }

        #endregion
    }
}
