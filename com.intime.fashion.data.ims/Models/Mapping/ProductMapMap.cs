using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Yintai.Hangzhou.Data.Models.Mapping
{
    public partial class ProductMapEntityMap : EntityTypeConfiguration<ProductMapEntity>
    {
        public ProductMapEntityMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Channel)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("ProductMap");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.ProductId).HasColumnName("ProductId");
            this.Property(t => t.ChannelPId).HasColumnName("ChannelPId");
            this.Property(t => t.ChannelBrandId).HasColumnName("ChannelBrandId");
            this.Property(t => t.ChannelCatId).HasColumnName("ChannelCatId");
            this.Property(t => t.Channel).HasColumnName("Channel");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
		Init();
        }

		partial void Init();
    }
}
