using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Intime.OPC.Domain.Models.Mapping
{
    public class SectionMap : EntityTypeConfiguration<Section>
    {
        public SectionMap()
        {
            // Primary Key
            HasKey(t => t.Id);
            Property(t => t.Id)
               .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // Properties
            Property(t => t.Location)
                .HasMaxLength(200);

            Property(t => t.ContactPhone)
                .IsRequired()
                .HasMaxLength(50);

            Property(t => t.ContactPerson)
                .HasMaxLength(20);

            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            Property(t => t.StoreCode)
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("Section");
            Property(t => t.Location).HasColumnName("Location");
            Property(t => t.ContactPhone).HasColumnName("ContactPhone");
            Property(t => t.Status).HasColumnName("Status");
            Property(t => t.BrandId).HasColumnName("BrandId");
            Property(t => t.StoreId).HasColumnName("StoreId");
            Property(t => t.CreateDate).HasColumnName("CreateDate");
            Property(t => t.CreateUser).HasColumnName("CreateUser");
            Property(t => t.UpdateDate).HasColumnName("UpdateDate");
            Property(t => t.UpdateUser).HasColumnName("UpdateUser");
            Property(t => t.ContactPerson).HasColumnName("ContactPerson");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.StoreCode).HasColumnName("StoreCode");
            Property(t => t.ChannelSectionId).HasColumnName("ChannelSectionId");
        }
    }
}