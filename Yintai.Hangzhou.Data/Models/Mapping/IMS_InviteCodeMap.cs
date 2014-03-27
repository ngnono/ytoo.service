using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Yintai.Hangzhou.Data.Models.Mapping
{
    public partial class IMS_InviteCodeEntityMap : EntityTypeConfiguration<IMS_InviteCodeEntity>
    {
        public IMS_InviteCodeEntityMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Code)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("IMS_InviteCode");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.SectionOperatorId).HasColumnName("SectionOperatorId");
            this.Property(t => t.AuthRight).HasColumnName("AuthRight");
            this.Property(t => t.IsBinded).HasColumnName("IsBinded");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.SourceType).HasColumnName("SourceType");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateUser).HasColumnName("CreateUser");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.UpdateUser).HasColumnName("UpdateUser");
            this.Property(t => t.Status).HasColumnName("Status");
		Init();
        }

		partial void Init();
    }
}
