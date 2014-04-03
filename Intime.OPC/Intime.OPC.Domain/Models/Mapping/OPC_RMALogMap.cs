using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Intime.OPC.Domain.Models.Mapping
{
    public class OPC_RMALogMap : EntityTypeConfiguration<OPC_RMALog>
    {
        public OPC_RMALogMap()
        {
            // Primary Key
            HasKey(t => t.Id);
            Property(t => t.Id)
               .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            // Properties
            Property(t => t.Operation)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            ToTable("OPC_RMALog");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.OpcRmaId).HasColumnName("OpcRmaId");
            Property(t => t.CreateDate).HasColumnName("CreateDate");
            Property(t => t.CreateUser).HasColumnName("CreateUser");
            Property(t => t.Operation).HasColumnName("Operation");
        }
    }
}