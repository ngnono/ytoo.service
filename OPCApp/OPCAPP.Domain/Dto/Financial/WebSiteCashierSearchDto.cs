namespace OPCAPP.Domain.Dto.Financial
{
    public class WebSiteCashierSearchDto : WebSiteBaseDto
    {
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
}