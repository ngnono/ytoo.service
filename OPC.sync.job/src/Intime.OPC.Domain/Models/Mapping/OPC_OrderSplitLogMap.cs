using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intime.OPC.Domain.Models.Mapping
{
    public class OPC_OrderSplitLogMapper : EntityTypeConfiguration<OPC_OrderSplitLog>
    {
        public OPC_OrderSplitLogMapper()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.OrderNo)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Reason)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("OPC_OrderSplitLog");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.OrderNo).HasColumnName("OrderNo");
            this.Property(t => t.Reason).HasColumnName("Reason");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");

        }

    }
}
