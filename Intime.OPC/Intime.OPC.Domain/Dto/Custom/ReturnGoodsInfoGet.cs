using System;

namespace Intime.OPC.Domain.Dto.Custom
{
    /// <summary>
    /// 客户服务-客服退货查询-退货信息
    /// </summary>
    public class ReturnGoodsInfoGet
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }


        public string RmaNo { get; set; }

        public string OrderNo { get; set; }

        public string SaleOrderNo { get; set; }

        public int? RmaStatus { get; set; }


        public string PayType { get; set; }

        public int? StoreID { get; set; }
    }
}