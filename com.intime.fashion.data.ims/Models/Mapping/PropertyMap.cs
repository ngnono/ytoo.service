using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Yintai.Hangzhou.Data.Models.Mapping
{
    public partial class PropertyEntityMap : EntityTypeConfiguration<PropertyEntity>
    {
        public PropertyEntityMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("Property");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.CategoryId).HasColumnName("CategoryId");
            this.Property(t => t.PropertyId).HasColumnName("PropertyId");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.OrderFlag).HasColumnName("OrderFlag");
            this.Property(t => t.IsSaleProperty).HasColumnName("IsSaleProperty");
            this.Property(t => t.IsKeyProperty).HasColumnName("IsKeyProperty");
            this.Property(t => t.IsNecessary).HasColumnName("IsNecessary");
		Init();
        }

		partial void Init();
    }
}
