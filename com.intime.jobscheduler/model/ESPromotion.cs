using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.intime.jobscheduler.Job
{
    class ESPromotion
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public int Status { get; set; }
        public int FavoriteCount { get; set; }
        public ESStore Store { get; set; }
        public bool IsTop { get; set; }
        public virtual IEnumerable<ESResource> Resource { get; set; }
        public int CreateUserId { get; set; }

    }
}
