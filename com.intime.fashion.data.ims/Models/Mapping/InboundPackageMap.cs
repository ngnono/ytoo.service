using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Yintai.Hangzhou.Data.Models.Mapping
{
    public partial class InboundPackageEntityMap : EntityTypeConfiguration<InboundPackageEntity>
    {
        public InboundPackageEntityMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.SourceNo)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.ShippingNo)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("InboundPackage");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.SourceNo).HasColumnName("SourceNo");
            this.Property(t => t.SourceType).HasColumnName("SourceType");
            this.Property(t => t.ShippingVia).HasColumnName("ShippingVia");
            this.Property(t => t.ShippingNo).HasColumnName("ShippingNo");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateUser).HasColumnName("CreateUser");
		Init();
        }

		partial void Init();
    }
}
