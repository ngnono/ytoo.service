using System.Data.Entity.ModelConfiguration;

namespace Intime.OPC.Domain.Models.Mapping
{
    public class OPC_ShippingSaleCommentMap : EntityTypeConfiguration<OPC_ShippingSaleComment>
    {
        public OPC_ShippingSaleCommentMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.ShippingCode)
                .IsRequired()
                .HasMaxLength(50);

            Property(t => t.Content)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            ToTable("OPC_ShippingSaleComment");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.ShippingCode).HasColumnName("ShippingCode");
            Property(t => t.Content).HasColumnName("Content");
            Property(t => t.CreateDate).HasColumnName("CreateDate");
            Property(t => t.CreateUser).HasColumnName("CreateUser");
            Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            Property(t => t.UpdateUser).HasColumnName("UpdateUser");
        }
    }
}