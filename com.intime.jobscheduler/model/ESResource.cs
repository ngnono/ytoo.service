using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.intime.jobscheduler
{
    class ESResource
    {
        public string Name { get; set; }
        public string Domain { get; set; }
        public int SortOrder { get; set; }
        public bool IsDefault { get; set; }
        public int Type { get; set; }
    }
}
