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
            this.Property(t => t.OrderNo)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.Phone)
                .IsRequired()
                .HasMaxLength(11);

            this.Property(t => t.Comment)
                .HasMaxLength(50);

            this.Property(t => t.FromNickName)
                .HasMaxLength(50);

            this.Property(t => t.FromPhone)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("IMS_GiftCardTransfers");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.OrderNo).HasColumnName("OrderNo");
            this.Property(t => t.FromUserId).HasColumnName("FromUserId");
            this.Property(t => t.ToUserId).HasColumnName("ToUserId");
            this.Property(t => t.Phone).HasColumnName("Phone");
            this.Property(t => t.Comment).HasColumnName("Comment");
            this.Property(t => t.IsActive).HasColumnName("IsActive");
            this.Property(t => t.IsDecline).HasColumnName("IsDecline");
            this.Property(t => t.PreTransferId).HasColumnName("PreTransferId");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateUser).HasColumnName("CreateUser");
            this.Property(t => t.OperateDate).HasColumnName("OperateDate");
            this.Property(t => t.OperateUser).HasColumnName("OperateUser");
            this.Property(t => t.FromNickName).HasColumnName("FromNickName");
            this.Property(t => t.FromPhone).HasColumnName("FromPhone");
		Init();
        }

		partial void Init();
    }
}
