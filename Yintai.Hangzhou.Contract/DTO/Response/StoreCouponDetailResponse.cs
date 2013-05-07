using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Yintai.Hangzhou.Contract.Response;

namespace Yintai.Hangzhou.Contract.DTO.Response
{
    [DataContract]
    public class StoreCouponDetailResponse : BaseResponse
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }
        [DataMember(Name = "code")]
        public string Code { get; set; }
        [DataMember(Name = "validstartdate")]
        public Nullable<System.DateTime> ValidStartDate { get; set; }
        [DataMember(Name = "validenddate")]
        public Nullable<System.DateTime> ValidEndDate { get; set; }
        [DataMember(Name = "status")]
        public Nullable<int> Status { get; set; }

        [DataMember(Name = "createddate")]
        public Nullable<System.DateTime> CreateDate { get; set; }
        [DataMember(Name = "amount")]
        public Nullable<decimal> Amount { get; set; }

        [DataMember(Name = "promotion")]
        public StorePromotionDetailResponse Promotion { get; set; }

    }
    [DataContract]
    public class StorePromotionDetailResponse : BaseResponse
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "description")]
        public string Description { get; set; }
        [DataMember(Name = "activestartdate")]
        public Nullable<System.DateTime> ActiveStartDate { get; set; }
        [DataMember(Name = "activeenddate")]
        public Nullable<System.DateTime> ActiveEndDate { get; set; }
        [DataMember(Name = "notice")]
        public string Notice { get; set; }
        [DataMember(Name = "usagenotice")]
        public string UsageNotice { get; set; }
        [DataMember(Name = "inscopenotice")]
        public string InScopeNotice { get; set; }
    }
}
