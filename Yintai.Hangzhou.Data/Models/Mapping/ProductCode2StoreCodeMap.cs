using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Yintai.Hangzhou.Data.Models.Mapping
{
    public partial class ProductCode2StoreCodeEntityMap : EntityTypeConfiguration<ProductCode2StoreCodeEntity>
    {
        public ProductCode2StoreCodeEntityMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("ProductCode2StoreCode");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.ProductId).HasColumnName("ProductId");
            this.Property(t => t.ExPId).HasColumnName("ExPId");
            this.Property(t => t.StoreId).HasColumnName("StoreId");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.UpdateUser).HasColumnName("UpdateUser");
		Init();
        }

		partial void Init();
    }
}
