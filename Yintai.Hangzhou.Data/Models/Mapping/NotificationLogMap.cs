using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Yintai.Hangzhou.Data.Models.Mapping
{
    public partial class NotificationLogEntityMap : EntityTypeConfiguration<NotificationLogEntity>
    {
        public NotificationLogEntityMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.DeviceToken)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Message)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("NotificationLog");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.DeviceToken).HasColumnName("DeviceToken");
            this.Property(t => t.NotifyDate).HasColumnName("NotifyDate");
            this.Property(t => t.Message).HasColumnName("Message");
            this.Property(t => t.InDate).HasColumnName("InDate");
            this.Property(t => t.Status).HasColumnName("Status");
        }
    }
}
