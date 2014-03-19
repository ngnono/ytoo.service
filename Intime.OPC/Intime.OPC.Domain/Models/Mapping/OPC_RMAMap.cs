using System.Data.Entity.ModelConfiguration;

namespace Intime.OPC.Domain.Models.Mapping
{
    public class OPC_RMAMap : EntityTypeConfiguration<OPC_RMA>
    {
        public OPC_RMAMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.RMANo)
                .IsRequired()
                .HasMaxLength(20);

            Property(t => t.SourceDesc)
                .HasMaxLength(50);

            Property(t => t.OrderNo)
                .IsRequired()
                .HasMaxLength(20);

            Property(t => t.BankName)
                .HasMaxLength(200);

            Property(t => t.BankAccount)
                .HasMaxLength(20);

            Property(t => t.BankCard)
                .HasMaxLength(50);

            Property(t => t.RejectReason)
                .HasMaxLength(500);

            Property(t => t.GiftReason)
                .HasMaxLength(100);

            Property(t => t.InvoiceReason)
                .HasMaxLength(100);

            Property(t => t.RebatePointReason)
                .HasMaxLength(100);

            Property(t => t.PostalFeeReason)
                .HasMaxLength(100);

            Property(t => t.ContactPhone)
                .HasMaxLength(20);

            Property(t => t.ShipNo)
                .HasMaxLength(50);

            Property(t => t.MailAddress)
                .HasMaxLength(1000);

            Property(t => t.ContactPerson)
                .HasMaxLength(20);

            // Table & Column Mappings
            ToTable("OPC_RMA");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.SaleId).HasColumnName("SaleId");
            Property(t => t.RMANo).HasColumnName("RMANo");
            Property(t => t.IsInquirer).HasColumnName("IsInquirer");
            Property(t => t.SourceDesc).HasColumnName("SourceDesc");
            Property(t => t.Count).HasColumnName("Count");
            Property(t => t.RefundAmount).HasColumnName("RefundAmount");
            Property(t => t.IsShipping).HasColumnName("IsShipping");
            Property(t => t.IsPackage).HasColumnName("IsPackage");
            Property(t => t.OrderNo).HasColumnName("OrderNo");
            Property(t => t.RMAType).HasColumnName("RMAType");
            Property(t => t.Status).HasColumnName("Status");
            Property(t => t.RMAAmount).HasColumnName("RMAAmount");
            Property(t => t.BankName).HasColumnName("BankName");
            Property(t => t.BankAccount).HasColumnName("BankAccount");
            Property(t => t.BankCard).HasColumnName("BankCard");
            Property(t => t.RejectReason).HasColumnName("RejectReason");
            Property(t => t.RebatePostfee).HasColumnName("RebatePostfee");
            Property(t => t.Chargepostfee).HasColumnName("Chargepostfee");
            Property(t => t.ActualAmount).HasColumnName("ActualAmount");
            Property(t => t.GiftReason).HasColumnName("GiftReason");
            Property(t => t.InvoiceReason).HasColumnName("InvoiceReason");
            Property(t => t.RebatePointReason).HasColumnName("RebatePointReason");
            Property(t => t.PostalFeeReason).HasColumnName("PostalFeeReason");
            Property(t => t.ChargeGiftFee).HasColumnName("ChargeGiftFee");
            Property(t => t.ContactPhone).HasColumnName("ContactPhone");
            Property(t => t.ShipviaId).HasColumnName("ShipviaId");
            Property(t => t.ShipNo).HasColumnName("ShipNo");
            Property(t => t.UserId).HasColumnName("UserId");
            Property(t => t.MailAddress).HasColumnName("MailAddress");
            Property(t => t.RMAReason).HasColumnName("RMAReason");
            Property(t => t.ContactPerson).HasColumnName("ContactPerson");
            Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            Property(t => t.CreatedUser).HasColumnName("CreatedUser");
            Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            Property(t => t.UpdatedUser).HasColumnName("UpdatedUser");
        }
    }
}