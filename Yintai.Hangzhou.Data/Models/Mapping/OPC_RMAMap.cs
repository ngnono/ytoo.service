using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Yintai.Hangzhou.Data.Models.Mapping
{
    public partial class OPC_RMAEntityMap : EntityTypeConfiguration<OPC_RMAEntity>
    {
        public OPC_RMAEntityMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.SaleOrderNo)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.RMANo)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.OrderNo)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.BankName)
                .HasMaxLength(200);

            this.Property(t => t.BankAccount)
                .HasMaxLength(20);

            this.Property(t => t.BankCard)
                .HasMaxLength(50);

            this.Property(t => t.RejectReason)
                .HasMaxLength(500);

            this.Property(t => t.GiftReason)
                .HasMaxLength(100);

            this.Property(t => t.InvoiceReason)
                .HasMaxLength(100);

            this.Property(t => t.RebatePointReason)
                .HasMaxLength(100);

            this.Property(t => t.PostalFeeReason)
                .HasMaxLength(100);

            this.Property(t => t.ContactPhone)
                .HasMaxLength(20);

            this.Property(t => t.ShipNo)
                .HasMaxLength(50);

            this.Property(t => t.MailAddress)
                .HasMaxLength(1000);

            this.Property(t => t.ContactPerson)
                .HasMaxLength(20);

            this.Property(t => t.RMACashNum)
                .HasMaxLength(50);

            this.Property(t => t.Reason)
                .HasMaxLength(200);

            this.Property(t => t.SaleRMASource)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("OPC_RMA");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.SaleOrderNo).HasColumnName("SaleOrderNo");
            this.Property(t => t.RMANo).HasColumnName("RMANo");
            this.Property(t => t.IsInquirer).HasColumnName("IsInquirer");
            this.Property(t => t.SectionId).HasColumnName("SectionId");
            this.Property(t => t.StoreId).HasColumnName("StoreId");
            this.Property(t => t.Count).HasColumnName("Count");
            this.Property(t => t.RefundAmount).HasColumnName("RefundAmount");
            this.Property(t => t.IsShipping).HasColumnName("IsShipping");
            this.Property(t => t.IsPackage).HasColumnName("IsPackage");
            this.Property(t => t.OrderNo).HasColumnName("OrderNo");
            this.Property(t => t.RMAType).HasColumnName("RMAType");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.RMAAmount).HasColumnName("RMAAmount");
            this.Property(t => t.BankName).HasColumnName("BankName");
            this.Property(t => t.BankAccount).HasColumnName("BankAccount");
            this.Property(t => t.BankCard).HasColumnName("BankCard");
            this.Property(t => t.RejectReason).HasColumnName("RejectReason");
            this.Property(t => t.RebatePostfee).HasColumnName("RebatePostfee");
            this.Property(t => t.Chargepostfee).HasColumnName("Chargepostfee");
            this.Property(t => t.ActualAmount).HasColumnName("ActualAmount");
            this.Property(t => t.GiftReason).HasColumnName("GiftReason");
            this.Property(t => t.InvoiceReason).HasColumnName("InvoiceReason");
            this.Property(t => t.RebatePointReason).HasColumnName("RebatePointReason");
            this.Property(t => t.PostalFeeReason).HasColumnName("PostalFeeReason");
            this.Property(t => t.ChargeGiftFee).HasColumnName("ChargeGiftFee");
            this.Property(t => t.ContactPhone).HasColumnName("ContactPhone");
            this.Property(t => t.ShipviaId).HasColumnName("ShipviaId");
            this.Property(t => t.ShipNo).HasColumnName("ShipNo");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.MailAddress).HasColumnName("MailAddress");
            this.Property(t => t.RMAReason).HasColumnName("RMAReason");
            this.Property(t => t.ContactPerson).HasColumnName("ContactPerson");
            this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            this.Property(t => t.CreatedUser).HasColumnName("CreatedUser");
            this.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            this.Property(t => t.UpdatedUser).HasColumnName("UpdatedUser");
            this.Property(t => t.RMACashNum).HasColumnName("RMACashNum");
            this.Property(t => t.RMACashDate).HasColumnName("RMACashDate");
            this.Property(t => t.Reason).HasColumnName("Reason");
            this.Property(t => t.BackDate).HasColumnName("BackDate");
            this.Property(t => t.CustomerAuthDate).HasColumnName("CustomerAuthDate");
            this.Property(t => t.StoreFee).HasColumnName("StoreFee");
            this.Property(t => t.CustomFee).HasColumnName("CustomFee");
            this.Property(t => t.CompensationFee).HasColumnName("CompensationFee");
            this.Property(t => t.SaleRMASource).HasColumnName("SaleRMASource");
            this.Property(t => t.RMAStatus).HasColumnName("RMAStatus");
            this.Property(t => t.RMACashStatus).HasColumnName("RMACashStatus");
            this.Property(t => t.RealRMASumMoney).HasColumnName("RealRMASumMoney");
            this.Property(t => t.RecoverableSumMoney).HasColumnName("RecoverableSumMoney");
		Init();
        }

		partial void Init();
    }
}
