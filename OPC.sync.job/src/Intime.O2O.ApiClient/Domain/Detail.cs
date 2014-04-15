using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Intime.O2O.ApiClient.Domain
{
      [DataContract]
    public class Detail
    {
        [DataMember(Name = "ID")]
        public string id { get; set; }
        [DataMember(Name = "PRODUCTID")]
        public string productid { get; set; }
        [DataMember(Name = "PRODUCTNAME")]
        public string productname { get; set; }
        [DataMember(Name = "PRICE")]

        public decimal price { get; set; }
        [DataMember(Name = "DISCOUNT")]

        public decimal discount { get; set; }
        [DataMember(Name = "VIPDISCOUNT")]

        public int vipdiscount { get; set; }
        [DataMember(Name = "QUANTITY")]

        public int quantity { get; set; }
        [DataMember(Name = "TOTAL")]

        public decimal total { get; set; }
        [DataMember(Name = "ROWNO")]

        public int rowno { get; set; }

        [DataMember(Name = "COMCODE")]

        public string comcode { get; set; }
        [DataMember(Name = "COUNTER")]

        public string counter { get; set; }
        [DataMember(Name = "MEMO")]

        public string memo { get; set; }
        [DataMember(Name = "STORENO")]

        public string storeno { get; set; }

    }
}
