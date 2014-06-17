using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yintai.Hangzhou.Model.ESModel
{
    public class ESAnalysisStore
    {
        public string Id { get; set; }

        public string Count { get; set; }

        public int? SectionId { get; set; }

        public int StoreId { get; set; }

        public int AssociateId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
