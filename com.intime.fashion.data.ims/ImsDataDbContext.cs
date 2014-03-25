namespace com.intime.fashion.data.ims
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class ImsDataDbContext : DbContext
    {
        public ImsDataDbContext()
            : base("name=ImsDataDbContext")
        {
        }

        public virtual DbSet<IMS_Associate> IMS_Associate { get; set; }
        public virtual DbSet<IMS_AssociateIncome> IMS_AssociateIncome { get; set; }
        public virtual DbSet<IMS_AssociateIncomeHistory> IMS_AssociateIncomeHistory { get; set; }
        public virtual DbSet<IMS_AssociateIncomeRequest> IMS_AssociateIncomeRequest { get; set; }
        public virtual DbSet<IMS_AssociateIncomeRule> IMS_AssociateIncomeRule { get; set; }
        public virtual DbSet<IMS_AssociateIncomeRuleFix> IMS_AssociateIncomeRuleFix { get; set; }
        public virtual DbSet<IMS_AssociateIncomeRuleFlatten> IMS_AssociateIncomeRuleFlatten { get; set; }
        public virtual DbSet<IMS_AssociateIncomeRuleFlex> IMS_AssociateIncomeRuleFlex { get; set; }
        public virtual DbSet<IMS_AssociateItems> IMS_AssociateItems { get; set; }
        public virtual DbSet<IMS_Combo> IMS_Combo { get; set; }
        public virtual DbSet<IMS_Combo2Product> IMS_Combo2Product { get; set; }
        public virtual DbSet<IMS_GiftCard> IMS_GiftCard { get; set; }
        public virtual DbSet<IMS_GiftCardItem> IMS_GiftCardItem { get; set; }
        public virtual DbSet<IMS_GiftCardOrder> IMS_GiftCardOrder { get; set; }
        public virtual DbSet<IMS_GiftCardRecharge> IMS_GiftCardRecharge { get; set; }
        public virtual DbSet<IMS_GiftCardTransfers> IMS_GiftCardTransfers { get; set; }
        public virtual DbSet<IMS_GiftCardUser> IMS_GiftCardUser { get; set; }
        public virtual DbSet<IMS_InviteCode> IMS_InviteCode { get; set; }
        public virtual DbSet<IMS_SalesCode> IMS_SalesCode { get; set; }
        public virtual DbSet<IMS_SectionBrand> IMS_SectionBrand { get; set; }
        public virtual DbSet<IMS_SectionOperator> IMS_SectionOperator { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IMS_AssociateIncomeHistory>()
                .Property(e => e.SourceNo)
                .IsUnicode(false);

            modelBuilder.Entity<IMS_AssociateIncomeRequest>()
                .Property(e => e.BankNo)
                .IsUnicode(false);

            modelBuilder.Entity<IMS_AssociateIncomeRuleFlatten>()
                .Property(e => e.Percentage)
                .HasPrecision(2, 2);

            modelBuilder.Entity<IMS_AssociateIncomeRuleFlex>()
                .Property(e => e.Percentage)
                .HasPrecision(2, 2);

            modelBuilder.Entity<IMS_GiftCardOrder>()
                .Property(e => e.No)
                .IsUnicode(false);

            modelBuilder.Entity<IMS_GiftCardRecharge>()
                .Property(e => e.PurchaseId)
                .IsUnicode(false);

            modelBuilder.Entity<IMS_GiftCardRecharge>()
                .Property(e => e.ChargePhone)
                .IsUnicode(false);

            modelBuilder.Entity<IMS_GiftCardTransfers>()
                .Property(e => e.Phone)
                .IsUnicode(false);

            modelBuilder.Entity<IMS_GiftCardUser>()
                .Property(e => e.GiftCardAccount)
                .IsUnicode(false);

            modelBuilder.Entity<IMS_InviteCode>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<IMS_SalesCode>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<IMS_SectionOperator>()
                .Property(e => e.Brands)
                .IsUnicode(false);

            modelBuilder.Entity<Order>()
                .Property(e => e.OrderNo)
                .IsUnicode(false);

            modelBuilder.Entity<Order>()
                .Property(e => e.TotalAmount)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Order>()
                .Property(e => e.RecAmount)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Order>()
                .Property(e => e.PaymentMethodCode)
                .IsUnicode(false);

            modelBuilder.Entity<Order>()
                .Property(e => e.ShippingZipCode)
                .IsUnicode(false);

            modelBuilder.Entity<Order>()
                .Property(e => e.ShippingContactPhone)
                .IsUnicode(false);

            modelBuilder.Entity<Order>()
                .Property(e => e.ShippingFee)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Order>()
                .Property(e => e.ShippingNo)
                .IsUnicode(false);

            modelBuilder.Entity<Order>()
                .Property(e => e.InvoiceAmount)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Order>()
                .Property(e => e.OrderSource)
                .IsUnicode(false);

            modelBuilder.Entity<OrderItem>()
                .Property(e => e.OrderNo)
                .IsUnicode(false);

            modelBuilder.Entity<OrderItem>()
                .Property(e => e.StoreItemNo)
                .IsUnicode(false);

            modelBuilder.Entity<OrderItem>()
                .Property(e => e.UnitPrice)
                .HasPrecision(10, 2);

            modelBuilder.Entity<OrderItem>()
                .Property(e => e.ItemPrice)
                .HasPrecision(10, 2);

            modelBuilder.Entity<OrderItem>()
                .Property(e => e.ExtendPrice)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Product>()
                .Property(e => e.UnitPrice)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Product>()
                .Property(e => e.SkuCode)
                .IsUnicode(false);

            modelBuilder.Entity<Product>()
                .Property(e => e.BarCode)
                .IsUnicode(false);
        }
    }
}
