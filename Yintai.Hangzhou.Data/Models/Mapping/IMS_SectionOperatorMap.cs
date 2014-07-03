using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Yintai.Hangzhou.Data.Models.Mapping
{
    public partial class IMS_SectionOperatorEntityMap : EntityTypeConfiguration<IMS_SectionOperatorEntity>
    {
        public IMS_SectionOperatorEntityMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.OperatorCode)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("IMS_SectionOperator");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.SectionId).HasColumnName("SectionId");
            this.Property(t => t.OperatorCode).HasColumnName("OperatorCode");
		Init();
        }

		partial void Init();
    }
}
