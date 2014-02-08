using Yintai.Hangzhou.Model.Enums;

namespace Yintai.Hangzhou.Data.Models
{
    public class Map4Order : Map4EntityBase
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 渠道订单号
        /// </summary>
        public string ChannelOrderCode { get; set; }

        public OrderOpera SyncStatus { get; set; }
    }
}