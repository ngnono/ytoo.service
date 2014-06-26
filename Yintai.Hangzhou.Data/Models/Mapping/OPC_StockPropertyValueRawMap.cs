using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace Yintai.Hangzhou.Data.Models.Mapping
{
    public partial class OPC_StockPropertyValueRawEntityMap : EntityTypeConfiguration<OPC_StockPropertyValueRawEntity>
    {
        public OPC_StockPropertyValueRawEntityMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("OPC_StockPropertyValueRaw");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.InventoryId).HasColumnName("InventoryId");
            this.Property(t => t.SourceStockId).HasColumnName("SourceStockId");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.BrandSizeCode).HasColumnName("BrandSizeCode");
            this.Property(t => t.BrandSizeName).HasColumnName("BrandSizeName");
            this.Property(t => t.PropertyData).HasColumnName("PropertyData");
            this.Property(t => t.Channel).HasColumnName("Channel");
		Init();
        }

		partial void Init();
    }
}
