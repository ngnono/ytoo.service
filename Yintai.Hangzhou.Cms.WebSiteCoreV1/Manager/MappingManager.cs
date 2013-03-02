using System;
using System.Collections.Generic;
using Yintai.Architecture.Common.Models;
using Yintai.Architecture.Framework.Mapping;
using Yintai.Architecture.Framework.ServiceLocation;
using Yintai.Hangzhou.Cms.WebSiteCoreV1.Models;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Repository.Contract;
using Yintai.Hangzhou.Service.Manager;
using System.Linq;
using MM = Yintai.Hangzhou.Service.Manager.MappingManagerV2;

namespace Yintai.Hangzhou.Cms.WebSiteCoreV1.Manager
{
    public class MappingManager : BaseMappingManager
    {
        private readonly MM _m;

        public MappingManager()
        {
            _m = new MM();
        }

        #region
        public static T MapCommon<T>(T source)
        {
           return Mapper.Map<T, T>(source);
        }
        public static TT MapCommon<T,TT>(T source)
        {
            return Mapper.Map<T, TT>(source);
        }
        #endregion

        #region user

        public UserEntity UserEntityMapping(UserEntity source, UserEntity target)
        {
            return _m.UserEntityMapping(source, target);

            //var result = Mapper.Map(source, target);

            //return PointHistoryEntityCheck(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public UserEntity UserEntityMapping(UserModel source)
        {
            if (source == null)
            {
                return null;
            }

            return _m.UserEntityMapping(source);
        }

        public UserModel UserModelMapping(UserEntity source)
        {
            if (source == null)
            {
                return null;
            }

            return _m.UserModelMapping(source);
        }

        public IEnumerable<UserModel> UserModelMapping(List<UserEntity> source)
        {
            if (source == null || source.Count == 0)
            {
                return new List<UserModel>(0);
            }

            return _m.UserModelMapping(source);
        }

        #endregion

        #region Customer

        public UserModel UserModelMapping(UserModel source, UserModel target)
        {
            var result = Mapper.Map(source, target);

            return UserModelCheck(result);
        }

        /// <summary>
        /// 检查 entity 为 null的情况
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private UserModel UserModelCheck(UserModel source)
        {
            source.Description = source.Description ?? String.Empty;
            source.Name = source.Name ?? String.Empty;

            return source;
        }

        public UserModel UserModelMapping(CustomerViewModel source)
        {
            if (source == null)
            {
                return null;
            }

            var target = Mapper.Map<CustomerViewModel, UserModel>(source);
            target.CreatedDate = DateTime.Now;
            target.UpdatedDate = DateTime.Now;

            return UserModelCheck(target);
        }

        public CustomerViewModel CustomerViewMapping(UserModel source)
        {
            if (source == null)
            {
                return null;
            }

            var target = Mapper.Map<UserModel, CustomerViewModel>(source);
            target.CreatedDate = DateTime.Now;
            target.UpdatedDate = DateTime.Now;

            return target;
        }

        public IEnumerable<CustomerViewModel> CustomerViewMapping(List<UserModel> source)
        {
            if (source == null)
            {
                return null;
            }

            var list = new List<CustomerViewModel>(source.Count);
            foreach (var item in source)
            {
                var target = CustomerViewMapping(item);
                if (target != null)
                {
                    list.Add(target);
                }
            }

            return list;
        }

        #endregion

        #region Point

        public PointHistoryEntity PointHistoryEntityMapping(PointHistoryEntity source, PointHistoryEntity target)
        {
            var result = Mapper.Map(source, target);

            return PointHistoryEntityCheck(result);
        }

        /// <summary>
        /// 检查 entity 为 null的情况
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private PointHistoryEntity PointHistoryEntityCheck(PointHistoryEntity source)
        {
            source.Description = source.Description ?? String.Empty;
            source.Name = source.Name ?? String.Empty;

            return source;
        }

        public PointHistoryEntity PointHistoryEntityMapping(PointViewModel source)
        {
            if (source == null)
            {
                return null;
            }

            var target = Mapper.Map<PointViewModel, PointHistoryEntity>(source);
            target.CreatedDate = DateTime.Now;
            target.UpdatedDate = DateTime.Now;

            return PointHistoryEntityCheck(target);
        }

        public PointViewModel PointViewMapping(PointHistoryEntity source)
        {
            if (source == null)
            {
                return null;
            }

            var target = Mapper.Map<PointHistoryEntity, PointViewModel>(source);

            return target;
        }

        public IEnumerable<PointViewModel> PointViewMapping(List<PointHistoryEntity> source)
        {
            if (source == null)
            {
                return null;
            }

            var list = new List<PointViewModel>(source.Count);
            foreach (var item in source)
            {
                var target = PointViewMapping(item);
                if (target != null)
                {
                    list.Add(target);
                }
            }

            return list;
        }

        #endregion

        #region Resource

        public ResourceEntity ResourceEntityMapping(ResourceEntity source, ResourceEntity target)
        {
            var result = Mapper.Map(source, target);

            return ResourceEntityCheck(result);
        }

        /// <summary>
        /// 检查 entity 为 null的情况
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private ResourceEntity ResourceEntityCheck(ResourceEntity source)
        {
            source.ExtName = source.ExtName ?? String.Empty;
            source.Name = source.Name ?? String.Empty;
            source.Size = source.Size ?? String.Empty;
            source.Domain = source.Domain ?? String.Empty;

            return source;
        }

        public ResourceEntity ResourceEntityMapping(ResourceViewModel source)
        {
            if (source == null)
            {
                return null;
            }

            var target = Mapper.Map<ResourceViewModel, ResourceEntity>(source);
            target.CreatedDate = DateTime.Now;
            target.UpdatedDate = DateTime.Now;

            return ResourceEntityCheck(target);
        }

        public ResourceViewModel ResourceViewMapping(ResourceEntity source)
        {
            if (source == null)
            {
                return null;
            }

            var target = Mapper.Map<ResourceEntity, ResourceViewModel>(source);

            if (String.IsNullOrWhiteSpace(target.Domain))
            {
                switch ((ResourceType)target.Type)
                {
                    case ResourceType.Image:
                        target.Domain = ConfigManager.GetHttpApiImagePath();
                        break;
                    case ResourceType.Sound:
                        target.Domain = ConfigManager.GetHttpApiSoundPath();
                        break;
                    case ResourceType.Video:
                        target.Domain = ConfigManager.GetHttpApivideoPath();
                        break;
                    case ResourceType.Default:
                        target.Domain = ConfigManager.GetHttpApidefPath();
                        break;
                }
            }

            return target;
        }

        public IEnumerable<ResourceViewModel> ResourceViewMapping(List<ResourceEntity> source)
        {
            if (source == null)
            {
                return null;
            }

            var list = new List<ResourceViewModel>(source.Count);
            foreach (var item in source)
            {
                var target = ResourceViewMapping(item);
                if (target != null)
                {
                    list.Add(target);
                }
            }

            return list;
        }

        #endregion

        #region Feedback

        public FeedbackEntity FeedbackEntityMapping(FeedbackEntity source, FeedbackEntity target)
        {
            var result = Mapper.Map(source, target);

            return FeedbackEntityCheck(result);
        }

        /// <summary>
        /// 检查 entity 为 null的情况
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private FeedbackEntity FeedbackEntityCheck(FeedbackEntity source)
        {
            source.Contact = source.Contact ?? String.Empty;
            source.Content = source.Content ?? String.Empty;

            return source;
        }

        public FeedbackEntity FeedbackEntityMapping(FeedbackViewModel source)
        {
            if (source == null)
            {
                return null;
            }

            var target = Mapper.Map<FeedbackViewModel, FeedbackEntity>(source);
            target.CreatedDate = DateTime.Now;
            target.UpdatedDate = DateTime.Now;

            return FeedbackEntityCheck(target);
        }

        public FeedbackViewModel FeedbackViewMapping(FeedbackEntity source)
        {
            if (source == null)
            {
                return null;
            }

            var target = Mapper.Map<FeedbackEntity, FeedbackViewModel>(source);

            return target;
        }

        public IEnumerable<FeedbackViewModel> FeedbackViewMapping(List<FeedbackEntity> source)
        {
            if (source == null)
            {
                return null;
            }

            var list = new List<FeedbackViewModel>(source.Count);
            foreach (var item in source)
            {
                var target = FeedbackViewMapping(item);
                if (target != null)
                {
                    list.Add(target);
                }
            }

            return list;
        }

        #endregion

        #region Comment

        public CommentEntity CommentEntityMapping(CommentEntity source, CommentEntity target)
        {
            var result = Mapper.Map(source, target);

            return CommentEntityCheck(result);
        }

        /// <summary>
        /// 检查 entity 为 null的情况
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private CommentEntity CommentEntityCheck(CommentEntity source)
        {
            source.Content = source.Content ?? String.Empty;

            return source;
        }

        public CommentEntity CommentEntityMapping(CommentViewModel source)
        {
            if (source == null)
            {
                return null;
            }

            var target = Mapper.Map<CommentViewModel, CommentEntity>(source);
            target.CreatedDate = DateTime.Now;
            target.UpdatedDate = DateTime.Now;

            return CommentEntityCheck(target);
        }

        public CommentViewModel CommentViewMapping(CommentEntity source)
        {
            if (source == null)
            {
                return null;
            }

            var target = Mapper.Map<CommentEntity, CommentViewModel>(source);

            return target;
        }

        public IEnumerable<CommentViewModel> CommentViewMapping(List<CommentEntity> source)
        {
            if (source == null)
            {
                return null;
            }

            var list = new List<CommentViewModel>(source.Count);
            foreach (var item in source)
            {
                var target = CommentViewMapping(item);
                if (target != null)
                {
                    list.Add(target);
                }
            }

            return list;
        }

        #endregion

        #region Promotion

        /// <summary>
        /// 检查 entity 为 null的情况
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static PromotionEntity PromotionEntityCheck(PromotionEntity source)
        {
            source.Description = CheckString(source.Description);
            source.Name = CheckString(source.Name);

            source.StartDate = EntityDateTime(source.StartDate);
            source.EndDate = EntityDateTime(source.EndDate);
            source.UpdatedDate = EntityDateTime(source.UpdatedDate);
            source.CreatedDate = EntityDateTime(source.CreatedDate);

            return source;
        }

        public PromotionEntity PromotionEntityMapping(PromotionEntity source, PromotionEntity target)
        {
            var result = Mapper.Map(source, target);

            return PromotionEntityCheck(result);
        }

        public PromotionEntity PromotionEntityMapping(PromotionViewModel source)
        {
            if (source == null)
            {
                return null;
            }

            var target = Mapper.Map<PromotionViewModel, PromotionEntity>(source);
            target.CreatedDate = DateTime.Now;
            target.UpdatedDate = DateTime.Now;

            return PromotionEntityCheck(target);
        }

        public PromotionViewModel PromotionViewMapping(PromotionEntity source)
        {
            if (source == null)
            {
                return null;
            }

            var resouces = ResourceViewMapping(GetListResourceEntities(SourceType.Promotion, source.Id)).ToList();

            return PromotionViewMapping(source, resouces);
        }

        private static PromotionViewModel PromotionViewMapping(PromotionEntity source, List<ResourceViewModel> resourceViewModels)
        {
            if (source == null)
            {
                return null;
            }

            var target = Mapper.Map<PromotionEntity, PromotionViewModel>(source);
            target.Resources = resourceViewModels;

            return target;
        }

        public IEnumerable<PromotionViewModel> PromotionViewMapping(List<PromotionEntity> source)
        {
            if (source == null)
            {
                return null;
            }

            var list = new List<PromotionViewModel>(source.Count);
            var ids = source.Select(v => v.Id).ToList();

            var resoucres = ResourceViewMapping(GetListResourceEntities(SourceType.Promotion, ids)).ToList();

            foreach (var item in source)
            {
                var r = resoucres.Where(v => v.SourceId == item.Id).ToList();

                var target = PromotionViewMapping(item, r);
                if (target != null)
                {
                    list.Add(target);
                }
            }

            return list;
        }

        #endregion

        #region Store

        /// <summary>
        /// 检查 entity 为 null的情况
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private StoreEntity StoreEntityCheck(StoreEntity source)
        {
            source.Description = source.Description ?? String.Empty;
            source.Name = source.Name ?? String.Empty;

            return source;
        }

        public StoreEntity StoreEntityMapping(StoreEntity source, StoreEntity target)
        {
            var result = Mapper.Map(source, target);

            return StoreEntityCheck(result);
        }

        public StoreEntity StoreEntityMapping(StoreViewModel source)
        {
            if (source == null)
            {
                return null;
            }

            var target = Mapper.Map<StoreViewModel, StoreEntity>(source);
            target.CreatedDate = DateTime.Now;
            target.UpdatedDate = DateTime.Now;

            return StoreEntityCheck(target);
        }

        public StoreViewModel StoreViewMapping(StoreEntity source)
        {
            if (source == null)
            {
                return null;
            }

            var target = Mapper.Map<StoreEntity, StoreViewModel>(source);

            return target;
        }

        public IEnumerable<StoreViewModel> StoreViewMapping(List<StoreEntity> source)
        {
            if (source == null)
            {
                return null;
            }

            var list = new List<StoreViewModel>(source.Count);
            foreach (var item in source)
            {
                var target = StoreViewMapping(item);
                if (target != null)
                {
                    list.Add(target);
                }
            }

            return list;
        }

        #endregion

        #region tag

        /// <summary>
        /// 检查 entity 为 null的情况
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private TagEntity TagEntityCheck(TagEntity source)
        {
            source.Description = source.Description ?? String.Empty;
            source.Name = source.Name ?? String.Empty;

            return source;
        }

        public TagEntity TagEntityMapping(TagEntity source, TagEntity target)
        {
            var result = Mapper.Map(source, target);

            return TagEntityCheck(result);
        }

        public TagEntity TagEntityMapping(TagViewModel source)
        {
            if (source == null)
            {
                return null;
            }

            var target = Mapper.Map<TagViewModel, TagEntity>(source);
            target.CreatedDate = DateTime.Now;
            target.UpdatedDate = DateTime.Now;

            return TagEntityCheck(target);
        }

        public TagViewModel TagViewMapping(TagEntity source)
        {
            if (source == null)
            {
                return null;
            }

            var target = Mapper.Map<TagEntity, TagViewModel>(source);

            return target;
        }

        public IEnumerable<TagViewModel> TagViewMapping(List<TagEntity> source)
        {
            if (source == null)
            {
                return null;
            }

            var list = new List<TagViewModel>(source.Count);
            foreach (var item in source)
            {
                var target = TagViewMapping(item);
                if (target != null)
                {
                    list.Add(target);
                }
            }

            return list;
        }

        #endregion

        #region brand

        public BrandEntity BrandEntityMapping(BrandEntity source, BrandEntity target)
        {
            var result = Mapper.Map(source, target);

            return BrandEntityCheck(result);
        }

        /// <summary>
        /// 检查 entity 为 null的情况
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private BrandEntity BrandEntityCheck(BrandEntity source)
        {
            source.Description = source.Description ?? String.Empty;
            source.Name = source.Name ?? String.Empty;
            source.EnglishName = source.EnglishName ?? String.Empty;
            source.Logo = source.Logo ?? String.Empty;
            source.WebSite = source.WebSite ?? String.Empty;

            source.CreatedDate = EntityDateTime(source.CreatedDate);
            source.UpdatedDate = EntityDateTime(source.UpdatedDate);


            return source;
        }

        public BrandEntity BrandEntityMapping(BrandViewModel source)
        {
            if (source == null)
            {
                return null;
            }

            var target = Mapper.Map<BrandViewModel, BrandEntity>(source);
            source.CreatedDate = EntityDateTime(source.CreatedDate);
            source.UpdatedDate = EntityDateTime(source.UpdatedDate);

            return BrandEntityCheck(target);
        }

        public BrandViewModel BrandViewMapping(BrandEntity source)
        {
            if (source == null)
            {
                return null;
            }

            var target = Mapper.Map<BrandEntity, BrandViewModel>(source);

            if (!String.IsNullOrWhiteSpace(target.Logo))
            {
                if (!target.Logo.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
                {
                    target.Logo = ConfigManager.GetHttpApiImagePath() + target.Logo;
                }
            }

            return target;
        }

        public IEnumerable<BrandViewModel> BrandViewMapping(List<BrandEntity> source)
        {
            if (source == null)
            {
                return null;
            }

            var list = new List<BrandViewModel>(source.Count);
            foreach (var item in source)
            {
                var target = BrandViewMapping(item);
                if (target != null)
                {
                    list.Add(target);
                }
            }

            return list;
        }

        #endregion

        #region product

        /// <summary>
        /// 检查 entity 为 null的情况
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static ProductEntity ProductEntityCheck(ProductEntity source)
        {
            source.Favorable = CheckString(source.Favorable);// source.Favorable ?? String.Empty;
            source.RecommendedReason = CheckString(source.RecommendedReason);
            source.Description = CheckString(source.Description);
            source.Name = CheckString(source.Name);

            source.CreatedDate = EntityDateTime(source.CreatedDate);
            source.UpdatedDate = EntityDateTime(source.UpdatedDate);

            return source;
        }

        public ProductEntity ProductEntityMapping(ProductEntity source, ProductEntity target)
        {
            var result = Mapper.Map(source, target);

            return ProductEntityCheck(result);
        }

        public ProductEntity ProductViewMapping(ProductViewModel source)
        {
            if (source == null)
            {
                return null;
            }

            var target = Mapper.Map<ProductViewModel, ProductEntity>(source);
            target.CreatedDate = EntityDateTime(source.CreatedDate);
            target.UpdatedDate = EntityDateTime(source.UpdatedDate);

            return ProductEntityCheck(target);
        }

        public IEnumerable<ProductViewModel> ProductViewMapping(List<ProductEntity> source)
        {
            if (source == null || source.Count == 0)
            {
                return new List<ProductViewModel>(0);
            }

            var list = new List<ProductViewModel>(source.Count);
            var ids = source.Select(v => v.Id).ToList();

            var resoucres = ResourceViewMapping(GetListResourceEntities(SourceType.Product, ids)).ToList();
            var ppr = GetPromotionForRelation(ids);
            var ptr = GetTopicRelationByProduct4Entities(ids);

            foreach (var item in source)
            {
                var r = resoucres.Where(v => v.SourceId == item.Id).ToList();
                var pp = ppr.Where(v => (v.ProdId ?? 0) == item.Id).Select(v => v.ProId ?? 0).Distinct().ToList();
                var pt = ptr.Where(v => v.Product_Id == item.Id).Select(v => v.SpecialTopic_Id).Distinct().ToList();

                var target = ProductViewMapping(item, r, pt, pp);
                if (target != null)
                {
                    list.Add(target);
                }
            }

            return list;
        }

        private static ProductViewModel ProductViewMapping(ProductEntity source, List<ResourceViewModel> resourceViewModels, IReadOnlyCollection<int> topicIds = null, IReadOnlyCollection<int> promotionIds = null)
        {
            if (source == null)
            {
                return null;
            }

            var target = Mapper.Map<ProductEntity, ProductViewModel>(source);
            target.Resources = resourceViewModels;

            if (topicIds != null && topicIds.Count > 0)
            {
                var t = String.Join(",", topicIds);
                target.TopicIds = t;
            }

            if (promotionIds != null && promotionIds.Count > 0)
            {
                var t = String.Join(",", promotionIds);
                target.PromotionIds = t;
            }

            return target;
        }

        public ProductViewModel ProductViewMapping(ProductEntity source)
        {
            if (source == null)
            {
                return null;
            }

            var resouces = ResourceViewMapping(GetListResourceEntities(SourceType.Product, source.Id)).ToList();
            var t = ServiceLocator.Current.Resolve<ISpecialTopicProductRelationRepository>().GetListByProduct(source.Id);
            var tids = t.Select(v => v.SpecialTopic_Id).Distinct().ToList();
            var p = ServiceLocator.Current.Resolve<IPromotionProductRelationRepository>().GetList4Product(new List<int> { source.Id });
            var pids = p.Select(v => v.ProId ?? 0).Distinct().ToList();

            return ProductViewMapping(source, resouces, tids, pids);
        }

        #endregion

        #region SpecialTopic

        public SpecialTopicEntity SpecialTopicEntityMapping(SpecialTopicEntity source, SpecialTopicEntity target)
        {
            var result = Mapper.Map(source, target);

            return SpecialTopicEntityCheck(result);
        }

        /// <summary>
        /// 检查 entity 为 null的情况
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private SpecialTopicEntity SpecialTopicEntityCheck(SpecialTopicEntity source)
        {
            source.Description = source.Description ?? String.Empty;
            source.Name = source.Name ?? String.Empty;
            source.CreatedDate = EntityDateTime(source.CreatedDate);
            source.UpdatedDate = EntityDateTime(source.UpdatedDate);

            return source;
        }

        public SpecialTopicEntity SpecialTopicEntityMapping(SpecialTopicViewModel source)
        {
            if (source == null)
            {
                return null;
            }

            var target = Mapper.Map<SpecialTopicViewModel, SpecialTopicEntity>(source);

            target.CreatedDate = EntityDateTime(source.CreatedDate);
            target.UpdatedDate = EntityDateTime(source.UpdatedDate);

            return SpecialTopicEntityCheck(target);
        }

        public SpecialTopicViewModel SpecialTopicViewMapping(SpecialTopicEntity source)
        {
            if (source == null)
            {
                return null;
            }

            var resouces = ResourceViewMapping(GetListResourceEntities(SourceType.SpecialTopic, source.Id)).ToList();

            return SpecialTopicViewMapping(source, resouces);
        }

        private SpecialTopicViewModel SpecialTopicViewMapping(SpecialTopicEntity source,
                                                              List<ResourceViewModel> resourceViewModels)
        {
            if (source == null)
            {
                return null;
            }

            var target = Mapper.Map<SpecialTopicEntity, SpecialTopicViewModel>(source);
            target.Resources = resourceViewModels;

            return target;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public IEnumerable<SpecialTopicViewModel> SpecialTopicViewMapping(List<SpecialTopicEntity> source)
        {
            if (source == null)
            {
                return null;
            }

            var list = new List<SpecialTopicViewModel>(source.Count);
            var ids = source.Select(v => v.Id).ToList();
            var resoucres = ResourceViewMapping(GetListResourceEntities(SourceType.SpecialTopic, ids)).ToList();

            foreach (var item in source)
            {
                if (item == null)
                {
                    continue;
                }

                var r = resoucres.Where(v => v.SourceId == item.Id).ToList();

                var target = SpecialTopicViewMapping(item, r);
                if (target != null)
                {
                    list.Add(target);
                }
            }

            return list;
        }

        #endregion
    }
}
