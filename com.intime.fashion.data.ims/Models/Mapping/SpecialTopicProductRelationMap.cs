using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Yintai.Hangzhou.Data.Models.Mapping
{
    public partial class SpecialTopicProductRelationEntityMap : EntityTypeConfiguration<SpecialTopicProductRelationEntity>
    {
        public SpecialTopicProductRelationEntityMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("SpecialTopicProductRelations");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.SpecialTopic_Id).HasColumnName("SpecialTopic_Id");
            this.Property(t => t.Product_Id).HasColumnName("Product_Id");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.CreatedUser).HasColumnName("CreatedUser");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.UpdatedUser).HasColumnName("UpdatedUser");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
		Init();
        }

		partial void Init();
    }
}
