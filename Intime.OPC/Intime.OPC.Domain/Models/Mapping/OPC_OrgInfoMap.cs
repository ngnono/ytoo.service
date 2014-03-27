using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Intime.OPC.Domain.Models.Mapping
{
    public class OPC_OrgInfoMap : EntityTypeConfiguration<OPC_OrgInfo>
    {
        public OPC_OrgInfoMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.OrgName)
                .HasMaxLength(50);

            Property(t => t.ParentID)
                .HasMaxLength(50);

            Property(t => t.StoreName)
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("OPC_OrgInfo");
            Property(t => t.Id).HasColumnName("ID");
            Property(t => t.OrgName).HasColumnName("OrgName");
            Property(t => t.ParentID).HasColumnName("ParentID");
            Property(t => t.StoreID).HasColumnName("StoreID");
            Property(t => t.StoreName).HasColumnName("StoreName");
            Property(t => t.OrgType).HasColumnName("OrgType");
            Property(t => t.IsDel).HasColumnName("IsDel");
        }
    }
}