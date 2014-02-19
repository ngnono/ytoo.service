
using System;

namespace Yintai.Hangzhou.Data.Models
{
    public class Map4Product:Map4EntityBase
    {
        public string ChannelProductId { get; set; }

        public int ProductId { get; set; }

        public Nullable<int> IsImageUpload { get; set; }

        /// <summary>
        /// 商品在渠道的上下架状态
        /// </summary>
        public int Status { get; set; }
    }

    public class MappedProductBackup : Map4EntityBase
    {
        public string ChannelProductId { get; set; }

        public string ProductId { get; set; }
    }
}
