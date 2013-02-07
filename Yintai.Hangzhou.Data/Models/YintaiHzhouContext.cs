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
    public partial class YintaiHzhouContext : DbContext , Architecture.Common.Data.EF.IUnitOfWork
    {
        private static readonly Architecture.Common.Logger.ILog _log;

        static YintaiHzhouContext()
        {
            Database.SetInitializer<YintaiHangzhouContext>(null);
            _log = Architecture.Framework.ServiceLocation.ServiceLocator.Current.Resolve<Architecture.Common.Logger.ILog>();
        }

		public YintaiHzhouContext()
            : base("Name=YintaiHangzhouContext")
		{
		}

        #region ef tracing

		public YintaiHzhouContext(string nameOrConnectionString)
            : this(nameOrConnectionString, new InMemoryCache(512), CachingPolicy.CacheAll)
        {
        }

        public YintaiHzhouContext(string nameOrConnectionString, ICache cacheProvider, CachingPolicy cachingPolicy)
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

        public DbSet<BrandEntity> Brands { get; set; }
        public DbSet<CommentEntity> Comments { get; set; }
        public DbSet<CouponHistoryEntity> CouponHistories { get; set; }
        public DbSet<DeviceLogEntity> DeviceLogs { get; set; }
        public DbSet<DeviceTokenEntity> DeviceTokens { get; set; }
        public DbSet<FavoriteEntity> Favorites { get; set; }
        public DbSet<FeedbackEntity> Feedbacks { get; set; }
        public DbSet<GroupEntity> Groups { get; set; }
        public DbSet<LikeEntity> Likes { get; set; }
        public DbSet<NotificationLogEntity> NotificationLogs { get; set; }
        public DbSet<OutsiteUserEntity> OutsiteUsers { get; set; }
        public DbSet<PointHistoryEntity> PointHistories { get; set; }
        public DbSet<ProductEntity> Products { get; set; }
        public DbSet<ProductStageEntity> ProductStages { get; set; }
        public DbSet<ProductUploadJobEntity> ProductUploadJobs { get; set; }
        public DbSet<PromotionEntity> Promotions { get; set; }
        public DbSet<Promotion2ProductEntity> Promotion2Product { get; set; }
        public DbSet<PromotionBrandRelationEntity> PromotionBrandRelations { get; set; }
        public DbSet<RemindEntity> Reminds { get; set; }
        public DbSet<ResourceEntity> Resources { get; set; }
        public DbSet<ResourceStageEntity> ResourceStages { get; set; }
        public DbSet<RoleEntity> Roles { get; set; }
        public DbSet<SeedEntity> Seeds { get; set; }
        public DbSet<ShareHistoryEntity> ShareHistories { get; set; }
        public DbSet<SpecialTopicEntity> SpecialTopics { get; set; }
        public DbSet<SpecialTopicProductRelationEntity> SpecialTopicProductRelations { get; set; }
        public DbSet<StoreEntity> Stores { get; set; }
        public DbSet<TagEntity> Tags { get; set; }
        public DbSet<TimeSeedEntity> TimeSeeds { get; set; }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<UserAccountEntity> UserAccounts { get; set; }
        public DbSet<UserRoleEntity> UserRoles { get; set; }
        public DbSet<VerifyCodeEntity> VerifyCodes { get; set; }
        public DbSet<VUserEntity> VUsers { get; set; }
        public DbSet<VUserRoleEntity> VUserRoles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // 移除复数表名的契约
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // 防止黑幕交易 要不然每次都要访问 EdmMetadata这个表 EF4.1后可注释
            modelBuilder.Conventions.Remove<IncludeMetadataConvention>();

            modelBuilder.Configurations.Add(new BrandEntityMap());
            modelBuilder.Configurations.Add(new CommentEntityMap());
            modelBuilder.Configurations.Add(new CouponHistoryEntityMap());
            modelBuilder.Configurations.Add(new DeviceLogEntityMap());
            modelBuilder.Configurations.Add(new DeviceTokenEntityMap());
            modelBuilder.Configurations.Add(new FavoriteEntityMap());
            modelBuilder.Configurations.Add(new FeedbackEntityMap());
            modelBuilder.Configurations.Add(new GroupEntityMap());
            modelBuilder.Configurations.Add(new LikeEntityMap());
            modelBuilder.Configurations.Add(new NotificationLogEntityMap());
            modelBuilder.Configurations.Add(new OutsiteUserEntityMap());
            modelBuilder.Configurations.Add(new PointHistoryEntityMap());
            modelBuilder.Configurations.Add(new ProductEntityMap());
            modelBuilder.Configurations.Add(new ProductStageEntityMap());
            modelBuilder.Configurations.Add(new ProductUploadJobEntityMap());
            modelBuilder.Configurations.Add(new PromotionEntityMap());
            modelBuilder.Configurations.Add(new Promotion2ProductEntityMap());
            modelBuilder.Configurations.Add(new PromotionBrandRelationEntityMap());
            modelBuilder.Configurations.Add(new RemindEntityMap());
            modelBuilder.Configurations.Add(new ResourceEntityMap());
            modelBuilder.Configurations.Add(new ResourceStageEntityMap());
            modelBuilder.Configurations.Add(new RoleEntityMap());
            modelBuilder.Configurations.Add(new SeedEntityMap());
            modelBuilder.Configurations.Add(new ShareHistoryEntityMap());
            modelBuilder.Configurations.Add(new SpecialTopicEntityMap());
            modelBuilder.Configurations.Add(new SpecialTopicProductRelationEntityMap());
            modelBuilder.Configurations.Add(new StoreEntityMap());
            modelBuilder.Configurations.Add(new TagEntityMap());
            modelBuilder.Configurations.Add(new TimeSeedEntityMap());
            modelBuilder.Configurations.Add(new UserEntityMap());
            modelBuilder.Configurations.Add(new UserAccountEntityMap());
            modelBuilder.Configurations.Add(new UserRoleEntityMap());
            modelBuilder.Configurations.Add(new VerifyCodeEntityMap());
            modelBuilder.Configurations.Add(new VUserEntityMap());
            modelBuilder.Configurations.Add(new VUserRoleEntityMap());
        }

		public override int SaveChanges()
		{
			var c =  base.SaveChanges();

			return c;
		}

        protected override void Dispose(bool disposing)
        {
            System.Diagnostics.Debug.WriteLine("context closed");
            base.Dispose(disposing);
        }
    }
}
