using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Yintai.Hangzhou.Contract.Response;

namespace Yintai.Hangzhou.Contract.DTO.Response.Card
{
    [DataContract]
    public class CardInfoResponse : BaseResponse
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "cardno")]
        public string CardNo { get; set; }

        [DataMember(Name = "amount")]
        public Decimal? Amount { get; set; }

        private DateTime? _lastDate;

        [DataMember(Name = "lastdate")]
        public DateTime? LastDate
        {
            get { return _lastDate; }
            set { _lastDate = value; }
        }

        [IgnoreDataMember]
        public int Type { get; set; }

        [IgnoreDataMember]
        public int User_Id { get; set; }

        [IgnoreDataMember]
        public int Status { get; set; }

        [IgnoreDataMember]
        public System.DateTime CreatedDate { get; set; }

        [IgnoreDataMember]
        public int CreatedUser { get; set; }

        [IgnoreDataMember]
        public System.DateTime UpdatedDate { get; set; }

        [IgnoreDataMember]
        public int UpdatedUser { get; set; }
    }
}
