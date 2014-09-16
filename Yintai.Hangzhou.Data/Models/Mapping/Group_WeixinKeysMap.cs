using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Yintai.Hangzhou.Data.Models.Mapping
{
    public partial class Group_WeixinKeysEntityMap : EntityTypeConfiguration<Group_WeixinKeysEntity>
    {
        public Group_WeixinKeysEntityMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.AppId)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.AppSecret)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.PaySignKey)
                .IsRequired()
                .HasMaxLength(500);

            this.Property(t => t.ParterId)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.ParterKey)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.PaidLikeUrl)
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("Group_WeixinKeys");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.GroupId).HasColumnName("GroupId");
            this.Property(t => t.AppId).HasColumnName("AppId");
            this.Property(t => t.AppSecret).HasColumnName("AppSecret");
            this.Property(t => t.PaySignKey).HasColumnName("PaySignKey");
            this.Property(t => t.ParterId).HasColumnName("ParterId");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateUser).HasColumnName("CreateUser");
            this.Property(t => t.ParterKey).HasColumnName("ParterKey");
            this.Property(t => t.PaidLikeUrl).HasColumnName("PaidLikeUrl");
		Init();
        }

		partial void Init();
    }
}
