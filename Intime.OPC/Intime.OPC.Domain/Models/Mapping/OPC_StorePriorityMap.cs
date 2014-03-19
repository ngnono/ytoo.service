using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Intime.OPC.Domain.Models.Mapping
{
    public class OPC_StorePriorityMap : EntityTypeConfiguration<OPC_StorePriority>
    {
        public OPC_StorePriorityMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("OPC_StorePriority");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.StoreId).HasColumnName("StoreId");
            Property(t => t.Priority).HasColumnName("Priority");
            Property(t => t.CreateDate).HasColumnName("CreateDate");
            Property(t => t.CreateUser).HasColumnName("CreateUser");
            Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            Property(t => t.UpdateUser).HasColumnName("UpdateUser");
        }
    }
}