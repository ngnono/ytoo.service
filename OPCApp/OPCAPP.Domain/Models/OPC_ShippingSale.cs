using System;

namespace OPCApp.Domain.Models
{
    public class OPC_ShippingSale
    {
        public bool IsSelected { get; set; }

        /// <summary>
        ///     发货单ID
        /// </summary>
        public int Id { get; set; }

        public string SaleOrderNo { get; set; }

        /// 发货单号
        /// </summary>
        public string GoodsOutCode { get; set; }

        /// <summary>
        ///     快递单号
        /// </summary>
        public string ExpressCode { get; set; }

        /// <summary>
        ///     发货状态
        /// </summary>
        public string ShippingStatus { get; set; }

        /// <summary>
        ///     收货人姓名
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        ///     收货人地址
        /// </summary>
        public string CustomerAddress { get; set; }

        /// <summary>
        ///     收货人电话
        /// </summary>
        public string CustomerPhone { get; set; }

        /// <summary>
        ///     发货时间
        /// </summary>
        public DateTime GoodsOutDate { get; set; }

        /// <summary>
        ///     发货方式
        /// </summary>
        public string GoodsOutType { get; set; }

        /// <summary>
        ///     快递公司
        /// </summary>
        public string ShipCompanyName { get; set; }

        /// <summary>
        ///     快递员
        /// </summary>
        public string ShipManName { get; set; }

        /// <summary>
        ///     打印状态
        /// </summary>
        public string PrintStatus { get; set; }

        /// <summary>
        ///     邮编
        /// </summary>
        public string ShippingZipCode { get; set; }

        /// <summary>
        ///     配送方式
        /// </summary>
        /// <value>The shipping method.</value>
        public string ShippingMethod { get; set; }

        /// <summary>
        ///     支付快递公司快递费
        /// </summary>
        /// <value>The express fee.</value>
        public double ShipViaExpressFee { get; set; }

        /// <summary>
        ///     快递费
        /// </summary>
        /// <value>The express fee.</value>
        public double ExpressFee { get; set; }
    }
}