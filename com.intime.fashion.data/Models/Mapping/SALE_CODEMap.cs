using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace com.intime.fashion.data.erp.Models.Mapping
{
    public class SALE_CODEMap : EntityTypeConfiguration<SALE_CODE>
    {
        public SALE_CODEMap()
        {
            // Primary Key
            this.HasKey(t => t.SID);

            // Properties
            this.Property(t => t.SID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.SALE_CODE1)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.OPT_REAL_NAME)
                .HasMaxLength(40);

            this.Property(t => t.MEMO)
                .HasMaxLength(200);

            this.Property(t => t.SUPPLY_SHOP_CODE)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.SALE_CODE_NAME)
                .HasMaxLength(60);

            this.Property(t => t.ADDRESS)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("SALE_CODE", "YINTAIWEB");
            this.Property(t => t.SID).HasColumnName("SID");
            this.Property(t => t.SHOP_SID).HasColumnName("SHOP_SID");
            this.Property(t => t.SALE_CODE1).HasColumnName("SALE_CODE");
            this.Property(t => t.SUPPLY_SALING_BIT).HasColumnName("SUPPLY_SALING_BIT");
            this.Property(t => t.NET_SALE_BIT).HasColumnName("NET_SALE_BIT");
            this.Property(t => t.OPT_USER_SID).HasColumnName("OPT_USER_SID");
            this.Property(t => t.OPT_REAL_NAME).HasColumnName("OPT_REAL_NAME");
            this.Property(t => t.OPT_UPDATE_TIME).HasColumnName("OPT_UPDATE_TIME");
            this.Property(t => t.MEMO).HasColumnName("MEMO");
            this.Property(t => t.SUPPLY_SHOP_CODE).HasColumnName("SUPPLY_SHOP_CODE");
            this.Property(t => t.ACTIVE_BIT).HasColumnName("ACTIVE_BIT");
            this.Property(t => t.SALE_CODE_NAME).HasColumnName("SALE_CODE_NAME");
            this.Property(t => t.ADDRESS).HasColumnName("ADDRESS");

            // Relationships
            this.HasOptional(t => t.SHOP_INFO)
                .WithMany(t => t.SALE_CODE)
                .HasForeignKey(d => d.SHOP_SID);

        }
    }
}
