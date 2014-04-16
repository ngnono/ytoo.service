﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Yintai.Hangzhou.Contract.DTO.Request
{
    [DataContract]
    public class IMSComboUpdateRequest:BaseRequest
    {
        public int Id { get; set; }
        public int Product_Type { get; set; }
        public int[] Image_Ids { get; set; }
        public int[] ProductIds { get; set; }
        public string Desc { get; set; }
        public string Private_To { get; set; }
    }
}