using System;
using System.Collections.Generic;
using Yintai.Architecture.Common.Models;
using Yintai.Architecture.Framework.ServiceLocation;
using Yintai.Hangzhou.Cms.WebSiteCoreV1.Models;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Repository.Contract;
using Yintai.Hangzhou.Service.Manager;
using System.Linq;
using MM = Yintai.Hangzhou.Service.Manager.MappingManagerV2;
using com.intime.fashion.common;
using Yintai.Architecture.Framework;

namespace Yintai.Hangzhou.Cms.WebSiteCoreV1.Manager
{
    public class MappingManager : BaseMappingManager
    {
        private readonly MM _m;

        public MappingManager()
        {
            _m = new MM();
        }

        #region Common
        public static T MapCommon<T>(T source)
        {
           return Mapper.Map<T, T>(source);
        }
        public static TT MapCommon<T,TT>(T source)
        {
            return Mapper.Map<T, TT>(source);
        }
        private TField DefaultIfEmpty<TSource,TField>(TSource source,Func<TSource,TField> expression)
        {
            if (source!=null)
               return expression(source);
           return default(TField);
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

            return UserModelCheck(target);
        }

        public CustomerViewModel CustomerViewMapping(UserModel source)
        {
            if (source == null)
            {
                return null;
            }

            var target = Mapper.Map<UserModel, CustomerViewModel>(source);

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
            target.CommentUser = MapCommon<UserEntity, CustomerViewModel>(_customerRepository.GetItem(source.User_Id));

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

        [Obsolete]
        public IEnumerable<ProductViewModel> ProductViewMapping(IEnumerable<ProductEntity> source)
        {
            //modify to use the late binding 
            if (source == null || source.Count() == 0)
            {
                return new List<ProductViewModel>(0);
            }
            var linq = source.Join(_productRepository.Context.Set<StoreEntity>(), o => o.Store_Id, i => i.Id, (o, i) => new { P = o, S = i })
                .Join(_productRepository.Context.Set<BrandEntity>(), o => o.P.Brand_Id, i => i.Id, (o, i) => new { P = o.P, S = o.S, B = i })
                .Join(_productRepository.Context.Set<UserEntity>(), o => o.P.CreatedUser, i => i.Id, (o, i) => new { P = o.P, S = o.S, B = o.B, C = i })
                .Join(_productRepository.Context.Set<TagEntity>(), o => o.P.Tag_Id, i => i.Id, (o, i) => new { P = o.P, S = o.S, B = o.B, C = o.C, T = i })
                .GroupJoin(_productRepository.Context.Set<PromotionEntity>().Join(_pprRepository.GetAll(),
                                                        o => o.Id,
                                                        i => i.ProId,
                                                        (o, i) => new { Pro = o, ProR = i }),
                            o => o.P.Id,
                            i => i.ProR.ProdId,
                            (o, i) => new { P = o.P, S = o.S, B = o.B, C = o.C, T = o.T, Pro = i })
                 .GroupJoin(_productRepository.Context.Set<SpecialTopicEntity>().Join(_stprRepository.GetAll(), o => o.Id, i => i.SpecialTopic_Id, (o, i) => new { Spe = o, SpeR = i }),
                            o => o.P.Id,
                            i => i.SpeR.Product_Id,
                            (o, i) => new { P = o.P, S = o.S, B = o.B, C = o.C, T = o.T, Pro = o.Pro, Spe = i })
                 .GroupJoin(_productRepository.Context.Set<ResourceEntity>().Where(r => r.SourceType == (int)SourceType.Product), o => o.P.Id, i => i.SourceId
                            , (o, i) => new { P = o.P, S = o.S, B = o.B, C = o.C, T = o.T, Pro = o.Pro, Spe = o.Spe,R = i });
            var result = from l in linq.ToList()
                         let p =l.P
                         let res = from r in l.R
                                   select ResourceViewMapping(r)
                        
                         select new ProductViewModel()
                         {
                             Brand_Id = p.Brand_Id
                             ,
                             BrandName = l.B.Name
                             ,
                             CreatedDate = p.CreatedDate
                             ,
                             CreatedUser = p.CreatedUser
                             ,
                             CreateUserName = l.C.Nickname
                             ,
                             Description = p.Description
                             ,
                             Favorable = p.Favorable
                             ,
                             FavoriteCount = p.FavoriteCount
                             ,
                             Id = p.Id
                             ,
                             InvolvedCount = p.InvolvedCount
                             ,
                             Name = p.Name
                             ,
                             Price = p.Price
                             ,
                             RecommendedReason = p.RecommendedReason
                             ,
                             RecommendSourceId = p.RecommendSourceId
                             ,
                             RecommendSourceType = p.RecommendSourceType
                             ,
                             RecommendUser = p.RecommendUser
                             ,
                             ShareCount = p.ShareCount
                             ,
                             SortOrder = p.SortOrder
                             ,
                             Status = p.Status
                             ,
                             Store_Id = p.Store_Id
                             ,
                             Tag_Id = p.Tag_Id
                             ,
                             StoreName = l.S.Name
                             ,
                             TagName = l.T.Name
                             ,
                             UpdatedDate = p.UpdatedDate
                             ,
                             UpdatedUser = p.UpdatedUser
                             ,
                             PromotionName = from pro in l.Pro
                                             select pro.Pro.Name
                             ,
                             PromotionIds = string.Join(",", (from pro in l.Pro
                                                              select pro.Pro.Id.ToString()).ToArray())
                             ,
                             TopicName = from top in l.Spe
                                         select top.Spe.Name
                             ,
                             TopicIds = string.Join(",", (from top in l.Spe
                                                          select top.Spe.Id.ToString()).ToArray())
                             ,
                             Resources = res
                         };
          
            return result;
        }
        public ProductViewModel ProductViewMapping(ProductEntity source)
        {
            if (source == null)
            {
                return null;
            }
            var productVM = Mapper.Map<ProductEntity, ProductViewModel>(source);
            productVM.BrandName = DefaultIfEmpty<BrandEntity,string>(_brandRepository.Find(productVM.Brand_Id),o=>o.Name);
            productVM.TagName = DefaultIfEmpty<TagEntity, string>(_tagRepository.Find(productVM.Tag_Id), o => o.Name);
            productVM.StoreName = DefaultIfEmpty<StoreEntity,string>(_storeRepository.Find(productVM.Store_Id),o=>o.Name);
            productVM.CreateUserName = DefaultIfEmpty<UserEntity,string>(_customerRepository.Find(productVM.CreatedUser),o=>o.Nickname);

            var pros = (from pro in _promotionRepository.GetAll()
                        join p2p in _pprRepository.GetAll() on pro.Id equals p2p.ProId
                        where p2p.ProdId == productVM.Id
                        select pro).ToList();
            var tops = (from st in _specialTopicRepository.GetAll()
                        join p2t in _stprRepository.GetAll() on st.Id equals p2t.SpecialTopic_Id
                        where p2t.Product_Id == productVM.Id
                        select st).ToList();
            var res = from r in
                          (from r in _resourceRepository.GetAll()
                           where r.SourceType == (int)SourceType.Product
                           && r.SourceId == productVM.Id
                           select r).ToList()
                      select ResourceViewMapping(r);
            productVM.Resources = res;
            productVM.TopicIds = string.Join(",", (from t in tops
                                                   select t.Id).ToArray());
            productVM.TopicName = from t in tops
                                  select t.Name;
            productVM.PromotionIds = string.Join(",", (from p in pros
                                                       select p.Id).ToArray());
            productVM.PromotionName = from p in pros
                                      select p.Name;


            return productVM;
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
