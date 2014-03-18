using System.Data.Entity.ModelConfiguration;

namespace Intime.OPC.Domain.Models.Mapping
{
    public class OPC_SaleRMAMap : EntityTypeConfiguration<OPC_SaleRMA>
    {
        public OPC_SaleRMAMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Reason)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            ToTable("OPC_SaleRMA");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.SaleId).HasColumnName("SaleId");
            Property(t => t.Reason).HasColumnName("Reason");
            Property(t => t.BackDate).HasColumnName("BackDate");
            Property(t => t.Status).HasColumnName("Status");
            Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            Property(t => t.CreatedUser).HasColumnName("CreatedUser");
            Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            Property(t => t.UpdatedUser).HasColumnName("UpdatedUser");
        }
    }
}