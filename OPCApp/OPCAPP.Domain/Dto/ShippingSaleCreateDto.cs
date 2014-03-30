
using System.Collections.Generic;

namespace OPCApp.Domain.Dto
{
    public  class ShippingSaleCreateDto
    {
        public ShippingSaleCreateDto()
        {
            SaleOrderIDs=new List<string>();
        }

        public double ShippingFee { get; set; }

        public string ShippingCode { get; set; }

        public int ShipViaID { get; set; }

        public string ShipViaName { get; set; }

        public IList<string> SaleOrderIDs { get; set; }

    }
}
