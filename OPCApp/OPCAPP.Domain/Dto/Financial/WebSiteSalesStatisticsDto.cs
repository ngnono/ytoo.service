using System;

namespace OPCAPP.Domain.Dto.Financial
{
    //网络销售明细统计
    public class WebSiteSalesStatisticsDto : WebSiteBaseDto
    {
        public WebSiteSalesStatisticsDto()
        {
            BuyDate = DateTime.Now;
        }

        /// <summary>
        ///     销售金额
        /// </summary>
        /// <value>The sale Total price.</value>
        public double SaleTotalPrice { get; set; }

        /// <summary>
        ///     销售数量
        /// </summary>
        /// <value>The sale count.</value>
        public int SellCount { get; set; }

        /// <summary>
        ///     订单运费
        /// </summary>
        public decimal? OrderTransFee { get; set; }
    }
}