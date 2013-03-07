using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Yintai.Hangzhou.Contract.DTO.Response.Resources;
using Yintai.Hangzhou.Contract.Response;

namespace Yintai.Hangzhou.Contract.DTO.Response.Store
{
    [DataContract]
    public class StoreInfoResponse : BaseResponse
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "location")]
        public string Location { get; set; }

        [DataMember(Name = "tel")]
        public string Tel { get; set; }

        [IgnoreDataMember]
        public int CreatedUser { get; set; }

        [IgnoreDataMember]
        public System.DateTime CreatedDate { get; set; }

        [IgnoreDataMember]
        public System.DateTime UpdatedDate { get; set; }

        [IgnoreDataMember]
        public int UpdatedUser { get; set; }

        [DataMember(Name = "lng")]
        public double Longitude { get; set; }

        [DataMember(Name = "lat")]
        public double Latitude { get; set; }

        [IgnoreDataMember]
        public int Group_Id { get; set; }

        [IgnoreDataMember]
        public int Status { get; set; }

        [IgnoreDataMember]
        public int Region_Id { get; set; }

        [IgnoreDataMember]
        public int StoreLevel { get; set; }

        /// <summary>
        /// 距离 单位 米
        /// </summary>
        [DataMember(Name = "distance")]
        public double Distance { get; set; }

        [DataMember(Name = "resources")]
        public List<ResourceInfoResponse> ResourceInfoResponses { get; set; }

        [IgnoreDataMember]
        public Nullable<double> GpsLat { get; set; }

        [IgnoreDataMember]
        public Nullable<double> GpsLng { get; set; }

        [IgnoreDataMember]
        public Nullable<double> GpsAlt { get; set; }
    }
}
