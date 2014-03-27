using System.Data.Entity.ModelConfiguration;

namespace Yintai.Hangzhou.Data.Models.Mapping
{
    public class IMS_GiftCardRechargeMap : EntityTypeConfiguration<IMS_GiftCardRecharge>
    {
        public IMS_GiftCardRechargeMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.PurchaseId)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.ChargePhone)
                .IsRequired()
                .HasMaxLength(11);

            // Table & Column Mappings
            this.ToTable("IMS_GiftCardRecharge");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.ChargeUserId).HasColumnName("ChargeUserId");
            this.Property(t => t.PurchaseId).HasColumnName("PurchaseId");
            this.Property(t => t.ChargePhone).HasColumnName("ChargePhone");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateUser).HasColumnName("CreateUser");
        }
    }
}
