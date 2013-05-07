using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Yintai.Hangzhou.Contract.Response;

namespace Yintai.Hangzhou.Contract.DTO.Response
{
    public class ExchangeStoreCouponResponse:BaseResponse
    {
        [DataMember(Name="id")]
        public int Id { get; set; }
        [DataMember(Name = "code")]
        public string Code { get; set; }
        [DataMember(Name = "validstartdate")]
        public System.DateTime ValidStartDate { get; set; }
        [DataMember(Name = "validenddate")]
        public System.DateTime ValidEndDate { get; set; }

    }
}
