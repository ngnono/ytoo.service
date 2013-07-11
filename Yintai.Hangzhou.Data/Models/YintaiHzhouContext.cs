using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Objects;
using EFCachingProvider;
using EFCachingProvider.Caching;
using EFTracingProvider;
using EFProviderWrapperToolkit;
using Yintai.Hangzhou.Data.Models.Mapping;

namespace Yintai.Hangzhou.Data.Models
{
    public partial class YintaiHangzhouContext : DbContext
    {
        private static readonly Architecture.Common.Logger.ILog _log;

        static YintaiHangzhouContext()
        {
            Database.SetInitializer<YintaiHangzhouContext>(null);
            _log = Architecture.Framework.ServiceLocation.ServiceLocator.Current.Resolve<Architecture.Common.Logger.ILog>();
        }

		/// <summary>
        /// 正式环境使用，无跟踪
        /// </summary>
        public YintaiHangzhouContext()
            : this("Name=YintaiHangzhouContext", "v1")
        {
        }

        /// <summary>
        /// 正式环境使用，无跟踪
        /// </summary>
        /// <param name="nameOrConnectionString"></param>
        /// <param name="version"></param>
        public YintaiHangzhouContext(string nameOrConnectionString, string version)
            : base(nameOrConnectionString)
        {
        }

        #region ef tracing

		public YintaiHangzhouContext(string nameOrConnectionString)
            : this(nameOrConnectionString, new InMemoryCache(512), CachingPolicy.CacheAll)
        {
        }

        public YintaiHangzhouContext(string nameOrConnectionString, ICache cacheProvider, CachingPolicy cachingPolicy)
            : base(Architecture.Common.Data.EF.EFTracingUtil.GetConnection(nameOrConnectionString), true)
        {
			var ctx = ((IObjectContextAdapter)this).ObjectContext;

            this.ObjectContext = ctx;

            EFTracingConnection tracingConnection;
            if (ObjectContext.TryUnwrapConnection(out tracingConnection))
            {
                ctx.GetTracingConnection().CommandExecuting += (s, e) => _log.Debug(e.ToTraceString());
            }

            EFCachingConnection cachingConnection;
            if (ObjectContext.TryUnwrapConnection(out cachingConnection))
            {
                Cache = cacheProvider;
                CachingPolicy = cachingPolicy;
            }
        }

        #endregion

		#region Tracing Extensions

        private ObjectContext ObjectContext { get; set; }

        private EFTracingConnection TracingConnection
        {
            get { return ObjectContext.UnwrapConnection<EFTracingConnection>(); }
        }

        public event EventHandler<CommandExecutionEventArgs> CommandExecuting
        {
            add { this.TracingConnection.CommandExecuting += value; }
            remove { this.TracingConnection.CommandExecuting -= value; }
        }

        public event EventHandler<CommandExecutionEventArgs> CommandFinished
        {
            add { this.TracingConnection.CommandFinished += value; }
            remove { this.TracingConnection.CommandFinished -= value; }
        }

        public event EventHandler<CommandExecutionEventArgs> CommandFailed
        {
            add { this.TracingConnection.CommandFailed += value; }
            remove { this.TracingConnection.CommandFailed -= value; }
        }

        #endregion

        #region Caching Extensions

        private EFCachingConnection CachingConnection
        {
            get { return ObjectContext.UnwrapConnection<EFCachingConnection>(); }
        }

        public ICache Cache
        {
            get { return CachingConnection.Cache; }
            set { CachingConnection.Cache = value; }
        }

        public CachingPolicy CachingPolicy
        {
            get { return CachingConnection.CachingPolicy; }
            set { CachingConnection.CachingPolicy = value; }
        }

        #endregion

		#region code reverse
        public DbSet<AdminAccessRightEntity> AdminAccessRights { get; set; }
        public DbSet<BannerEntity> Banners { get; set; }
        public DbSet<BrandEntity> Brands { get; set; }
        public DbSet<CardEntity> Cards { get; set; }
        public DbSet<CardBlackEntity> CardBlacks { get; set; }
        public DbSet<CategoryPropertyEntity> CategoryProperties { get; set; }
        public DbSet<CategoryPropertyValueEntity> CategoryPropertyValues { get; set; }
        public DbSet<CityEntity> Cities { get; set; }
        public DbSet<CommentEntity> Comments { get; set; }
        public DbSet<ConfigMsgEntity> ConfigMsgs { get; set; }
        public DbSet<CouponHistoryEntity> CouponHistories { get; set; }
        public DbSet<CouponLogEntity> CouponLogs { get; set; }
        public DbSet<DeviceLogEntity> DeviceLogs { get; set; }
        public DbSet<DeviceTokenEntity> DeviceTokens { get; set; }
        public DbSet<FavoriteEntity> Favorites { get; set; }
        public DbSet<FeedbackEntity> Feedbacks { get; set; }
        public DbSet<GroupEntity> Groups { get; set; }
        public DbSet<HotWordEntity> HotWords { get; set; }
        public DbSet<InboundPackageEntity> InboundPackages { get; set; }
        public DbSet<LikeEntity> Likes { get; set; }
        public DbSet<NotificationLogEntity> NotificationLogs { get; set; }
        public DbSet<OrderEntity> Orders { get; set; }
        public DbSet<OrderItemEntity> OrderItems { get; set; }
        public DbSet<OrderLogEntity> OrderLogs { get; set; }
        public DbSet<OutboundEntity> Outbounds { get; set; }
        public DbSet<OutboundItemEntity> OutboundItems { get; set; }
        public DbSet<OutsiteUserEntity> OutsiteUsers { get; set; }
        public DbSet<PaymentMethodEntity> PaymentMethods { get; set; }
        public DbSet<PMessageEntity> PMessages { get; set; }
        public DbSet<PointHistoryEntity> PointHistories { get; set; }
        public DbSet<PointOrderRuleEntity> PointOrderRules { get; set; }
        public DbSet<ProductEntity> Products { get; set; }
        public DbSet<ProductPropertyEntity> ProductProperties { get; set; }
        public DbSet<ProductPropertyStageEntity> ProductPropertyStages { get; set; }
        public DbSet<ProductPropertyValueEntity> ProductPropertyValues { get; set; }
        public DbSet<ProductStageEntity> ProductStages { get; set; }
        public DbSet<ProductUploadJobEntity> ProductUploadJobs { get; set; }
        public DbSet<PromotionEntity> Promotions { get; set; }
        public DbSet<Promotion2ProductEntity> Promotion2Product { get; set; }
        public DbSet<PromotionBrandRelationEntity> PromotionBrandRelations { get; set; }
        public DbSet<RemindEntity> Reminds { get; set; }
        public DbSet<ResourceEntity> Resources { get; set; }
        public DbSet<ResourceStageEntity> ResourceStages { get; set; }
        public DbSet<RMAEntity> RMAs { get; set; }
        public DbSet<RMAItemEntity> RMAItems { get; set; }
        public DbSet<RMALogEntity> RMALogs { get; set; }
        public DbSet<RoleEntity> Roles { get; set; }
        public DbSet<RoleAccessRightEntity> RoleAccessRights { get; set; }
        public DbSet<SectionEntity> Sections { get; set; }
        public DbSet<SeedEntity> Seeds { get; set; }
        public DbSet<ShareHistoryEntity> ShareHistories { get; set; }
        public DbSet<ShippingAddressEntity> ShippingAddresses { get; set; }
        public DbSet<ShipViaEntity> ShipVias { get; set; }
        public DbSet<SpecialTopicEntity> SpecialTopics { get; set; }
        public DbSet<SpecialTopicProductRelationEntity> SpecialTopicProductRelations { get; set; }
        public DbSet<StoreEntity> Stores { get; set; }
        public DbSet<StoreCouponEntity> StoreCoupons { get; set; }
        public DbSet<StorePromotionEntity> StorePromotions { get; set; }
        public DbSet<StorePromotionScopeEntity> StorePromotionScopes { get; set; }
        public DbSet<StoreRealEntity> StoreReals { get; set; }
        public DbSet<TagEntity> Tags { get; set; }
        public DbSet<TimeSeedEntity> TimeSeeds { get; set; }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<UserAccountEntity> UserAccounts { get; set; }
        public DbSet<UserAuthEntity> UserAuths { get; set; }
        public DbSet<UserRoleEntity> UserRoles { get; set; }
        public DbSet<VerifyCodeEntity> VerifyCodes { get; set; }
        public DbSet<WXReplyEntity> WXReplies { get; set; }
        public DbSet<VUserEntity> VUsers { get; set; }
        public DbSet<VUserRoleEntity> VUserRoles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
			Configuration.AutoDetectChangesEnabled = false;
            // 移除复数表名的契约
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // 防止黑幕交易 要不然每次都要访问 EdmMetadata这个表 EF4.1后可注释
            modelBuilder.Conventions.Remove<IncludeMetadataConvention>();

            modelBuilder.Configurations.Add(new AdminAccessRightEntityMap());
            modelBuilder.Configurations.Add(new BannerEntityMap());
            modelBuilder.Configurations.Add(new BrandEntityMap());
            modelBuilder.Configurations.Add(new CardEntityMap());
            modelBuilder.Configurations.Add(new CardBlackEntityMap());
            modelBuilder.Configurations.Add(new CategoryPropertyEntityMap());
            modelBuilder.Configurations.Add(new CategoryPropertyValueEntityMap());
            modelBuilder.Configurations.Add(new CityEntityMap());
            modelBuilder.Configurations.Add(new CommentEntityMap());
            modelBuilder.Configurations.Add(new ConfigMsgEntityMap());
            modelBuilder.Configurations.Add(new CouponHistoryEntityMap());
            modelBuilder.Configurations.Add(new CouponLogEntityMap());
            modelBuilder.Configurations.Add(new DeviceLogEntityMap());
            modelBuilder.Configurations.Add(new DeviceTokenEntityMap());
            modelBuilder.Configurations.Add(new FavoriteEntityMap());
            modelBuilder.Configurations.Add(new FeedbackEntityMap());
            modelBuilder.Configurations.Add(new GroupEntityMap());
            modelBuilder.Configurations.Add(new HotWordEntityMap());
            modelBuilder.Configurations.Add(new InboundPackageEntityMap());
            modelBuilder.Configurations.Add(new LikeEntityMap());
            modelBuilder.Configurations.Add(new NotificationLogEntityMap());
            modelBuilder.Configurations.Add(new OrderEntityMap());
            modelBuilder.Configurations.Add(new OrderItemEntityMap());
            modelBuilder.Configurations.Add(new OrderLogEntityMap());
            modelBuilder.Configurations.Add(new OutboundEntityMap());
            modelBuilder.Configurations.Add(new OutboundItemEntityMap());
            modelBuilder.Configurations.Add(new OutsiteUserEntityMap());
            modelBuilder.Configurations.Add(new PaymentMethodEntityMap());
            modelBuilder.Configurations.Add(new PMessageEntityMap());
            modelBuilder.Configurations.Add(new PointHistoryEntityMap());
            modelBuilder.Configurations.Add(new PointOrderRuleEntityMap());
            modelBuilder.Configurations.Add(new ProductEntityMap());
            modelBuilder.Configurations.Add(new ProductPropertyEntityMap());
            modelBuilder.Configurations.Add(new ProductPropertyStageEntityMap());
            modelBuilder.Configurations.Add(new ProductPropertyValueEntityMap());
            modelBuilder.Configurations.Add(new ProductStageEntityMap());
            modelBuilder.Configurations.Add(new ProductUploadJobEntityMap());
            modelBuilder.Configurations.Add(new PromotionEntityMap());
            modelBuilder.Configurations.Add(new Promotion2ProductEntityMap());
            modelBuilder.Configurations.Add(new PromotionBrandRelationEntityMap());
            modelBuilder.Configurations.Add(new RemindEntityMap());
            modelBuilder.Configurations.Add(new ResourceEntityMap());
            modelBuilder.Configurations.Add(new ResourceStageEntityMap());
            modelBuilder.Configurations.Add(new RMAEntityMap());
            modelBuilder.Configurations.Add(new RMAItemEntityMap());
            modelBuilder.Configurations.Add(new RMALogEntityMap());
            modelBuilder.Configurations.Add(new RoleEntityMap());
            modelBuilder.Configurations.Add(new RoleAccessRightEntityMap());
            modelBuilder.Configurations.Add(new SectionEntityMap());
            modelBuilder.Configurations.Add(new SeedEntityMap());
            modelBuilder.Configurations.Add(new ShareHistoryEntityMap());
            modelBuilder.Configurations.Add(new ShippingAddressEntityMap());
            modelBuilder.Configurations.Add(new ShipViaEntityMap());
            modelBuilder.Configurations.Add(new SpecialTopicEntityMap());
            modelBuilder.Configurations.Add(new SpecialTopicProductRelationEntityMap());
            modelBuilder.Configurations.Add(new StoreEntityMap());
            modelBuilder.Configurations.Add(new StoreCouponEntityMap());
            modelBuilder.Configurations.Add(new StorePromotionEntityMap());
            modelBuilder.Configurations.Add(new StorePromotionScopeEntityMap());
            modelBuilder.Configurations.Add(new StoreRealEntityMap());
            modelBuilder.Configurations.Add(new TagEntityMap());
            modelBuilder.Configurations.Add(new TimeSeedEntityMap());
            modelBuilder.Configurations.Add(new UserEntityMap());
            modelBuilder.Configurations.Add(new UserAccountEntityMap());
            modelBuilder.Configurations.Add(new UserAuthEntityMap());
            modelBuilder.Configurations.Add(new UserRoleEntityMap());
            modelBuilder.Configurations.Add(new VerifyCodeEntityMap());
            modelBuilder.Configurations.Add(new WXReplyEntityMap());
            modelBuilder.Configurations.Add(new VUserEntityMap());
            modelBuilder.Configurations.Add(new VUserRoleEntityMap());
        }

		#endregion
    }
}
