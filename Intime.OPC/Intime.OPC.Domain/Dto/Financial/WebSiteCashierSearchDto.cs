using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Intime.OPC.Domain.Dto.Financial
{
    public class WebSiteCashierSearchDto : WebSiteBaseDto
    {
        /// <summary>
        /// 数量
        /// </summary>
        /// <value>The count.</value>
        public int Count { get; set; }
        /// <summary>
        ///     退货单号
        /// </summary>
        /// <value>The rma no.</value>
        public string RMANo { get; set; }

        /// <summary>
        ///     收银流水号
        /// </summary>
        /// <value>The cash number.</value>
        public string CashNum { get; set; }

        /// <summary>
        ///     退货收银流水号
        /// </summary>
        /// <value>The cash number.</value>
        public string RmaCashNum { get; set; }
    }

    public class CashierList : List<WebSiteCashierSearchDto>
    {
        /// <summary>
        /// 销售总数量
        /// </summary>
        /// <value>The total sale count.</value>
        public int TotalSaleCount { get; set; }

        /// <summary>
        /// 销售总金额
        /// </summary>
        /// <value>The total sale total price.</value>
        public decimal TotalSaleTotalPrice { get; set; }


        public void Stat()
        {
            TotalSaleCount = this.Sum(t => t.Count);
            TotalSaleTotalPrice = this.Sum(t => t.SalePrice);

            var dto = new WebSiteCashierSearchDto();
            dto.Count = TotalSaleCount;
            dto.SalePrice = TotalSaleTotalPrice;
            this.Add(dto);
        }
    }
}