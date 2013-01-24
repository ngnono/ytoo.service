using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Yintai.Hangzhou.Data.Models.Mapping
{
    public partial class ProductEntityMap : EntityTypeConfiguration<ProductEntity>
    {
        public ProductEntityMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(64);

            this.Property(t => t.Description)
                .IsRequired();

            this.Property(t => t.RecommendedReason)
                .IsRequired();

            this.Property(t => t.Favorable)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("Product");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Brand_Id).HasColumnName("Brand_Id");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.CreatedUser).HasColumnName("CreatedUser");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.UpdatedUser).HasColumnName("UpdatedUser");
            this.Property(t => t.Price).HasColumnName("Price");
            this.Property(t => t.RecommendedReason).HasColumnName("RecommendedReason");
            this.Property(t => t.Favorable).HasColumnName("Favorable");
            this.Property(t => t.RecommendUser).HasColumnName("RecommendUser");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.Store_Id).HasColumnName("Store_Id");
            this.Property(t => t.Tag_Id).HasColumnName("Tag_Id");
            this.Property(t => t.FavoriteCount).HasColumnName("FavoriteCount");
            this.Property(t => t.ShareCount).HasColumnName("ShareCount");
            this.Property(t => t.InvolvedCount).HasColumnName("InvolvedCount");
            this.Property(t => t.RecommendSourceId).HasColumnName("RecommendSourceId");
            this.Property(t => t.RecommendSourceType).HasColumnName("RecommendSourceType");
            this.Property(t => t.SortOrder).HasColumnName("SortOrder");
        }
    }
}
