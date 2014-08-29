using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Yintai.Hangzhou.Data.Models.Mapping
{
    public partial class ShipViaEntityMap : EntityTypeConfiguration<ShipViaEntity>
    {
        public ShipViaEntityMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Url)
                .HasMaxLength(200);

            this.Property(t => t.TemplateName)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("ShipVia");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Url).HasColumnName("Url");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.IsOnline).HasColumnName("IsOnline");
            this.Property(t => t.TemplateName).HasColumnName("TemplateName");
		Init();
        }

		partial void Init();
    }
}
