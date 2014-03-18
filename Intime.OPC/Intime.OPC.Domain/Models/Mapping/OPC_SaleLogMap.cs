using System.Data.Entity.ModelConfiguration;

namespace Intime.OPC.Domain.Models.Mapping
{
    public class OPC_SaleLogMap : EntityTypeConfiguration<OPC_SaleLog>
    {
        public OPC_SaleLogMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.SaleId)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("OPC_SaleLog");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.SaleId).HasColumnName("SaleId");
            Property(t => t.Status).HasColumnName("Status");
            Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            Property(t => t.CreatedUser).HasColumnName("CreatedUser");
        }
    }
}