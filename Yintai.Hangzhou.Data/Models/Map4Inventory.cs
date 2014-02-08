
namespace Yintai.Hangzhou.Data.Models
{
    public class Map4Inventory:Map4EntityBase
    {
        public int ProductId { get; set; }

        public long InventoryId { get; set; }

        public string itemId { get; set; }
        public string attr { get; set; }

        public string desc { get; set; }

        public string stockId { get; set; }

        public int sellerUin { get; set; }

        public int soldNum { get; set; }

        public int num { get; set; }

        public long skuId { get; set; }

        public int status { get; set; }

        public string saleAttr { get; set; }

        public string pic { get; set; }

        public string specAttr { get; set; }

        public decimal price { get; set; }
    }
}
