using System.Runtime.Serialization;
using Yintai.Hangzhou.Contract.Response;

namespace Yintai.Hangzhou.Contract.DTO.Response.Brand
{
    [DataContract]
    public class BrandInfo : BaseResponse
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "englishname")]
        public string EnglishName { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [IgnoreDataMember]
        public int CreatedUser { get; set; }

        [IgnoreDataMember]
        public System.DateTime CreatedDate { get; set; }

        [IgnoreDataMember]
        public System.DateTime UpdatedDate { get; set; }

        [IgnoreDataMember]
        public int UpdatedUser { get; set; }

        [DataMember(Name = "logo")]
        public string Logo { get; set; }

        [DataMember(Name = "website")]
        public string WebSite { get; set; }

        [IgnoreDataMember]
        public int Status { get; set; }
    }

    [DataContract]
    public class BrandInfoResponse : BrandInfo
    {

    }
}
