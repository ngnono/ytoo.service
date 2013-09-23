using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Repository.Contract;
using Yintai.Hangzhou.Repository.Impl;

namespace Yintai.Hangzhou.WebSupport.Ioc
{
    internal class RepositoryIocRegister : BaseIocRegister
    {
        #region Overrides of BaseIocRegister

        public override void Register()
        {
            Current.Register<ICustomerRepository, CustomerRepository>();
            Current.Register<IOutSiteCustomerRepository, OutsiteCustomerRepository>();
            Current.Register<IUserAccountRepository, UserAccountRepository>();
            Current.Register<IStoreRepository, StoreRepository>();
            Current.Register<IVerifyCodeRepository, VerifyCodeRepository>();
            Current.Register<IPromotionRepository, PromotionRepository>();
            Current.Register<IResourceRepository, ResourceRepository>();
            Current.Register<IFavoriteRepository, FavoriteRepository>();

            Current.Register<IShareRepository, ShareRepository>();
            Current.Register<ICouponRepository, CouponRepository>();
            Current.Register<ITimeSeedRepository, TimeSeedRepository>();



            Current.Register<IBrandRepository, BrandRepository>();
            Current.Register<IDeviceTokenRepository, DeviceTokenRepository>();
            Current.Register<IDeviceLogsRepository, DeviceLogsRepository>();

            Current.Register<ICommentRepository, CommentRepository>();
            Current.Register<IRemindRepository, RemindRepository>();
            Current.Register<IProductRepository, ProductRepository>();
            Current.Register<ITagRepository, TagRepository>();

            Current.Register<IUserRoleRepository, UserRoleRepository>();
            Current.Register<IRoleRepository, RoleRepository>();
            Current.Register<IVUserRoleRepository, VUserRoleRepository>();

            Current.Register<ILikeRepository, LikeRepository>();
            Current.Register<IPointRepository, PointRepository>();

            Current.Register<IPromotionBrandRelationRepository, PromotionBrandRelationRepository>();
            Current.Register<IFeedbackRepository, FeedbackRepository>();

            Current.Register<ISpecialTopicProductRelationRepository, SpecialTopicProductRelationRepository>();
            Current.Register<ISpecialTopicRepository, SpecialTopicRepository>();

            Current.Register<ISeedRepository, SeedRepository>();

            Current.Register<IPromotionProductRelationRepository, PromotionProductRelationRepository>();
            Current.Register<IProductBulkRepository, ProductBulkRepository>();
            Current.Register<IAdminAccessRightRepository, AdminAccessRightRepository>();

            Current.Register<ICardRepository, CardRepository>();

            Current.Register<IBannerRepository, BannerRepository>();

            Current.Register<IHotWordRepository, HotWordRepository>();
            Current.Register<IUserAuthRepository, UserAuthRepository>();
            Current.Register<IStoreCouponsRepository, StoreCouponsRepository>();
            Current.Register<IStorePromotionRepository, StorePromotionRepository>();
            Current.Register<IPointOrderRuleRepository, PointOrderRuleRepository>();
            Current.Register<IStorePromotionScopeRepository, StorePromotionScopeRepository>();
            Current.Register<ICouponLogRepository, CouponLogRepository>();
            Current.Register<IOrderItemRepository, OrderItemRepository>();
            Current.Register<IOrderRepository, OrderRepository>();
            Current.Register<IOrderLogRepository, OrderLogRepository>();
            Current.Register<IOutboundRepository, OutboundRepository>();
            Current.Register<IOutboundItemRepository, OutboundItemRepository>();
            Current.Register<ISectionRepository, SectionRepository>();
            Current.Register<IInboundPackRepository, InboundPackageRepository>();
            Current.Register<IStoreRealRepository, StoreRealRepository>();
            Current.Register<ICategoryPropertyRepository, CategoryPropertyRepository>();
            Current.Register<ICategoryPropertyValueRepository, CategoryPropertyValueRepository>();
            Current.Register<IProductPropertyRepository, ProductPropertyRepository>();
            Current.Register<IProductPropertyValueRepository, ProductPropertyValueRepository>();
            Current.Register<IWXReplyRepository, WXReplyRepository>();
            Current.Register<IShippingAddressRepository, ShippingAddressRepository>();
            Current.Register<IShippViaRepository, ShipViaRepository>();
            Current.Register<IRMARepository, RMARepository>();
            Current.Register<IRMALogRepository, RMALogRepository>();
            Current.Register<IRMAItemRepository, RMAItemRepository>();
            Current.Register<IPMessageRepository, PMessageRepository>();
            Current.Register<IProductCode2StoreCodeRepository, ProductCode2StoreCodeRepository>();
            Current.Register<ICategoryRepository, CategoryRepository>();
            Current.Register<IOrder2ExRepository, Order2ExRepository>();
            Current.Register<IRMA2ExRepository, RMA2ExRepository>();
        }

        #endregion
    }
}