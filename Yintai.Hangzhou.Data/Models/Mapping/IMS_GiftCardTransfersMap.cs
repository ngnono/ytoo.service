using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Yintai.Hangzhou.Data.Models.Mapping
{
    public partial class IMS_GiftCardTransfersEntityMap : EntityTypeConfiguration<IMS_GiftCardTransfersEntity>
    {
        public IMS_GiftCardTransfersEntityMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Phone)
                .IsRequired()
                .HasMaxLength(11);

            this.Property(t => t.Comment)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("IMS_GiftCardTransfers");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.GiftCardId).HasColumnName("GiftCardId");
            this.Property(t => t.FromUserId).HasColumnName("FromUserId");
            this.Property(t => t.ToUserId).HasColumnName("ToUserId");
            this.Property(t => t.Phone).HasColumnName("Phone");
            this.Property(t => t.Comment).HasColumnName("Comment");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.IsDecline).HasColumnName("IsDecline");
            this.Property(t => t.PreTransferId).HasColumnName("PreTransferId");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateUser).HasColumnName("CreateUser");
		Init();
        }

		partial void Init();
    }
}
