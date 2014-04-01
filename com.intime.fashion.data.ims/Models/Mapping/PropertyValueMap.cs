using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Yintai.Hangzhou.Data.Models.Mapping
{
    public partial class PropertyValueEntityMap : EntityTypeConfiguration<PropertyValueEntity>
    {
        public PropertyValueEntityMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Value)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("PropertyValue");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.PropertyId).HasColumnName("PropertyId");
            this.Property(t => t.ValueId).HasColumnName("ValueId");
            this.Property(t => t.Value).HasColumnName("Value");
            this.Property(t => t.Order).HasColumnName("Order");
            this.Property(t => t.HasChild).HasColumnName("HasChild");
		Init();
        }

		partial void Init();
    }
}
