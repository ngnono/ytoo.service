using System.Collections.Generic;

namespace OPCApp.Domain.Dto
{
    public class ShippingSaleCreateDto
    {
        public ShippingSaleCreateDto()
        {
            SaleOrderIDs = new List<string>();
        }

        public double ShippingFee { get; set; }

        public string OrderNo { get; set; }

        public string ShippingStatusName { get; set; }

        public string ShippingCode { get; set; }

        public int ShipViaID { get; set; }

        public string ShipViaName { get; set; }
        public string RmaNo { get; set; }
        public IList<string> SaleOrderIDs { get; set; }
    }
}