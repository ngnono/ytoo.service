using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Intime.OPC.Domain.Models.Mapping
{
    public class IMS_AssociateMapper : EntityTypeConfiguration<IMS_Associate>
    {
        public IMS_AssociateMapper()
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
            this.Property(t => t.StoreId).HasColumnName("StoreId");
        }
    }
}
