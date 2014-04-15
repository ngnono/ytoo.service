using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Intime.O2O.ApiClient.Domain
{
      [DataContract]
    public class Head
    {
        //订单号
        [DataMember(Name = "ID")]
        public string id { get; set; }
        //同订单号
        [DataMember(Name = "MAINID")]
        public string mainid { get; set; }
        //标识  1
        [DataMember(Name = "FLAG")]

        public int flag { get;set;}
        [DataMember(Name = "CREATETIME")]

        public DateTime createtime { get; set; }
        [DataMember(Name = "PAYTIME")]

        //收银时间
        public DateTime paytime { get; set; }
        [DataMember(Name = "TYPE")]

        //   1
        public int type { get; set; }
        [DataMember(Name = "STATUS")]

        //状态
        public int status { get; set; }
        [DataMember(Name = "QUANTITY")]

        //数量
        public int quantity { get; set; }
        [DataMember(Name = "DISCOUNT")]


        public decimal discount { get; set; }
        [DataMember(Name = "TOTAL")]

        public decimal total { get; set; }
        [DataMember(Name = "VIPNO")]

        public string vipno { get; set; }
        [DataMember(Name = "VIPMEMO")]

        public string vipmemo { get; set; }
        [DataMember(Name = "STRORENO")]

        public string storeno { get; set; }
        [DataMember(Name = "OLDID")]

        public string oldid { get; set; }
        [DataMember(Name = "OPERID")]

        public string operid { get; set; }
        [DataMember(Name = "OPERNAME")]

        public string opername { get; set; }
        [DataMember(Name = "OPERTIME")]

        public string opertime { get; set; }


    }
}
