using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Intime.OPC.Domain.Models.Mapping
{
    public class OPC_RMADetailMap : EntityTypeConfiguration<OPC_RMADetail>
    {
        public OPC_RMADetailMap()
        {
            // Primary Key
            HasKey(t => t.Id);
            Property(t => t.Id)
               .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // Properties
            Property(t => t.OpcRmaId)
                .IsRequired()
                .HasMaxLength(50);

            Property(t => t.CashNum)
                .HasMaxLength(50);

            Property(t => t.ProdSaleCode)
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("OPC_RMADetail");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.OpcRmaId).HasColumnName("OpcRmaId");
            Property(t => t.CashNum).HasColumnName("CashNum");
            Property(t => t.StockId).HasColumnName("StockId");
            Property(t => t.Status).HasColumnName("Status");
            Property(t => t.BackCount).HasColumnName("BackCount");
            Property(t => t.Price).HasColumnName("Price");
            Property(t => t.Amount).HasColumnName("Amount");
            Property(t => t.ProdSaleCode).HasColumnName("ProdSaleCode");
            Property(t => t.SalesPersonConfirm).HasColumnName("SalesPersonConfirm");
            Property(t => t.RefundDate).HasColumnName("RefundDate");
            Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            Property(t => t.CreatedUser).HasColumnName("CreatedUser");
            Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            Property(t => t.UpdatedUser).HasColumnName("UpdatedUser");
        }
    }
}