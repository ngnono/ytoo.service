using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Intime.OPC.Domain.Models.Mapping
{
    public class OPC_AuthUserMap : EntityTypeConfiguration<OPC_AuthUser>
    {
        public OPC_AuthUserMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(40);

            this.Property(t => t.LogonName)
                .HasMaxLength(40);

            this.Property(t => t.Password)
                .HasMaxLength(40);

            this.Property(t => t.Phone)
                .HasMaxLength(40);

            // Table & Column Mappings
            this.ToTable("OPC_AuthUser");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.SectionId).HasColumnName("SectionId");
            this.Property(t => t.LogonName).HasColumnName("LogonName");
            this.Property(t => t.Password).HasColumnName("Password");
            this.Property(t => t.Phone).HasColumnName("Phone");
            this.Property(t => t.IsValid).HasColumnName("IsValid");
            this.Property(t => t.OrgId).HasColumnName("OrgId");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateUserId).HasColumnName("CreateUserId");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.UpdateUserId).HasColumnName("UpdateUserId");
        }
    }
}
