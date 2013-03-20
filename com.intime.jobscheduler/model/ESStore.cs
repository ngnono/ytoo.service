using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.intime.jobscheduler.Job
{
    class ESStore
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string Tel { get; set; }
        [ElasticProperty(Type=FieldType.geo_point)]
        public Location Location { get; set; }
        public Nullable<decimal> GpsLat { get; set; }
        public Nullable<decimal> GpsLng { get; set; }
        public Nullable<decimal> GpsAlt { get; set; }

    }

    class Location
    {
        public decimal Lat { get; set; }  
        public decimal Lon { get; set; }

    }
}
