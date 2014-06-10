
namespace Yintai.Hangzhou.Model.ESModel
{
    public class ESStockPropertyValue
    {
        public int Id { get; set; }

        public int InventoryId { get; set; }

        public int ChannelPropertyId { get; set; }

        public int PropertyId { get; set; }

        public string PropertyDesc { get; set; }

        public string ValueDesc { get; set; }

        public int ChannelValueId { get; set; }

        public int ValueId { get; set; }
    }
}
