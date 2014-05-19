using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intime.OPC.Domain.Enums;

namespace Intime.OPC.Domain.Dto.Request
{
    public class CreateShippingSaleOrderRequest
    {
        [Required]
        public List<int> SaleOrderNos { get; set; }
    }

    public class GetShippingSaleOrderRequest : DateRangeRequest
    {
        /// <summary>
        /// 订单 no
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 销售单 no
        /// </summary>
        public string SaleOrderNo { get; set; }
    }

    public class PutShippingSaleOrderRequest : DateRangeRequest
    {
        /// <summary>
        /// 发货单号
        /// </summary>
        public int ShippingSaleOrderId { get; set; }

        /// <summary>
        /// 快递公司
        /// </summary>
        public int  ShipViaId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public int ShippingRemark { get; set; }

        /// <summary>
        /// 快递单号
        /// </summary>
        public string ShippingNo { get; set; }

        /// <summary>
        /// 快递费
        /// </summary>
        public decimal ShippingFee { get; set; }
    }
}
