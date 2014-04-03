using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Intime.OPC.Domain.Models.Mapping
{
    public class OPC_SKUMap : EntityTypeConfiguration<OPC_SKU>
    {
        public OPC_SKUMap()
        {
            // Primary Key
            HasKey(t => t.Id);
            Property(t => t.Id)
               .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            // Properties
            // Table & Column Mappings
            ToTable("OPC_SKU");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.ProductId).HasColumnName("ProductId");
            Property(t => t.ColorValueId).HasColumnName("ColorValueId");
            Property(t => t.SizeValueId).HasColumnName("SizeValueId");
        }
    }
}