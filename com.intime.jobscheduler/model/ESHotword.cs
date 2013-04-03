using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.intime.jobscheduler
{
    class ESHotword
    {
        public int Id { get; set; }
        public int Status { get; set; }
        public int Type { get; set; }
        public int SortOrder { get; set; }
        public string Word { get; set; }
        public int BrandId { get; set; }
    }
}
