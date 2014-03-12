using Intime.OPC.Domain.Models.Mapping;
using System.Data.Entity;

namespace Intime.OPC.Domain.Models
{
    public partial class YintaiHZhouContext : DbContext
    {
        static YintaiHZhouContext()
        {
            Database.SetInitializer<YintaiHZhouContext>(null);
        }

        public YintaiHZhouContext()
            : base("Name=YintaiHZhouContext")
        {
        }

        public DbSet<OPC_AuthMenu> OPC_AuthMenu { get; set; }
        public DbSet<OPC_AuthRole> OPC_AuthRole { get; set; }
        public DbSet<OPC_AuthRoleMenu> OPC_AuthRoleMenu { get; set; }
        public DbSet<OPC_AuthRoleUser> OPC_AuthRoleUser { get; set; }
        public DbSet<OPC_AuthUser> OPC_AuthUser { get; set; }
        public DbSet<OPC_ChannelProduct> OPC_ChannelProduct { get; set; }
        public DbSet<OPC_OrderComment> OPC_OrderComment { get; set; }
        public DbSet<OPC_RMA> OPC_RMA { get; set; }
        public DbSet<OPC_RMADetail> OPC_RMADetail { get; set; }
        public DbSet<OPC_RMALog> OPC_RMALog { get; set; }
        public DbSet<OPC_Sale> OPC_Sale { get; set; }
        public DbSet<OPC_SaleComment> OPC_SaleComment { get; set; }
        public DbSet<OPC_SaleDetail> OPC_SaleDetail { get; set; }
        public DbSet<OPC_SaleLog> OPC_SaleLog { get; set; }
        public DbSet<OPC_SaleRMA> OPC_SaleRMA { get; set; }
        public DbSet<OPC_SKU> OPC_SKU { get; set; }
        public DbSet<OPC_Stock> OPC_Stock { get; set; }
        public DbSet<OPC_StorePriority> OPC_StorePriority { get; set; }
        public DbSet<OPC_SupplierInfo> OPC_SupplierInfo { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
           
            modelBuilder.Configurations.Add(new OPC_AuthMenuMap());
            modelBuilder.Configurations.Add(new OPC_AuthRoleMap());
            modelBuilder.Configurations.Add(new OPC_AuthRoleMenuMap());
            modelBuilder.Configurations.Add(new OPC_AuthRoleUserMap());
            modelBuilder.Configurations.Add(new OPC_AuthUserMap());
            modelBuilder.Configurations.Add(new OPC_ChannelProductMap());
            modelBuilder.Configurations.Add(new OPC_OrderCommentMap());
            modelBuilder.Configurations.Add(new OPC_RMAMap());
            modelBuilder.Configurations.Add(new OPC_RMADetailMap());
            modelBuilder.Configurations.Add(new OPC_RMALogMap());
            modelBuilder.Configurations.Add(new OPC_SaleMap());
            modelBuilder.Configurations.Add(new OPC_SaleCommentMap());
            modelBuilder.Configurations.Add(new OPC_SaleDetailMap());
            modelBuilder.Configurations.Add(new OPC_SaleLogMap());
            modelBuilder.Configurations.Add(new OPC_SaleRMAMap());
            modelBuilder.Configurations.Add(new OPC_SKUMap());
            modelBuilder.Configurations.Add(new OPC_StockMap());
            modelBuilder.Configurations.Add(new OPC_StorePriorityMap());
            modelBuilder.Configurations.Add(new OPC_SupplierInfoMap());
           
        }
    }
}
