using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Intime.OPC.Domain.Models.Mapping
{
    public class OPC_StockMap : EntityTypeConfiguration<OPC_Stock>
    {
        public OPC_StockMap()
        {
            // Primary Key
            HasKey(t => t.Id);
            Property(t => t.Id)
               .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // Properties
            // Table & Column Mappings
            ToTable("OPC_Stock");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.SkuId).HasColumnName("SkuId");
            Property(t => t.SourceStockId).HasColumnName("SourceStockId");
            Property(t => t.SectionId).HasColumnName("SectionId");
            Property(t => t.Count).HasColumnName("Count");
            Property(t => t.Price).HasColumnName("Price");
            Property(t => t.Status).HasColumnName("Status");
            Property(t => t.IsDel).HasColumnName("IsDel");
            Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            Property(t => t.CreatedUser).HasColumnName("CreatedUser");
            Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            Property(t => t.UpdatedUser).HasColumnName("UpdatedUser");
        }
    }
}