using System.Data.Entity.ModelConfiguration;

namespace Intime.OPC.Domain.Models.Mapping
{
    public class OPC_SaleMap : EntityTypeConfiguration<OPC_Sale>
    {
        public OPC_SaleMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.OrderNo)
                .IsRequired()
                .HasMaxLength(20);

            Property(t => t.SaleOrderNo)
                .IsRequired()
                .HasMaxLength(50);

            Property(t => t.ShippingRemark)
                .HasMaxLength(500);

            Property(t => t.CashNum)
                .HasMaxLength(50);

            Property(t => t.Remark)
                .HasMaxLength(500);

            // Table & Column Mappings
            ToTable("OPC_Sale");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.OrderNo).HasColumnName("OrderNo");
            Property(t => t.SaleOrderNo).HasColumnName("SaleOrderNo");
            Property(t => t.SalesType).HasColumnName("SalesType");
            Property(t => t.ShipViaId).HasColumnName("ShipViaId");
            Property(t => t.Status).HasColumnName("Status");
            Property(t => t.ShippingCode).HasColumnName("ShippingCode");
            Property(t => t.ShippingFee).HasColumnName("ShippingFee");
            Property(t => t.ShippingStatus).HasColumnName("ShippingStatus");
            Property(t => t.ShippingRemark).HasColumnName("ShippingRemark");
            Property(t => t.SellDate).HasColumnName("SellDate");
            Property(t => t.IfTrans).HasColumnName("IfTrans");
            Property(t => t.TransStatus).HasColumnName("TransStatus");
            Property(t => t.SalesAmount).HasColumnName("SalesAmount");
            Property(t => t.SalesCount).HasColumnName("SalesCount");
            Property(t => t.CashStatus).HasColumnName("CashStatus");
            Property(t => t.CashNum).HasColumnName("CashNum");
            Property(t => t.CashDate).HasColumnName("CashDate");
            Property(t => t.SectionId).HasColumnName("SectionId");
            Property(t => t.PrintTimes).HasColumnName("PrintTimes");
            Property(t => t.Remark).HasColumnName("Remark");
            Property(t => t.RemarkDate).HasColumnName("RemarkDate");
            Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            Property(t => t.CreatedUser).HasColumnName("CreatedUser");
            Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            Property(t => t.UpdatedUser).HasColumnName("UpdatedUser");
        }
    }
}