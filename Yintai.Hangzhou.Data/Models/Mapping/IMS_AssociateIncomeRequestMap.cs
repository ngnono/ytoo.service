using System.Data.Entity.ModelConfiguration;

namespace Yintai.Hangzhou.Data.Models.Mapping
{
    public class IMS_AssociateIncomeRequestMap : EntityTypeConfiguration<IMS_AssociateIncomeRequest>
    {
        public IMS_AssociateIncomeRequestMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.BankName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.BankNo)
                .IsRequired()
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("IMS_AssociateIncomeRequest");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.BankName).HasColumnName("BankName");
            this.Property(t => t.BankNo).HasColumnName("BankNo");
            this.Property(t => t.Amount).HasColumnName("Amount");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
        }
    }
}
