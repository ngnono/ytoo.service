using System.Runtime.Serialization;

namespace Yintai.Hangzhou.Service.Contract.Apis
{
    internal interface IGroupCardService
    {
        GroupCardPointResult GetPoint(GroupCardPointRequest request);

        GroupCardInfoResult GetInfo(GroupCardInfoRequest request);
    }

    public class GroupCardInfoRequest
    {
        public string CardNo { get; set; }
        public string Passwd { get; set; }
    }

    public class GroupCardPointRequest
    {
        public string CardNo { get; set; }
    }

    [DataContract]
    public abstract class GroupCardResult
    {
    }

    [DataContract]
    public class GroupCardPointResult : GroupCardResult
    {
        [DataMember(Name = "Success")]
        public bool Success { get; set; }

        [DataMember(Name = "ErrorCode")]
        public int ErrorCode { get; set; }

        [DataMember(Name = "Desc")]
        public string Desc { get; set; }

        [DataMember(Name = "Point")]
        public int Point { get; set; }
    }

    [DataContract]
    public class GroupCardInfoResult : GroupCardResult
    {
        [DataMember(Name = "Ret")]
        public int Ret { get; set; }

        [DataMember(Name = "Desc")]
        public string Desc { get; set; }

        [DataMember(Name = "Lvl")]
        public string Lvl { get; set; }

        [DataMember(Name = "Type")]
        public string Type { get; set; }
    }
}
