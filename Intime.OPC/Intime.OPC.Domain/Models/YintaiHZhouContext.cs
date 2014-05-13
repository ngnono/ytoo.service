using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Intime.OPC.Domain.Models.Mapping;

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


        public DbSet<Brand> Brands { get; set; }
      
        public DbSet<OPC_AuthMenu> OPC_AuthMenus { get; set; }
        public DbSet<OPC_AuthRole> OPC_AuthRoles { get; set; }
        public DbSet<OPC_AuthRoleMenu> OPC_AuthRoleMenus { get; set; }
        public DbSet<OPC_AuthRoleUser> OPC_AuthRoleUsers { get; set; }
        public DbSet<OPC_AuthUser> OPC_AuthUsers { get; set; }
        public DbSet<OPC_ChannelProduct> OPC_ChannelProducts { get; set; }
        public DbSet<OPC_OrderComment> OPC_OrderComments { get; set; }
        public DbSet<OPC_OrgInfo> OPC_OrgInfos { get; set; }
        public DbSet<OPC_RMA> OPC_RMAs { get; set; }
        public DbSet<OPC_RMAComment> OPC_RMAComments { get; set; }
        public DbSet<OPC_RMADetail> OPC_RMADetails { get; set; }
        public DbSet<OPC_RMALog> OPC_RMALogs { get; set; }
        public DbSet<OPC_Sale> OPC_Sales { get; set; }
        public DbSet<OPC_SaleComment> OPC_SaleComments { get; set; }
        public DbSet<OPC_SaleDetail> OPC_SaleDetails { get; set; }
        public DbSet<OPC_SaleLog> OPC_SaleLogs { get; set; }
        public DbSet<OPC_SaleRMA> OPC_SaleRMAs { get; set; }
        public DbSet<OPC_ShippingSaleComment> OPC_ShippingSaleComments { get; set; }
        public DbSet<OPC_SKU> OPC_SKUs { get; set; }
        public DbSet<OPC_Stock> OPC_Stocks { get; set; }
        public DbSet<OPC_StorePriority> OPC_StorePriorities { get; set; }
        public DbSet<OPC_SupplierInfo> OPC_SupplierInfos { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<RMA> RMAs { get; set; }
        public DbSet<RMAItem> RMAItems { get; set; }
        public DbSet<RMAReason> RMAReasons { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<ShipVia> ShipVias { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<OPC_SaleRMAComment> OPC_SaleRMAComments { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<OrderTransaction> OrderTransactions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }  
        public DbSet<OPC_ShippingSale> OPC_ShippingSales { get; set; }
        public DbSet<Supplier_Brand> Supplier_Brand { get; set; }
        public DbSet<IMS_SectionBrand> SectionBrands { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new OPC_AuthMenuMap());
            modelBuilder.Configurations.Add(new OPC_AuthRoleMap());
            modelBuilder.Configurations.Add(new OPC_AuthRoleMenuMap());
            modelBuilder.Configurations.Add(new OPC_AuthRoleUserMap());
            modelBuilder.Configurations.Add(new OPC_AuthUserMap());
            modelBuilder.Configurations.Add(new OPC_ChannelProductMap());
            modelBuilder.Configurations.Add(new OPC_OrderCommentMap());
            modelBuilder.Configurations.Add(new OPC_OrgInfoMap());
            modelBuilder.Configurations.Add(new OPC_RMAMap());
            modelBuilder.Configurations.Add(new OPC_RMACommentMap());
            modelBuilder.Configurations.Add(new OPC_RMADetailMap());
            modelBuilder.Configurations.Add(new OPC_RMALogMap());
            modelBuilder.Configurations.Add(new OPC_SaleMap());
            modelBuilder.Configurations.Add(new OPC_SaleCommentMap());
            modelBuilder.Configurations.Add(new OPC_SaleDetailMap());
            modelBuilder.Configurations.Add(new OPC_SaleLogMap());
            modelBuilder.Configurations.Add(new OPC_SaleRMAMap());
            modelBuilder.Configurations.Add(new OPC_ShippingSaleMap());
            modelBuilder.Configurations.Add(new OPC_ShippingSaleCommentMap());
            modelBuilder.Configurations.Add(new OPC_SKUMap());
            modelBuilder.Configurations.Add(new OPC_StockMap());
            modelBuilder.Configurations.Add(new OPC_StorePriorityMap());
            modelBuilder.Configurations.Add(new OPC_SupplierInfoMap());
            modelBuilder.Configurations.Add(new OrderMap());
            modelBuilder.Configurations.Add(new OrderItemMap());
            modelBuilder.Configurations.Add(new RMAMap());
            modelBuilder.Configurations.Add(new RMAItemMap());
            modelBuilder.Configurations.Add(new RMAReasonMap());
            modelBuilder.Configurations.Add(new SectionMap());
            modelBuilder.Configurations.Add(new ShipViaMap());
            modelBuilder.Configurations.Add(new StoreMap());
            modelBuilder.Configurations.Add(new BrandMap());
            modelBuilder.Configurations.Add(new OPC_SaleRMACommentMap());
            modelBuilder.Configurations.Add(new PaymentMethodMap());
            modelBuilder.Configurations.Add(new OrderTransactionMap());
            modelBuilder.Configurations.Add(new UserMap());
            modelBuilder.Configurations.Add(new ProductMap());
            modelBuilder.Configurations.Add(new Supplier_BrandMap());
            modelBuilder.Configurations.Add(new IMS_SectionBrandMap());
        }
    }
}
