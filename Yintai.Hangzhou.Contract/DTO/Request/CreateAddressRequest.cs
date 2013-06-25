using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yintai.Hangzhou.Contract.DTO.Request
{
    public class CreateAddressRequest:BaseRequest
    {
        public int Id { get; set; }
        [Required(ErrorMessage="联系人不能为空")]
        public string ShippingContactPerson { get; set; }
        [Required(ErrorMessage = "联系人地址")]
        public string ShippingAddress { get; set; }
        [Required(ErrorMessage = "联系人号码不能为空")]
        public string ShippingContactPhone { get; set; }
        [Required(ErrorMessage = "邮编不能为空")]
        public string ShippingZipCode { get; set; }
        [Required(ErrorMessage = "地址省不能为空")]
        public string ShippingProvince { get; set; }
        public int ShippingProvinceId { get; set; }
        [Required(ErrorMessage ="地址城市不能为空")]
        public string ShippingCity { get; set; }
        public int? ShippingCityId { get; set; }

       
    }
}
