using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Intime.OPC.Domain.Models.Mapping
{
    public class OPC_SKUMap : EntityTypeConfiguration<OPC_SKU>
    {
        public OPC_SKUMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("OPC_SKU");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.ProductId).HasColumnName("ProductId");
            this.Property(t => t.ColorId).HasColumnName("ColorId");
            this.Property(t => t.SizeId).HasColumnName("SizeId");
        }
    }
}