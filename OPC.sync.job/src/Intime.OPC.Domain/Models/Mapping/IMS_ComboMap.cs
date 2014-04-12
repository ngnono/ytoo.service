using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Intime.OPC.Domain.Models.Mapping
{
    public class IMS_ComboMapper : EntityTypeConfiguration<IMS_Combo>
    {
        public IMS_ComboMapper()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Desc)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Private2Name)
                .IsRequired()
                .HasMaxLength(10);

            // Table & Column Mappings
            this.ToTable("IMS_Combo");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Desc).HasColumnName("Desc");
            this.Property(t => t.Price).HasColumnName("Price");
            this.Property(t => t.Private2Name).HasColumnName("Private2Name");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateUser).HasColumnName("CreateUser");
            this.Property(t => t.OnlineDate).HasColumnName("OnlineDate");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.UpdateUser).HasColumnName("UpdateUser");
            this.Property(t => t.ProductType).HasColumnName("ProductType");
        }
    }
}
