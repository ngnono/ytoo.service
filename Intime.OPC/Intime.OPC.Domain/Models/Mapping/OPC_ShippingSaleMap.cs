using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Intime.OPC.Domain.Models.Mapping
{
    public class OPC_ShippingSaleMap : EntityTypeConfiguration<OPC_ShippingSale>
    {
        public OPC_ShippingSaleMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);
            Property(t => t.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            // Properties
            this.Property(t => t.SaleOrderNo)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.ShippingCode)
                .HasMaxLength(50);

            this.Property(t => t.ShippingRemark)
                .HasMaxLength(500);
            this.Property(t => t.ShipViaName)
                .HasMaxLength(50);


            // Table & Column Mappings
            this.ToTable("OPC_ShippingSale");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.SaleOrderNo).HasColumnName("SaleOrderNo");
            this.Property(t => t.ShipViaId).HasColumnName("ShipViaId");
            this.Property(t => t.ShippingCode).HasColumnName("ShippingCode");
            this.Property(t => t.ShippingFee).HasColumnName("ShippingFee");
            this.Property(t => t.ShippingStatus).HasColumnName("ShippingStatus");
            this.Property(t => t.ShippingRemark).HasColumnName("ShippingRemark");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateUser).HasColumnName("CreateUser");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.UpdateUser).HasColumnName("UpdateUser");
            this.Property(t => t.ShipViaName).HasColumnName("ShipViaName");
        }
    }
}
