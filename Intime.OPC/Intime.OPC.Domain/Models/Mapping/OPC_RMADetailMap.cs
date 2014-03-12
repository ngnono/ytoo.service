using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Intime.OPC.Domain.Models.Mapping
{
    public class OPC_RMADetailMap : EntityTypeConfiguration<OPC_RMADetail>
    {
        public OPC_RMADetailMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.OpcRmaId)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.CashNum)
                .HasMaxLength(50);

            this.Property(t => t.ProdSaleCode)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("OPC_RMADetail");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.OpcRmaId).HasColumnName("OpcRmaId");
            this.Property(t => t.CashNum).HasColumnName("CashNum");
            this.Property(t => t.StockId).HasColumnName("StockId");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.BackCount).HasColumnName("BackCount");
            this.Property(t => t.Price).HasColumnName("Price");
            this.Property(t => t.Amount).HasColumnName("Amount");
            this.Property(t => t.ProdSaleCode).HasColumnName("ProdSaleCode");
            this.Property(t => t.SalesPersonConfirm).HasColumnName("SalesPersonConfirm");
            this.Property(t => t.RefundDate).HasColumnName("RefundDate");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.CreatedUser).HasColumnName("CreatedUser");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.UpdatedUser).HasColumnName("UpdatedUser");
        }
    }
}
