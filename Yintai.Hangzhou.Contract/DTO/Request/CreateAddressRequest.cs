using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yintai.Hangzhou.Contract.DTO.Request
{
    public class CreateAddressRequest:AuthRequest
    {
        public string ShippingContactPerson { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingContactPhone { get; set; }
        public string ShippingZipCode { get; set; }
        public string ShippingProvince { get; set; }
        public int ShippingProvinceId { get; set; }
        public string ShippingCity { get; set; }
        public int? ShippingCityId { get; set; }
    }
}
