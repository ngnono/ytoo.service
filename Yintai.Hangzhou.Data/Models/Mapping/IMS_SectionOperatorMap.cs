using System.Data.Entity.ModelConfiguration;

namespace Yintai.Hangzhou.Data.Models.Mapping
{
    public class IMS_SectionOperatorMap : EntityTypeConfiguration<IMS_SectionOperator>
    {
        public IMS_SectionOperatorMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Brands)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("IMS_SectionOperator");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Brands).HasColumnName("Brands");
        }
    }
}
