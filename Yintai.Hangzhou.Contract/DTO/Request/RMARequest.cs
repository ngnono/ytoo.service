using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yintai.Hangzhou.Contract.DTO.Request
{
    public class RMARequest:BaseRequest
    {
        public string OrderNo { get; set; }
        public string Reason { get; set; }      
        public string BankName { get; set; }
        public string BankAccount { get; set; }
        public string BankCard { get; set; }
       
    }
}
