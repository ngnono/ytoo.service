using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Yintai.Hangzhou.Data.Models.Mapping
{
    public partial class IMS_InviteCodeRequestEntityMap : EntityTypeConfiguration<IMS_InviteCodeRequestEntity>
    {
        public IMS_InviteCodeRequestEntityMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.ContactMobile)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.SectionCode)
                .HasMaxLength(50);

            this.Property(t => t.SectionName)
                .HasMaxLength(50);

            this.Property(t => t.OperatorCode)
                .HasMaxLength(50);


            // Table & Column Mappings
            this.ToTable("IMS_InviteCodeRequest");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.ContactMobile).HasColumnName("ContactMobile");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.SectionCode).HasColumnName("SectionCode");
            this.Property(t => t.SectionName).HasColumnName("SectionName");
            this.Property(t => t.OperatorCode).HasColumnName("OperatorCode");
            this.Property(t => t.StoreId).HasColumnName("StoreId");
            this.Property(t => t.RequestType).HasColumnName("RequestType");
            this.Property(t => t.CreateUser).HasColumnName("CreateUser");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.UpdateUser).HasColumnName("UpdateUser");
            this.Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.UserId).HasColumnName("UserId");
		Init();
        }

		partial void Init();
    }
}