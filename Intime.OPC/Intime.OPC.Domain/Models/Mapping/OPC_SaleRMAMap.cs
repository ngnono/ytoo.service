using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Intime.OPC.Domain.Models.Mapping
{
    public class OPC_SaleRMAMap : EntityTypeConfiguration<OPC_SaleRMA>
    {
        public OPC_SaleRMAMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Reason)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("OPC_SaleRMA");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.SaleId).HasColumnName("SaleId");
            this.Property(t => t.Reason).HasColumnName("Reason");
            this.Property(t => t.BackDate).HasColumnName("BackDate");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.CreatedUser).HasColumnName("CreatedUser");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.UpdatedUser).HasColumnName("UpdatedUser");
        }
    }
}