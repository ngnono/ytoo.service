using System.Data.Entity.ModelConfiguration;

namespace Intime.OPC.Domain.Models.Mapping
{
    public class OPC_SaleDetailMap : EntityTypeConfiguration<OPC_SaleDetail>
    {
        public OPC_SaleDetailMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.ProdSaleCode)
                .HasMaxLength(50);

            Property(t => t.Remark)
                .HasMaxLength(500);

            // Table & Column Mappings
            ToTable("OPC_SaleDetail");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.SaleId).HasColumnName("SaleId");
            Property(t => t.Status).HasColumnName("Status");
            Property(t => t.StockId).HasColumnName("StockId");
            Property(t => t.SaleCount).HasColumnName("SaleCount");
            Property(t => t.Price).HasColumnName("Price");
            Property(t => t.BackNumber).HasColumnName("BackNumber");
            Property(t => t.ProdSaleCode).HasColumnName("ProdSaleCode");
            Property(t => t.Remark).HasColumnName("Remark");
            Property(t => t.RemarkDate).HasColumnName("RemarkDate");
            Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            Property(t => t.CreatedUser).HasColumnName("CreatedUser");
            Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            Property(t => t.UpdatedUser).HasColumnName("UpdatedUser");
        }
    }
}