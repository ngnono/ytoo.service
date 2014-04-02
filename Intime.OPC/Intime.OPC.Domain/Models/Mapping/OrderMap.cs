using System.Data.Entity.ModelConfiguration;

namespace Intime.OPC.Domain.Models.Mapping
{
    public class OrderMap : EntityTypeConfiguration<Order>
    {
        public OrderMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.OrderNo)
                .IsRequired()
                .HasMaxLength(20);

            Property(t => t.PaymentMethodCode)
                .IsRequired()
                .HasMaxLength(10);

            Property(t => t.PaymentMethodName)
                .HasMaxLength(20);

            Property(t => t.ShippingZipCode)
                .HasMaxLength(20);

            Property(t => t.ShippingAddress)
                .HasMaxLength(500);

            Property(t => t.ShippingContactPerson)
                .HasMaxLength(10);

            Property(t => t.ShippingContactPhone)
                .HasMaxLength(20);

            Property(t => t.InvoiceSubject)
                .HasMaxLength(200);

            Property(t => t.InvoiceDetail)
                .HasMaxLength(200);

            Property(t => t.ShippingNo)
                .HasMaxLength(50);

            Property(t => t.Memo)
                .HasMaxLength(200);

            Property(t => t.OrderSource)
                .HasMaxLength(10);

            // Table & Column Mappings
            ToTable("Order");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.OrderNo).HasColumnName("OrderNo");
            Property(t => t.CustomerId).HasColumnName("CustomerId");
            Property(t => t.TotalAmount).HasColumnName("TotalAmount");
            Property(t => t.RecAmount).HasColumnName("RecAmount");
            Property(t => t.Status).HasColumnName("Status");
            Property(t => t.PaymentMethodCode).HasColumnName("PaymentMethodCode");
            Property(t => t.PaymentMethodName).HasColumnName("PaymentMethodName");
            Property(t => t.ShippingZipCode).HasColumnName("ShippingZipCode");
            Property(t => t.ShippingAddress).HasColumnName("ShippingAddress");
            Property(t => t.ShippingContactPerson).HasColumnName("ShippingContactPerson");
            Property(t => t.ShippingContactPhone).HasColumnName("ShippingContactPhone");
            Property(t => t.NeedInvoice).HasColumnName("NeedInvoice");
            Property(t => t.InvoiceSubject).HasColumnName("InvoiceSubject");
            Property(t => t.InvoiceDetail).HasColumnName("InvoiceDetail");
            Property(t => t.ShippingFee).HasColumnName("ShippingFee");
            Property(t => t.CreateDate).HasColumnName("CreateDate");
            Property(t => t.CreateUser).HasColumnName("CreateUser");
            Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            Property(t => t.UpdateUser).HasColumnName("UpdateUser");
            Property(t => t.ShippingNo).HasColumnName("ShippingNo");
            Property(t => t.ShippingVia).HasColumnName("ShippingVia");
            Property(t => t.StoreId).HasColumnName("StoreId");
            Property(t => t.BrandId).HasColumnName("BrandId");
            Property(t => t.Memo).HasColumnName("Memo");
            Property(t => t.InvoiceAmount).HasColumnName("InvoiceAmount");
            Property(t => t.TotalPoints).HasColumnName("TotalPoints");
            Property(t => t.OrderSource).HasColumnName("OrderSource");
            Property(t => t.OrderType).HasColumnName("OrderProductType");
        }
    }
}