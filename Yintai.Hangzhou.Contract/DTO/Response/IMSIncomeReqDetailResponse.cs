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
    public class IMSIncomeReqDetailResponse : BaseResponse
    {
        [DataMember(Name="id")]
        public int Id { get; set; }
        [DataMember(Name="bank_name")]
        public string BankName { get; set; }
        [DataMember(Name="card_no")]
        public string BankNo { get; set; }
        [DataMember(Name="amount")]
        public decimal Amount { get; set; }
        [DataMember(Name = "status")]
        public int Status { get; set; }
        [DataMember(Name="create_date")]
        public System.DateTime CreateDate { get; set; }
        
    }
}
