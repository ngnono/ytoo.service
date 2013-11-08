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
       [Required(ErrorMessage = "DealNo 不能为空")]
       public string Sales_Sid { get; set; }
       public int ShipVia { get; set; }
       [Required(ErrorMessage = "ShipViaName 不能为空")]
       public string ShipViaName { get; set; }
       public string ShipNo { get; set; }
       public IEnumerable<OrderProductDetailRequest> Products { get; set; }
        [Required(ErrorMessage = "UpdateTime 不能为空")]
       public DateTime UpdateTime { get; set; }
       public int StoreId { get; set; }
       public bool ForceShip { get; set; }

    }
}
