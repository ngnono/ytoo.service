using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intime.OPC.Domain.Models
{
    public class OPC_SaleOrderNotificationLog
    {
        public int Id { get; set; }
        public string SaleOrderNo { get; set; }
        public int Status { get; set; }
        public System.DateTime CreateDate { get; set; }
        public int CreateUser { get; set; }
        public string Message { get; set; }
    }
}
