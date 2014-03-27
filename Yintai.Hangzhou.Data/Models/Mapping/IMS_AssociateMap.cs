using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Yintai.Hangzhou.Data.Models.Mapping
{
    public partial class IMS_AssociateEntityMap : EntityTypeConfiguration<IMS_AssociateEntity>
    {
        public IMS_AssociateEntityMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("IMS_Associate");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateUser).HasColumnName("CreateUser");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.TemplateId).HasColumnName("TemplateId");
            this.Property(t => t.OperateRight).HasColumnName("OperateRight");
		Init();
        }

		partial void Init();
    }
}
