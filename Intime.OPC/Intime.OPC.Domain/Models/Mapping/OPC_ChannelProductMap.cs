using System.Data.Entity.ModelConfiguration;

namespace Intime.OPC.Domain.Models.Mapping
{
    public class OPC_ChannelProductMap : EntityTypeConfiguration<OPC_ChannelProduct>
    {
        public OPC_ChannelProductMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            ToTable("OPC_ChannelProduct");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.ProductId).HasColumnName("ProductId");
            Property(t => t.ColorId).HasColumnName("ColorId");
            Property(t => t.ChannelId).HasColumnName("ChannelId");
            Property(t => t.Price).HasColumnName("Price");
            Property(t => t.Status).HasColumnName("Status");
            Property(t => t.UpDateTime).HasColumnName("UpDateTime");
            Property(t => t.DownDateTime).HasColumnName("DownDateTime");
            Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            Property(t => t.CreatedUser).HasColumnName("CreatedUser");
            Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            Property(t => t.UpdatedUser).HasColumnName("UpdatedUser");
        }
    }
}