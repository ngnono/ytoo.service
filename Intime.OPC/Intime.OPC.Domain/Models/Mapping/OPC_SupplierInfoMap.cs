using System.Data.Entity.ModelConfiguration;

namespace Intime.OPC.Domain.Models.Mapping
{
    public class OPC_SupplierInfoMap : EntityTypeConfiguration<OPC_SupplierInfo>
    {
        public OPC_SupplierInfoMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.SupplierNo)
                .IsRequired()
                .HasMaxLength(50);

            Property(t => t.SupplierName)
                .IsRequired()
                .HasMaxLength(50);

            Property(t => t.Corporate)
                .HasMaxLength(50);

            Property(t => t.Contact)
                .HasMaxLength(20);

            Property(t => t.Contract)
                .HasMaxLength(20);

            Property(t => t.Telephone)
                .HasMaxLength(20);

            Property(t => t.Address)
                .HasMaxLength(100);

            Property(t => t.FaxNo)
                .HasMaxLength(20);

            Property(t => t.TaxNo)
                .HasMaxLength(20);

            Property(t => t.Memo)
                .HasMaxLength(200);

            // Table & Column Mappings
            ToTable("OPC_SupplierInfo");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.SupplierNo).HasColumnName("SupplierNo");
            Property(t => t.SupplierName).HasColumnName("SupplierName");
            Property(t => t.StoreId).HasColumnName("StoreId");
            Property(t => t.Corporate).HasColumnName("Corporate");
            Property(t => t.Contact).HasColumnName("Contact");
            Property(t => t.Contract).HasColumnName("Contract");
            Property(t => t.Telephone).HasColumnName("Telephone");
            Property(t => t.Address).HasColumnName("Address");
            Property(t => t.FaxNo).HasColumnName("FaxNo");
            Property(t => t.TaxNo).HasColumnName("TaxNo");
            Property(t => t.Status).HasColumnName("Status");
            Property(t => t.Memo).HasColumnName("Memo");
            Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            Property(t => t.CreatedUser).HasColumnName("CreatedUser");
            Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            Property(t => t.UpdatedUser).HasColumnName("UpdatedUser");
        }
    }
}