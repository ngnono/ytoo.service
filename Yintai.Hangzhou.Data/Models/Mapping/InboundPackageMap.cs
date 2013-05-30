using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Yintai.Hangzhou.Data.Models.Mapping
{
    public partial class InboundPackageEntityMap : EntityTypeConfiguration<InboundPackageEntity>
    {
        public InboundPackageEntityMap()
        {
            // Primary Key
            this.HasKey(t => new { t.Id, t.SourceNo, t.SourceType, t.ShippingVia, t.ShippingNo, t.CreateDate, t.CreateUser });

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(t => t.SourceNo)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.SourceType)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.ShippingVia)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.ShippingNo)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.CreateUser)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

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
