using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace Yintai.Hangzhou.Data.Models.Mapping
{
    public partial class OPC_StockPropertyValueEntityMap : EntityTypeConfiguration<OPC_StockPropertyValueEntity>
    {
        public OPC_StockPropertyValueEntityMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("OPC_StockPropertyValue");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.InventoryId).HasColumnName("InventoryId");
            this.Property(t => t.ChannelValueId).HasColumnName("ChannelValueId");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.ValueDesc).HasColumnName("ValueDesc");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.StockPropertyId).HasColumnName("StockPropertyId");
		Init();
        }

		partial void Init();
    }
}
