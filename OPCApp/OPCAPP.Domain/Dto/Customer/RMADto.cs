using System;

namespace Intime.OPC.Domain.Dto
{
    public class RMADto
    {
        public int Id { get; set; }
        public string SaleOrderNo { get; set; }
        /// <summary>
        /// 退货单号
        /// </summary>
        /// <value>The rma no.</value>
        public string RMANo { get; set; }
        public string SourceDesc { get; set; }
        /// <summary>
        /// 退货总数
        /// </summary>
        /// <value>The count.</value>
        public int? Count { get; set; }
       
        public decimal RefundAmount { get; set; }

        public string OrderNo { get; set; }
        /// <summary>
        /// 退货类型
        /// </summary>
        /// <value>The type of the rma.</value>
        public int RMAType { get; set; }
        /// <summary>
        /// 退货单状态ID
        /// </summary>
        /// <value>The status.</value>
        public int Status { get; set; }
        /// <summary>
        /// 退货单状态
        /// </summary>
        /// <value>The name of the status.</value>
        public string StatusName { get; set; }
        /// <summary>
        /// 退货总金额
        /// </summary>
        /// <value>The refund amount.</value>
        public decimal RMAAmount { get; set; }
       
        public int? UserId { get; set; }
        /// <summary>
        /// 退货原因
        /// </summary>
        /// <value>The rma reason.</value>
        public string  RMAReason { get; set; }
        public string ContactPerson { get; set; }
        /// <summary>
        /// 退货时间
        /// </summary>
        /// <value>The created date.</value>
        public DateTime CreatedDate { get; set; }

        public int StoreId { get; set; }
        public string StoreName { get; set; }

        /// <summary>
        /// 退货收银状态
        /// </summary>
        /// <value>The name of the rma cash status.</value>
        public string RmaCashStatusName { get; set; }

        /// <summary>
        /// 退货状态
        /// </summary>
        /// <value>The name of the rma cash status.</value>
        public string RmaStatusName { get; set; }

        /// <summary>
        /// 专柜码
        /// </summary>
        /// <value>The name of the rma cash status.</value>
        public string 专柜码 { get; set; }

        /// <summary>
        /// 收银流水号
        /// </summary>
        /// <value>The cash number.</value>
        public string CashNum { get; set; }

        /// <summary>
        /// 收银时间
        /// </summary>
        /// <value>The cash number.</value>
        public DateTime? CashDate { get; set; }

        /// <summary>
        /// 退货收银流水号
        /// </summary>
        /// <value>The cash number.</value>
        public string RmaCashNum { get; set; }

        /// <summary>
        /// 退货收银时间
        /// </summary>
        /// <value>The cash number.</value>
        public DateTime? RmaCashDate { get; set; }
    }
}