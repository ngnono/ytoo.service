using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Yintai.Hangzhou.Data.Models.Mapping
{
    public partial class PMessageEntityMap : EntityTypeConfiguration<PMessageEntity>
    {
        public PMessageEntityMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.TextMsg)
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("PMessage");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.IsVoice).HasColumnName("IsVoice");
            this.Property(t => t.TextMsg).HasColumnName("TextMsg");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.FromUser).HasColumnName("FromUser");
            this.Property(t => t.ToUser).HasColumnName("ToUser");
            this.Property(t => t.IsAuto).HasColumnName("IsAuto");
		Init();
        }

		partial void Init();
    }
}
