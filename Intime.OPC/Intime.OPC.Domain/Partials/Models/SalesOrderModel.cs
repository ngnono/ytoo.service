using System;

namespace Intime.OPC.Domain.Models
{
    public class SalesOrderModel
    {

        /// <summary>
        /// Id 主键ID
        /// </summary>

        public int Id { get; set; }

        /// <summary>
        /// 订单 NO
        /// </summary>

        public string OrderNo { get; set; }

        /// <summary>
        /// 销售单 NO
        /// </summary>

        public string SaleOrderNo { get; set; }


        public int SalesType { get; set; }


        public int? ShipViaId { get; set; }


        public int Status { get; set; }

        /// <summary>
        /// 物流单 code
        /// </summary>

        public string ShippingCode { get; set; }

        /// <summary>
        /// 物流 费
        /// </summary>

        public decimal ShippingFee { get; set; }

        /// <summary>
        /// 物流 状态
        /// </summary>

        public int? ShippingStatus { get; set; }

        /// <summary>
        /// 物流状态 Name
        /// </summary>

        //public string ShippingStatusName { get; set; }

        /// <summary>
        /// 物流备注
        /// </summary>

        public string ShippingRemark { get; set; }


        public DateTime SellDate { get; set; }


        public bool? IfTrans { get; set; }


        public int? TransStatus { get; set; }


        public decimal SalesAmount { get; set; }


        public int? SalesCount { get; set; }


        public int? CashStatus { get; set; }


        public string CashNum { get; set; }


        public DateTime? CashDate { get; set; }


        public int? SectionId { get; set; }


        public int? PrintTimes { get; set; }


        public string Remark { get; set; }


        public DateTime? RemarkDate { get; set; }


        public DateTime CreatedDate { get; set; }


        public int? CreatedUser { get; set; }


        public DateTime? UpdatedDate { get; set; }


        public int? UpdatedUser { get; set; }


       // public string StatusName { get; set; }


        public string CashStatusName { get; set; }


        public string StoreName { get; set; }


        public string InvoiceSubject { get; set; }


        public string PayType { get; set; }


        public string SectionName { get; set; }


        public bool? Invoice { get; set; }


        public string StoreTelephone { get; set; }


        public string StoreAddress { get; set; }


        public string TransNo { get; set; }


        public string OrderSource { get; set; }


        public string ReceivePerson { get; set; }


        public string CustomerName { get; set; }


        public string CustomerPhone { get; set; }


        public int StoreId { get; set; }


        public string SectionCode { get; set; }

        public int? ShippingSaleId { get; set; }
    }
}
