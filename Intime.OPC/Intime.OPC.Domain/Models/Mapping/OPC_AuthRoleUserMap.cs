using System.Data.Entity.ModelConfiguration;

namespace Intime.OPC.Domain.Models.Mapping
{
    public class OPC_AuthRoleUserMap : EntityTypeConfiguration<OPC_AuthRoleUser>
    {
        public OPC_AuthRoleUserMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            ToTable("OPC_AuthRoleUser");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.OPC_AuthUserId).HasColumnName("OPC_AuthUserId");
            Property(t => t.OPC_AuthRoleId).HasColumnName("OPC_AuthRoleId");
            Property(t => t.CreateDate).HasColumnName("CreateDate");
            Property(t => t.CreateUserId).HasColumnName("CreateUserId");
            Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            Property(t => t.UpdateUserId).HasColumnName("UpdateUserId");
        }
    }
}