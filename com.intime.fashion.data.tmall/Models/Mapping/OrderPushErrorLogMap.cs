using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace com.intime.fashion.data.tmall.Models.Mapping
{
    public class OrderPushErrorLogMap : EntityTypeConfiguration<OrderPushErrorLog>
    {
        public OrderPushErrorLogMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Reason)
                .IsRequired()
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("OrderPushErrorLog");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.TmallOrderId).HasColumnName("TmallOrderId");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.Reason).HasColumnName("Reason");
            this.Property(t => t.Type).HasColumnName("Type");
            this.Property(t => t.FailedCount).HasColumnName("FailedCount");
        }
    }
}
