using System.Data.Entity.ModelConfiguration;

namespace Yintai.Hangzhou.Data.Models.Mapping
{
    public class IMS_GiftCardOrderMap : EntityTypeConfiguration<IMS_GiftCardOrder>
    {
        public IMS_GiftCardOrderMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.No)
                .IsRequired()
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("IMS_GiftCardOrder");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.No).HasColumnName("No");
            this.Property(t => t.GiftCardId).HasColumnName("GiftCardId");
            this.Property(t => t.Amount).HasColumnName("Amount");
            this.Property(t => t.PurchaseUserId).HasColumnName("PurchaseUserId");
            this.Property(t => t.OwnerUserId).HasColumnName("OwnerUserId");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateUser).HasColumnName("CreateUser");
        }
    }
}
