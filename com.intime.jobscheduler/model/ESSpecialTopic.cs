using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.intime.jobscheduler
{
    class ESSpecialTopic
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreateUser { get; set; }
        public IEnumerable<ESResource> Resource { get; set; }
    }
}
