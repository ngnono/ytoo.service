using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Intime.OPC.Domain.Models.Mapping
{
    public class OPC_OrderCommentMap : EntityTypeConfiguration<OPC_OrderComment>
    {
        public OPC_OrderCommentMap()
        {
            // Primary Key
            HasKey(t => new {t.Id, t.OrderNo, t.Content, t.CreateDate, t.CreateUser, t.UpdateDate, t.UpdateUser});

            // Properties
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.OrderNo)
                .IsRequired()
                .HasMaxLength(20);

            Property(t => t.Content)
                .IsRequired()
                .HasMaxLength(500);

            Property(t => t.CreateUser)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.UpdateUser)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("OPC_OrderComment");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.OrderNo).HasColumnName("OrderNo");
            Property(t => t.Content).HasColumnName("Content");
            Property(t => t.CreateDate).HasColumnName("CreateDate");
            Property(t => t.CreateUser).HasColumnName("CreateUser");
            Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            Property(t => t.UpdateUser).HasColumnName("UpdateUser");
        }
    }
}