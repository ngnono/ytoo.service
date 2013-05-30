using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Yintai.Hangzhou.Data.Models.Mapping
{
    public partial class RMAEntityMap : EntityTypeConfiguration<RMAEntity>
    {
        public RMAEntityMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.RMANo)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.OrderNo)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.Reason)
                .HasMaxLength(500);

            this.Property(t => t.BankName)
                .HasMaxLength(200);

            this.Property(t => t.BankAccount)
                .HasMaxLength(20);

            this.Property(t => t.BankCard)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("RMA");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.RMANo).HasColumnName("RMANo");
            this.Property(t => t.OrderNo).HasColumnName("OrderNo");
            this.Property(t => t.RMAType).HasColumnName("RMAType");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.Reason).HasColumnName("Reason");
            this.Property(t => t.RMAAmount).HasColumnName("RMAAmount");
            this.Property(t => t.CreateUser).HasColumnName("CreateUser");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.UpdateUser).HasColumnName("UpdateUser");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.BankName).HasColumnName("BankName");
            this.Property(t => t.BankAccount).HasColumnName("BankAccount");
            this.Property(t => t.BankCard).HasColumnName("BankCard");
		Init();
        }

		partial void Init();
    }
}
