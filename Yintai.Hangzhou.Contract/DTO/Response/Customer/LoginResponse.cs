using System.Runtime.Serialization;
using Yintai.Hangzhou.Contract.DTO.Response.Favorite;
using Yintai.Hangzhou.Contract.Response;
using Yintai.Hangzhou.Model;
using Yintai.Hangzhou.Model.Enums;

namespace Yintai.Hangzhou.Contract.DTO.Response.Customer
{
    [DataContract(Name = "darencustomerinfo")]
    public class DarenCustomerInfoResponse : UserInfoResponse
    {
        public FavoriteCollectionResponse FavoriteCollectionResponse { get; set; }
    }

    [DataContract(Name = "showcustomerinfo")]
    public class ShowCustomerInfoResponse : UserInfoResponse
    {
        /// <summary>
        /// 当前发起请求的用户是否关注当前的达人
        /// </summary>
        [DataMember(Name = "isliked")]
        public bool IsLiked { get; set; }
    }

    [DataContract(Name = "userinfo")]
    public abstract class UserInfoResponse : BaseResponse
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [IgnoreDataMember]
        public string Password { get; set; }

        [DataMember(Name = "nickname")]
        public string Nickname { get; set; }

        [IgnoreDataMember]
        public int CreatedUser { get; set; }

        [IgnoreDataMember]
        public System.DateTime CreatedDate { get; set; }

        [IgnoreDataMember]
        public int UpdatedUser { get; set; }

        [IgnoreDataMember]
        public System.DateTime UpdatedDate { get; set; }

        [IgnoreDataMember]
        public System.DateTime LastLoginDate { get; set; }

        [DataMember(Name = "mobile")]
        public string Mobile { get; set; }

        [DataMember(Name = "email")]
        public string EMail { get; set; }

        [IgnoreDataMember]
        public int Status { get; set; }

        [DataMember(Name = "level")]
        public int UserLevel { get; set; }

        [DataMember(Name = "region_id")]
        public int Region_Id { get; set; }

        [DataMember(Name = "logo")]
        public string Logo { get; set; }

        [IgnoreDataMember]
        public UserLevel Level { get; set; }

        [DataMember(Name = "region")]
        public RegionModel Region { get; set; }

        [DataMember(Name = "desc")]
        public string Description { get; set; }

        [DataMember(Name = "gender")]
        public byte Gender { get; set; }

        //[DataMember(Name = "accounts")]
        //public List<CustomerAccountResponse> Accounts { get; set; }

        [DataMember(Name = "coupontotal")]
        public int CouponCount { get; set; }

        [DataMember(Name = "pointtotal")]
        public int PointCount { get; set; }

        /// <summary>
        /// 我喜欢数
        /// </summary>
        [DataMember(Name = "liketotal")]
        public int ILikeCount { get; set; }

        /// <summary>
        /// 喜欢我数
        /// </summary>
        [DataMember(Name = "likedtotal")]
        public int LikeMeCount { get; set; }

        /// <summary>
        /// 收藏数
        /// </summary>
        [DataMember(Name = "favortotal")]
        public int FavorCount { get; set; }

        /// <summary>
        /// 消费次数
        /// </summary>
        [DataMember(Name = "consumptiontotal")]
        public int ConsumptionCount { get; set; }

        /// <summary>
        /// 分享数
        /// </summary>
        [DataMember(Name = "sharetotal")]
        public int ShareCount { get; set; }
    }

    [DataContract(Name = "customerinfo")]
    public class CustomerInfoResponse : UserInfoResponse
    {
        [DataMember(Name = "token", Order = 1)]
        public string Token { get; set; }

        /// <summary>
        /// APP store 评论页地址
        /// </summary>
        [DataMember(Name = "appid")]
        public string AppId { get; set; }
    }
}
