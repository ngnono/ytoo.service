using System.Data.Entity.ModelConfiguration;

namespace Yintai.Hangzhou.Data.Models.Mapping
{
    public class Map4ChannelMap<T> : EntityTypeConfiguration<T> where T : Map4EntityBase
    {
        public Map4ChannelMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Channel)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Channel).HasColumnName("Channel");
            this.Property(t => t.ChannelId).HasColumnName("ChannelId");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
        }
    }

    public class Map4BrandMap : Map4ChannelMap<Map4Brand>
    {
        public Map4BrandMap()
        {
            this.ToTable("Map4Brand");
            this.Property(t => t.BrandId).HasColumnName("BrandId");
            this.Property(t => t.ChannelBrandId).HasColumnName("ChannelBrandId");
        }
    }

    public class Map4ProductMap : Map4ChannelMap<Map4Product>
    {
        public Map4ProductMap()
        {
            this.ToTable("Map4Product");
            this.Property(t => t.ProductId).HasColumnName("ProductId");
            this.Property(t => t.ChannelProductId).HasColumnName("ChannelProductId");
            this.Property(t => t.Status).HasColumnName("Status");
        }
    }

    public class MappedProductBackupMap : Map4ChannelMap<MappedProductBackup>
    {
        public MappedProductBackupMap()
        {
            this.ToTable("MappedProductBackup");
            this.Property(t => t.ProductId).HasColumnName("ProductId");
            this.Property(t => t.ChannelProductId).HasColumnName("ChannelProductId");
        }
    }

    public class Map4CategoryMap : Map4ChannelMap<Map4Category>
    {
        public Map4CategoryMap()
        {
            this.ToTable("Map4Category");
            this.Property(t => t.CategoryCode).HasColumnName("CategoryCode");
            this.Property(t => t.ChannelCategoryId).HasColumnName("ChannelCategoryId");
        }
    }

    public class Map4InventoryMap : Map4ChannelMap<Map4Inventory>
    {
        public Map4InventoryMap()
        {
            this.ToTable("Map4Inventory");
            this.Property(t => t.attr).HasColumnName("attr");
            this.Property(t => t.desc).HasColumnName("desc");
            this.Property(t => t.stockId).HasColumnName("stockId");
            this.Property(t => t.sellerUin).HasColumnName("sellerUin");
            this.Property(t => t.soldNum).HasColumnName("soldNum");
            this.Property(t => t.skuId).HasColumnName("skuId");
            this.Property(t => t.status).HasColumnName("status");
            this.Property(t => t.saleAttr).HasColumnName("saleAttr");
            this.Property(t => t.pic).HasColumnName("pic");
            this.Property(t => t.specAttr).HasColumnName("specAttr");
            this.Property(t => t.ProductId).HasColumnName("ProductId");
            this.Property(t => t.InventoryId).HasColumnName("InventoryId");
            this.Property(t => t.itemId).HasColumnName("itemId");
            this.Property(t => t.price).HasColumnName("price");
        }
    }


    public class Map4OrderMap : Map4ChannelMap<Map4Order>
    {
        public Map4OrderMap()
        {
            this.ToTable("Map4Order");
            this.Property(t => t.OrderNo).HasColumnName("OrderNo");
            this.Property(t => t.ChannelOrderCode).HasColumnName("ChannelOrderCode");
            this.Property(t => t.SyncStatus).HasColumnName("SyncStatus");
        }
    }
}
