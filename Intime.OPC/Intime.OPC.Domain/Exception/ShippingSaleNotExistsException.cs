using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intime.OPC.Domain.Exception
{
   public  class ShippingSaleNotExistsException:System.Exception
    {
       public ShippingSaleNotExistsException(string saleOrderNo)
           : base(saleOrderNo)
        {
            this.SaleOrderNo = saleOrderNo;
        }

        public string SaleOrderNo { get; private set; }
    }

   public class ShippingSaleExistsException : System.Exception
   {
       public ShippingSaleExistsException(string saleOrderNo)
           : base(saleOrderNo)
       {
           this.SaleOrderNo = saleOrderNo;
       }

       public string SaleOrderNo { get; private set; }
   }
}
