using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OPCApp.Domain.Models;


namespace OPCApp.Domain.Customer
{
   public  class PackageReceiveDto
    {
       public PackageReceiveDto()
       {
           StartDate = DateTime.Now;
           EndDate = DateTime.Now;
       }

       public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string OrderNo { get; set; }

        public string SaleOrderNo { get; set; }
       public override string ToString()
       {
           return string.Format("StartDate={0}&EndDate={1}&OrderNo={2}&SaleOrderNo={3}", StartDate, EndDate, OrderNo, SaleOrderNo);
       }
    }
}
