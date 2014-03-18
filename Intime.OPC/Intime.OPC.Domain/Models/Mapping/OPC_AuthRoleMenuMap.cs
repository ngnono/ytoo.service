using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Intime.OPC.Domain.Models.Mapping
{
    public class OPC_AuthRoleMenuMap : EntityTypeConfiguration<OPC_AuthRoleMenu>
    {
        public OPC_AuthRoleMenuMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("OPC_AuthRoleMenu");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.OPC_AuthMenuId).HasColumnName("OPC_AuthMenuId");
            Property(t => t.OPC_AuthRoleId).HasColumnName("OPC_AuthRoleId");
            Property(t => t.CreateDate).HasColumnName("CreateDate");
            Property(t => t.CreateUserId).HasColumnName("CreateUserId");
            Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            Property(t => t.UpdateUserId).HasColumnName("UpdateUserId");
        }
    }
}