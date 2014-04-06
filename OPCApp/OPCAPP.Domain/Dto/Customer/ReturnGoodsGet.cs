using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPCApp.Domain.Customer
{
    public  class ReturnGoodsGet
    {
        public ReturnGoodsGet()
        {
            StartDate = DateTime.Now;
            EndDate = DateTime.Now;
        }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string  OrderNo { get; set; }

        public string Telephone { get; set; }

        public string   PayType { get; set; }

        public int? BandId { get; set; }
        public override string ToString()
        {

            return string.Format("StartDate={0}&EndDate={1}&OrderNo={2}&Telephone={2}&PayType={3}&BandId={4}", StartDate,EndDate, OrderNo, Telephone, PayType, BandId);
        }
    }
}
