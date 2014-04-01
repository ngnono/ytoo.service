using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Yintai.Hangzhou.Data.Models.Mapping
{
    public partial class OrderLogEntityMap : EntityTypeConfiguration<OrderLogEntity>
    {
        public OrderLogEntityMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.OrderNo)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.Operation)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("OrderLog");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.OrderNo).HasColumnName("OrderNo");
            this.Property(t => t.CustomerId).HasColumnName("CustomerId");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateUser).HasColumnName("CreateUser");
            this.Property(t => t.Operation).HasColumnName("Operation");
            this.Property(t => t.Type).HasColumnName("Type");
		Init();
        }

		partial void Init();
    }
}
