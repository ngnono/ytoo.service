using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Yintai.Hangzhou.Data.Models.Mapping
{
    public partial class RMAItemEntityMap : EntityTypeConfiguration<RMAItemEntity>
    {
        public RMAItemEntityMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.RMANo)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.ProductDesc)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.StoreItem)
                .HasMaxLength(50);

            this.Property(t => t.StoreDesc)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("RMAItem");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.RMANo).HasColumnName("RMANo");
            this.Property(t => t.ProductId).HasColumnName("ProductId");
            this.Property(t => t.ProductDesc).HasColumnName("ProductDesc");
            this.Property(t => t.StoreItem).HasColumnName("StoreItem");
            this.Property(t => t.StoreDesc).HasColumnName("StoreDesc");
            this.Property(t => t.ItemPrice).HasColumnName("ItemPrice");
            this.Property(t => t.Quantity).HasColumnName("Quantity");
            this.Property(t => t.ExtendPrice).HasColumnName("ExtendPrice");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.Status).HasColumnName("Status");
		Init();
        }

		partial void Init();
    }
}
