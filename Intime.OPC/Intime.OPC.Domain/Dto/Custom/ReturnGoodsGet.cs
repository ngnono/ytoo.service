using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intime.OPC.Domain.Dto.Custom
{
    public  class ReturnGoodsGet
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string  OrderNo { get; set; }

        public string Telephone { get; set; }

        public string   PayType { get; set; }

        public int? BandId { get; set; }
    }
}
