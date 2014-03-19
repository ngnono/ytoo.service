using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Intime.OPC.Domain.Models.Mapping
{
    public class OPC_AuthUserMap : EntityTypeConfiguration<OPC_AuthUser>
    {
        public OPC_AuthUserMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(40);

            Property(t => t.LogonName)
                .HasMaxLength(40);

            Property(t => t.Password)
                .HasMaxLength(40);

            Property(t => t.Phone)
                .HasMaxLength(40);

            // Table & Column Mappings
            ToTable("OPC_AuthUser");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.SectionId).HasColumnName("SectionId");
            Property(t => t.LogonName).HasColumnName("LogonName");
            Property(t => t.Password).HasColumnName("Password");
            Property(t => t.Phone).HasColumnName("Phone");
            Property(t => t.IsValid).HasColumnName("IsValid");
            Property(t => t.OrgId).HasColumnName("OrgId");
            Property(t => t.CreateDate).HasColumnName("CreateDate");
            Property(t => t.CreateUserId).HasColumnName("CreateUserId");
            Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            Property(t => t.UpdateUserId).HasColumnName("UpdateUserId");
        }
    }
}