﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Yintai.Hangzhou.Contract.Response;
using com.intime.fashion.common.Extension;

namespace Yintai.Hangzhou.Contract.DTO.Response
{
    [DataContract]
    public class IMSComboDetailResponse : BaseResponse
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }
        [DataMember(Name = "desc")]
        public string Desc { get; set; }
        [DataMember(Name = "price")]
        public decimal Price { get; set; }
        [DataMember(Name = "private_desc")]
        public string Private2Name { get; set; }
        [DataMember(Name = "is_online")]
        public bool Status_B { get {
            return Status == 1 && (!ExpireDate.HasValue || ExpireDate>DateTime.Now) ? true : false;
        } }
        [DataMember(Name = "user_id")]
        public int UserId { get; set; }
        [DataMember(Name = "create_date")]
        public System.DateTime CreateDate { get; set; }
        [DataMember(Name = "owner_id")]
        public int CreateUser { get; set; }
        [DataMember(Name = "online_date")]
        public System.DateTime OnlineDate { get; set; }
        [DataMember(Name = "update_date")]
        public System.DateTime UpdateDate { get; set; }
        [DataMember(Name = "update_user")]
        public int UpdateUser { get; set; }
        [DataMember(Name = "image")]
        public string Image
        {
            get
            {
                return ImageUrl.Image320Url();
            }
        }
        [DataMember(Name="images")]
        public IEnumerable<string> Images { get; set; }
        [DataMember(Name="products")]
        public IEnumerable<IMSProductDetailResponse> Products { get; set; }
        [DataMember(Name="is_owner")]
        public bool Is_Owner { get; set; }
         [DataMember(Name = "is_favored")]
        public bool Is_Favored { get; set; }
        [DataMember(Name="expire_in")]
         public int? ExpiredIn { get {
             if (!ExpireDate.HasValue)
                 return null;
             var expireSpan = (int)((ExpireDate.Value - DateTime.Now).TotalSeconds);
             return expireSpan < 0 ? 0 : expireSpan ;
         } }

        [IgnoreDataMember]
        public string ImageUrl { get; set; }
        [IgnoreDataMember]
        public int Status { get; set; }
        [IgnoreDataMember]
        public DateTime? ExpireDate { get; set; }
    }
}