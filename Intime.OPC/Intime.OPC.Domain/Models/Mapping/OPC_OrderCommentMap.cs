using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Intime.OPC.Domain.Models.Mapping
{
    public class OPC_OrderCommentMap : EntityTypeConfiguration<OPC_OrderComment>
    {
        public OPC_OrderCommentMap()
        {
            // Primary Key
            this.HasKey(t => new { t.Id, t.OrderNo, t.Content, t.CreateDate, t.CreateUser, t.UpdateDate, t.UpdateUser });

            // Properties
            this.Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.OrderNo)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.Content)
                .IsRequired()
                .HasMaxLength(500);

            this.Property(t => t.CreateUser)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.UpdateUser)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("OPC_OrderComment");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.OrderNo).HasColumnName("OrderNo");
            this.Property(t => t.Content).HasColumnName("Content");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateUser).HasColumnName("CreateUser");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.UpdateUser).HasColumnName("UpdateUser");
        }
    }
}
