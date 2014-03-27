using System.Data.Entity.ModelConfiguration;

namespace Intime.OPC.Domain.Models.Mapping
{
    public class StoreMap : EntityTypeConfiguration<Store>
    {
        public StoreMap()
        {
            // Primary Key
            HasKey(t => t.Id);

            // Properties
            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(64);

            Property(t => t.Description)
                .IsRequired();

            Property(t => t.Location)
                .IsRequired()
                .HasMaxLength(2048);

            Property(t => t.Tel)
                .IsRequired()
                .HasMaxLength(64);

            Property(t => t.RMAAddress)
                .HasMaxLength(200);

            Property(t => t.RMAZipCode)
                .HasMaxLength(50);

            Property(t => t.RMAPerson)
                .HasMaxLength(10);

            Property(t => t.RMAPhone)
                .HasMaxLength(20);

            // Table & Column Mappings
            ToTable("Store");
            Property(t => t.Id).HasColumnName("Id");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.Description).HasColumnName("Description");
            Property(t => t.Location).HasColumnName("Location");
            Property(t => t.Tel).HasColumnName("Tel");
            Property(t => t.CreatedUser).HasColumnName("CreatedUser");
            Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
            Property(t => t.UpdatedUser).HasColumnName("UpdatedUser");
            Property(t => t.Longitude).HasColumnName("Longitude");
            Property(t => t.Latitude).HasColumnName("Latitude");
            Property(t => t.Group_Id).HasColumnName("Group_Id");
            Property(t => t.Status).HasColumnName("Status");
            Property(t => t.Region_Id).HasColumnName("Region_Id");
            Property(t => t.StoreLevel).HasColumnName("StoreLevel");
            Property(t => t.GpsLat).HasColumnName("GpsLat");
            Property(t => t.GpsLng).HasColumnName("GpsLng");
            Property(t => t.GpsAlt).HasColumnName("GpsAlt");
            Property(t => t.ExStoreId).HasColumnName("ExStoreId");
            Property(t => t.RMAAddress).HasColumnName("RMAAddress");
            Property(t => t.RMAZipCode).HasColumnName("RMAZipCode");
            Property(t => t.RMAPerson).HasColumnName("RMAPerson");
            Property(t => t.RMAPhone).HasColumnName("RMAPhone");
        }
    }
}