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
        }

        #endregion
    }
}