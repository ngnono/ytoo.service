using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Intime.O2O.ApiClient.Domain
{
    [DataContract]
    public class Payment
    {
        [DataMember(Name = "ID")]
        public string id { get; set; }
        [DataMember(Name = "TYPE")]

        public string type { get; set; }
        [DataMember(Name = "TYPEID")]

        public int typeid { get; set; }
        [DataMember(Name = "TYPENAME")]

        public string typename { get; set; }
        [DataMember(Name = "NO")]

        public string no { get; set; }
        [DataMember(Name = "AMOUNT")]

        public decimal amount { get; set; }
        [DataMember(Name = "ROWNO")]

        public int rowno { get; set; }
        [DataMember(Name = "MENO")]

        public string memo { get; set; }
        [DataMember(Name = "STORENO")]

        public string storeno { get; set; }

    }
}
