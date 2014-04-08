using System.Data.Entity;
using Intime.OPC.Domain.Models;
using Intime.OPC.Domain.Models.Mapping;

namespace Intime.OPC.Repository
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
      
        public DbSet<OPC_AuthMenu> OPC_AuthMenu { get; set; }
        public DbSet<OPC_AuthRole> OPC_AuthRole { get; set; }
        public DbSet<OPC_AuthRoleMenu> OPC_AuthRoleMenu { get; set; }
        public DbSet<OPC_AuthRoleUser> OPC_AuthRoleUser { get; set; }
        public DbSet<OPC_AuthUser> OPC_AuthUser { get; set; }
        public DbSet<OPC_ChannelProduct> OPC_ChannelProduct { get; set; }
        public DbSet<OPC_OrderComment> OPC_OrderComment { get; set; }
        public DbSet<OPC_OrgInfo> OrgInfos { get; set; }
        public DbSet<OPC_RMA> OPC_RMA { get; set; }
        public DbSet<OPC_RMAComment> OPC_RMAComment { get; set; }
        public DbSet<OPC_RMADetail> OPC_RMADetail { get; set; }
        public DbSet<OPC_RMALog> OPC_RMALog { get; set; }
        public DbSet<OPC_Sale> OPC_Sale { get; set; }
        public DbSet<OPC_SaleComment> OPC_SaleComment { get; set; }
        public DbSet<OPC_SaleDetail> OPC_SaleDetail { get; set; }
        public DbSet<OPC_SaleLog> OPC_SaleLog { get; set; }
        public DbSet<OPC_SaleRMA> OPC_SaleRMA { get; set; }
        public DbSet<OPC_ShippingSale> ShippingSales { get; set; }
        public DbSet<OPC_ShippingSaleComment> OPC_ShippingSaleComment { get; set; }
        public DbSet<OPC_SKU> OPC_SKU { get; set; }
        public DbSet<OPC_Stock> OPC_Stock { get; set; }
        public DbSet<OPC_StorePriority> OPC_StorePriority { get; set; }
        public DbSet<OPC_SupplierInfo> OPC_SupplierInfo { get; set; }
        public DbSet<Order> Orders { get; set; }
       
        public DbSet<OrderItem> OrderItems { get; set; }
      
        public DbSet<RMA> RMAs { get; set; }
      
        public DbSet<RMAItem> RMAItems { get; set; }
      
        public DbSet<RMAReason> RMAReasons { get; set; }
     
        public DbSet<Section> Sections { get; set; }

        public DbSet<ShipVia> ShipVias { get; set; }
       
        public DbSet<Store> Stores { get; set; }
        public DbSet<OPC_SaleRMAComment> OPC_SaleRMAComments { get; set; }
 
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
        }
    }
}
