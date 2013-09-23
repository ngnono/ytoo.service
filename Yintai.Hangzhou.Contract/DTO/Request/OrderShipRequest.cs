using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yintai.Hangzhou.Contract.DTO.Request
{
   public class OrderShipRequest:BaseRequest
    {
       [Required(ErrorMessage="OrderNo 不能为空")]
       public string OrderNo { get; set; }
       [Required(ErrorMessage = "ShipVia 不能为空")]
       public string ShipVia { get; set; }
       [Required(ErrorMessage = "ShipNo 不能为空")]
       public string ShipNo { get; set; }
       [Required(ErrorMessage = "Products 不能为空")]
       public IEnumerable<OrderProductDetailRequest> Products { get; set; }
        [Required(ErrorMessage = "UpdateTime 不能为空")]
       public DateTime UpdateTime { get; set; }
        public int StoreId { get; set; }

    }
}
