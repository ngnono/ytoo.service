using System;
using System.Runtime.Serialization;

namespace Intime.O2O.ApiClient.Domain
{
    [DataContract]
    public class PropertyValue
    {
        [DataMember(Name = "RN")]
        public int RN { get; set; }

        [DataMember(Name = "ID")]
        public int ID { get; set; }

        [DataMember(Name = "PROPERTYID")]
        public int PropertyId { get; set; }

        [DataMember(Name = "PROPERTYNAME")]
        public string PropertyName { get; set; }

        [DataMember(Name = "VALUE")]
        public string PropertyValueId { get; set; }

        [DataMember(Name = "MEMO")]
        public string ValueName { get; set; }

        [DataMember(Name = "ARTNO")]
        public string ProductCode { get; set; }

        [DataMember(Name = "SHOPPEID")]
        public string ShoppeId { get; set; }

        [DataMember(Name = "LASTUPDATE")]
        public LastUpdate LastUpdate { get; set; }

    }

    public class LastUpdate
    {
        [DataMember(Name = "date")]
        public int Date { get; set; }

        [DataMember(Name = "day")]
        public int Day { get; set; }

        [DataMember(Name = "hours")]
        public int Hours { get; set; }

        [DataMember(Name = "minutes")]
        public int Minutes { get; set; }

        [DataMember(Name = "month")]
        public int Month { get; set; }

        [DataMember(Name = "nanos")]
        public int Nanos { get; set; }

        [DataMember(Name = "seconds")]
        public int Seconds { get; set; }

        [DataMember(Name = "time")]
        public double Time { get; set; }

        [DataMember(Name = "timezoneOffset")]
        public double TimezoneOffset { get; set; }

        [DataMember(Name = "year")]
        public double Year { get; set; }

        public DateTime ConvertToTime()
        {
            throw new NotImplementedException();
        }

    }
}
