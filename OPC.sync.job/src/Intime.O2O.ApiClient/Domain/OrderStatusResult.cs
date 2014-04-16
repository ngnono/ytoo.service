using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Intime.O2O.ApiClient.Domain
{
    [DataContract]
    public class OrderStatusResult
    {
        //返回值  成功 1
        [DataMember(Name = "RET")]
        public string ret { get; set; }

        //执行状态，中文
        [DataMember(Name = "DESC")]
        public string desc { get; set; }
        //
        [DataMember(Name = "POSNO")]
        public string posno { get; set; }
        //
        [DataMember(Name = "FLAG")]
        public int  flag { get; set; }
        //
        [DataMember(Name = "ERROR")]
        public string error { get; set; }
        //
        [DataMember(Name = "STATUS")]
        public string status { get; set; }

        //
        [DataMember(Name = "POSTIME")]
        public string postime { get; set; }

        //
        [DataMember(Name = "POSSEQNO")]
        public string posseqno { get; set; }

    }
}
