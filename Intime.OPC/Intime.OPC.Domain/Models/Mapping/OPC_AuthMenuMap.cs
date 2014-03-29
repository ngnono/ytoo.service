using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Intime.OPC.Domain.Models.Mapping
{
    public class OPC_AuthMenuMap : EntityTypeConfiguration<OPC_AuthMenu>
    {
        public OPC_AuthMenuMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.MenuName)
                .HasMaxLength(40);

            Property(t => t.PraentMenuId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.Url)
                .HasMaxLength(40);

            // Table & Column Mappings
            ToTable("OPC_AuthMenu");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.MenuName).HasColumnName("MenuName");
            Property(t => t.CreateDate).HasColumnName("CreateDate");
            Property(t => t.CreateUserId).HasColumnName("CreateUserId");
            Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            Property(t => t.UpdateUserId).HasColumnName("UpdateUserId");
            Property(t => t.PraentMenuId).HasColumnName("PraentMenuId");
            Property(t => t.IsValid).HasColumnName("IsValid");
            Property(t => t.Sort).HasColumnName("Sort");
            Property(t => t.Url).HasColumnName("Url");
        }
    }
}