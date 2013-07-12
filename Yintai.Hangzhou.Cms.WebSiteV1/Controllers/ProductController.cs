using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;
using Yintai.Architecture.Common.Models;
using Yintai.Architecture.Framework.ServiceLocation;
using Yintai.Hangzhou.Cms.WebSiteV1.Dto.Product;
using Yintai.Hangzhou.Cms.WebSiteV1.Models;
using Yintai.Hangzhou.Cms.WebSiteV1.Util;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Model.Filters;
using Yintai.Hangzhou.Repository.Contract;
using Yintai.Hangzhou.Service.Contract;
using Yintai.Hangzhou.WebSupport.Binder;
using MappingManager = Yintai.Hangzhou.Cms.WebSiteV1.Manager.MappingManager;

namespace Yintai.Hangzhou.Cms.WebSiteV1.Controllers
{
    [AdminAuthorize]
    public class ProductController : UserController
    {
        private readonly IProductRepository _productRepository;
        private readonly ISpecialTopicProductRelationRepository _stprRepository;
        private readonly IPromotionProductRelationRepository _pprRepository;
        private IStoreRepository _storeRepository;
        private IResourceService _resourceService;
        private IUserAuthRepository _userAuthRepo;
        private IProductCode2StoreCodeRepository _productcodemapRepo;

        public ProductController(IProductRepository productRepository
            , ISpecialTopicProductRelationRepository specialTopicProductRelationRepository
            , IPromotionProductRelationRepository promotionProductRelationRepository
            , IStoreRepository storeRepository
            , IResourceService resourceService
            , IUserAuthRepository userAuthRepo
            , IProductCode2StoreCodeRepository productcodemapRepo)
        {
            _productRepository = productRepository;
            _stprRepository = specialTopicProductRelationRepository;
            _pprRepository = promotionProductRelationRepository;
            _storeRepository = storeRepository;
            _resourceService = resourceService;
            _userAuthRepo = userAuthRepo;
            _productcodemapRepo = productcodemapRepo;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List(ProductSearchOption search, PagerRequest request)
        {
            int totalCount;
            search.CurrentUser = CurrentUser.CustomerId;
            search.CurrentUserRole = CurrentUser.Role;
            var dbContext = _productRepository.Context;
            var linq = dbContext.Set<ProductEntity>().Where(p => (!search.PId.HasValue || p.Id == search.PId.Value) &&
                (string.IsNullOrEmpty(search.Name) || p.Name.StartsWith(search.Name)) &&
                (!search.User.HasValue || p.CreatedUser == search.User.Value) &&
                (!search.Status.HasValue || p.Status == (int)search.Status.Value) &&
                p.Status != (int)DataStatus.Deleted);
            linq = _userAuthRepo.AuthFilter(linq, search.CurrentUser, search.CurrentUserRole) as IQueryable<ProductEntity>;
            if (!string.IsNullOrEmpty(search.Topic) &&
                 search.Topic.Trim().Length > 0)
            { 
                linq = linq.Where(p=>(from s in dbContext.Set<SpecialTopicEntity>()
                                     join ps in dbContext.Set<SpecialTopicProductRelationEntity>() on s.Id equals ps.SpecialTopic_Id 
                                     where s.Name.StartsWith(search.Topic) && ps.Product_Id == p.Id
                                      select s).Any());
            }
            if (!string.IsNullOrEmpty(search.Promotion) &&
                search.Promotion.Trim().Length > 0)
            {
                linq = linq.Where(p=>(from pr in dbContext.Set<PromotionEntity>()
                                     join ps in dbContext.Set<Promotion2ProductEntity>() on pr.Id equals ps.ProId
                                     where pr.Name.StartsWith(search.Promotion) && ps.ProdId == p.Id
                                      select pr).Any());

            }
            var linq2 = linq.Join(dbContext.Set<StoreEntity>().Where(s=>string.IsNullOrEmpty(search.Store) || s.Name.StartsWith(search.Store)), o => o.Store_Id, i => i.Id, (o, i) => new { P = o, S = i })
               .Join(dbContext.Set<BrandEntity>().Where(b=>string.IsNullOrEmpty(search.Brand) || b.Name.StartsWith(search.Brand)), o => o.P.Brand_Id, i => i.Id, (o, i) => new { P = o.P, S = o.S, B = i })
               .Join(dbContext.Set<UserEntity>(), o => o.P.CreatedUser, i => i.Id, (o, i) => new { P = o.P, S = o.S, B = o.B, C = i })
               .Join(dbContext.Set<TagEntity>().Where(t=>string.IsNullOrEmpty(search.Tag) || t.Name.StartsWith(search.Tag)), o => o.P.Tag_Id, i => i.Id, (o, i) => new { P = o.P, S = o.S, B = o.B, C = o.C, T = i })
               .GroupJoin(dbContext.Set<PromotionEntity>().Join(_pprRepository.GetAll(),
                                                       o => o.Id,
                                                       i => i.ProId,
                                                       (o, i) => new { Pro = o, ProR = i }),
                           o => o.P.Id,
                           i => i.ProR.ProdId,
                           (o, i) => new { P = o.P, S = o.S, B = o.B, C = o.C, T = o.T, Pro = i })
                .GroupJoin(dbContext.Set<SpecialTopicEntity>().Join(_stprRepository.GetAll(), o => o.Id, i => i.SpecialTopic_Id, (o, i) => new { Spe = o, SpeR = i }),
                           o => o.P.Id,
                           i => i.SpeR.Product_Id,
                           (o, i) => new { P = o.P, S = o.S, B = o.B, C = o.C, T = o.T, Pro = o.Pro, Spe = i })
                .GroupJoin(dbContext.Set<ResourceEntity>().Where(r => r.SourceType == (int)SourceType.Product), o => o.P.Id, i => i.SourceId
                           , (o, i) => new { P = o.P, S = o.S, B = o.B, C = o.C, T = o.T, Pro = o.Pro, Spe = o.Spe, R = i });


          if (!search.OrderBy.HasValue)
              linq2 =linq2.OrderByDescending(l=>l.P.CreatedDate);
          else
          {
            switch(search.OrderBy.Value)
            {
                case ProductSortOrder.CreatedUserDesc:
                    linq2 = linq2.OrderByDescending(l=>l.C.Nickname).ThenByDescending(l=>l.P.CreatedDate);
                    break;
                case ProductSortOrder.SortOrderDesc:
                   linq2= linq2.OrderByDescending(l=>l.P.SortOrder).ThenByDescending(l=>l.P.CreatedDate);
                    break;
                case ProductSortOrder.SortByBrand:
                    linq2=linq2.OrderByDescending(l=>l.B.Name).ThenByDescending(l=>l.P.CreatedDate);
                    break;
                case ProductSortOrder.CreatedDateDesc:
                    linq2 = linq2.OrderByDescending(l=>l.P.CreatedDate);
                    break;


            }
          }           
           totalCount = linq2.Count();

            var skipCount = (request.PageIndex - 1) * request.PageSize;

            linq2 = skipCount == 0 ? linq2.Take(request.PageSize) : linq2.Skip(skipCount).Take(request.PageSize);
            

            var vo = from l in linq2.ToList()
                     select new ProductViewModel().FromEntity<ProductViewModel>(l.P,p=>{
                         p.StoreName = l.S.Name;
                         p.TagName = l.T.Name;
                         p.BrandName = l.B.Name;
                         p.CreateUserName = l.C.Nickname;
                         p.PromotionName = from pro in l.Pro
                                             select pro.Pro.Name;
                         p.PromotionIds = string.Join(",", (from pro in l.Pro
                                                              select pro.Pro.Id.ToString()).ToArray());
                         p.TopicName = from top in l.Spe
                                         select top.Spe.Name;
                             p.TopicIds = string.Join(",", (from top in l.Spe
                                                          select top.Spe.Id.ToString()).ToArray());
                          p.Resources = l.R.Select(r=>new ResourceViewModel().FromEntity<ResourceViewModel>(r));
                     });

            var v = new ProductCollectionViewModel(request, totalCount) { Products = vo.ToList() };
            ViewBag.SearchOptions = search;
            return View(v);
        }

        public ActionResult Details(int? id, [FetchProduct(KeyName = "id")]ProductEntity entity)
        {
            if (id == null || entity == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View();
            }

            var vo = MappingManager.ProductViewMapping(entity);
            

            return View(vo);
        }

        public ActionResult Create()
        {
            return View();
        }


        public ActionResult Edit(int? id, [FetchProduct(KeyName = "id")]ProductEntity entity)
        {
            if (id == null || entity == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View();
            }

            var vo = MappingManager.ProductViewMapping(entity);

            return View(vo);
        }

        [HttpPost]
        public ActionResult Create(FormCollection formCollection, ProductViewModel vo)
        {
            if (ModelState.IsValid)
            {

                var entity = MappingManager.ProductViewMapping(vo);
                entity.CreatedUser = CurrentUser.CustomerId;
                entity.UpdatedUser = CurrentUser.CustomerId;
                entity.CreatedDate = DateTime.Now;
                entity.RecommendedReason = entity.RecommendedReason ?? string.Empty;
                entity.RecommendUser = CurrentUser.CustomerId;
                entity.RecommendSourceId = entity.RecommendUser;
                entity.RecommendSourceType = (int)RecommendSourceType.Default;
                entity.UpdatedDate = DateTime.Now;
                entity.Status = (int)DataStatus.Default;
                using (var ts = new TransactionScope())
                {
                    entity = _productRepository.Insert(entity);
                    _productcodemapRepo.Insert(new ProductCode2StoreCodeEntity()
                    {
                        ProductId = entity.Id,
                        Status = (int)DataStatus.Normal,
                        StoreId = entity.Store_Id,
                        StoreProductCode = vo.UPCCode,
                        UpdateDate = DateTime.Now,
                        UpdateUser = CurrentUser.CustomerId
                    });
                    SaveT(entity.Id, StringsToInts(vo.TopicIds, ","));
                    SaveP(entity.Id, StringsToInts(vo.PromotionIds, ","));
                    _resourceService.Save(ControllerContext.HttpContext.Request.Files
                       , CurrentUser.CustomerId
                       , -1, entity.Id
                       , SourceType.Product);
                    ts.Complete();
                }

                return RedirectToAction("Edit", new { id = @entity.Id });
            }

            return View(vo);
        }

        private void SaveT(int pId, IEnumerable<int> topicList)
        {
            if (topicList == null)
            {
                _stprRepository.DeleteByProductId(pId);

                return;
            }

            var r = _stprRepository.GetListByProduct(pId);
            if (r != null && r.Count > 0)
            {
                foreach (var r1 in r)
                {
                    r1.Status = (int)DataStatus.Deleted;
                    r1.UpdatedDate = DateTime.Now;
                    r1.UpdatedUser = CurrentUser.CustomerId;

                    _stprRepository.Delete(r1);
                }
            }

            foreach (var i in topicList)
            {
                _stprRepository.Insert(new SpecialTopicProductRelationEntity
                {
                    CreatedDate = DateTime.Now,
                    CreatedUser = CurrentUser.CustomerId,
                    Product_Id = pId,
                    SpecialTopic_Id = i,
                    Status = (int)DataStatus.Normal,
                    UpdatedDate = DateTime.Now,
                    UpdatedUser = CurrentUser.CustomerId
                });
            }
        }

        private void SaveP(int pId, IEnumerable<int> promotionList)
        {
            //TODO:BUG
            if (promotionList == null)
            {
                //
                _pprRepository.DeletedByProduct(pId);

                return;
            }

            var r = _pprRepository.GetList4Product(new List<int> { pId });
            if (r != null && r.Count > 0)
            {
                foreach (var r1 in r)
                {
                    r1.Status = (int)DataStatus.Deleted;
                    _pprRepository.Delete(r1);
                }
            }

            var p = ServiceLocator.Current.Resolve<IPromotionRepository>();
            foreach (var i in promotionList)
            {
                _pprRepository.Insert(new Promotion2ProductEntity
                {
                    Status = (int)DataStatus.Normal,
                    ProdId = pId,
                    ProId = i
                });

                //true
                p.SetIsProd(i, true);
            }
        }

        [HttpPost]
        public ActionResult Edit(FormCollection formCollection, [FetchProduct(KeyName = "id")]ProductEntity entity, ProductViewModel vo)
        {
            if (entity == null || !ModelState.IsValid)
            {
                ModelState.AddModelError(String.Empty, "参数验证失败.");
                return View(vo);
            }

            entity.UpdatedDate = DateTime.Now;
            entity.UpdatedUser = CurrentUser.CustomerId;
            entity.Name = vo.Name;
            entity.Price = vo.Price;
            entity.RecommendedReason = vo.RecommendedReason ?? string.Empty;
            entity.SortOrder = vo.SortOrder;
            entity.Price = vo.Price;
            entity.Status = vo.Status;
            entity.Store_Id = vo.Store_Id;
            entity.Tag_Id = vo.Tag_Id;
            entity.Brand_Id = vo.Brand_Id;
            entity.Description = vo.Description;
            entity.Is4Sale = vo.Is4Sale;
            entity.UnitPrice = vo.UnitPrice;

            using (var ts = new TransactionScope())
            {
                _productRepository.Update(entity);
                //persist product code map 2 store code
                var codemapEntity = _productcodemapRepo.Get(p => p.StoreId == entity.Store_Id && p.ProductId == entity.Id && p.Status != (int)DataStatus.Deleted)
                                    .FirstOrDefault();
                if (string.IsNullOrEmpty(vo.UPCCode))
                {
                    if (codemapEntity != null)
                    {
                        codemapEntity.Status = (int)DataStatus.Deleted;
                        codemapEntity.UpdateUser = CurrentUser.CustomerId;
                        codemapEntity.UpdateDate = DateTime.Now;
                        _productcodemapRepo.Update(codemapEntity);
                    }
                }
                else
                {
                    if (codemapEntity != null)
                    {
                        codemapEntity.StoreProductCode = vo.UPCCode;
                        codemapEntity.UpdateUser = CurrentUser.CustomerId;
                        codemapEntity.UpdateDate = DateTime.Now;
                        _productcodemapRepo.Update(codemapEntity);
                    }
                    else
                    {
                        _productcodemapRepo.Insert(new ProductCode2StoreCodeEntity()
                        {
                            ProductId = entity.Id,
                            Status = (int)DataStatus.Normal,
                            StoreId = entity.Store_Id,
                            StoreProductCode = vo.UPCCode,
                            UpdateDate = DateTime.Now,
                            UpdateUser = CurrentUser.CustomerId
                        });
                    }
                }
                

                SaveT(entity.Id, StringsToInts(vo.TopicIds, ","));
                SaveP(entity.Id, StringsToInts(vo.PromotionIds, ","));
                if (ControllerContext.HttpContext.Request.Files.Count > 0)
                {
                    this._resourceService.Save(ControllerContext.HttpContext.Request.Files
                          , CurrentUser.CustomerId
                        , -1, entity.Id
                        , SourceType.Product);
                }

                ts.Complete();
            }

            return RedirectToAction2(() =>
            {
                return RedirectToAction("Details", new { id = vo.Id });
            });
        }

        private IEnumerable<int> StringsToInts(string str, string spit)
        {
            if (String.IsNullOrWhiteSpace(str))
            {
                return new List<int>(0);
            }

            var poo = str.Split(new[] { spit }, StringSplitOptions.RemoveEmptyEntries);
            var pi = new List<int>(poo.Length);
            foreach (var s in poo)
            {
                pi.Add(Int32.Parse(s));
            }

            return pi;
        }
        public PartialViewResult Select(ProductSearchOption search, PagerRequest request)
        {
            int totalCount;
            search.CurrentUser = CurrentUser.CustomerId;
            search.CurrentUserRole = CurrentUser.Role;
            var dbContext = _productRepository.Context;
            var linq = dbContext.Set<ProductEntity>().Where(p => (!search.PId.HasValue || p.Id == search.PId.Value) &&
                (string.IsNullOrEmpty(search.Name) || p.Name.StartsWith(search.Name)) &&
                (!search.User.HasValue || p.CreatedUser == search.User.Value) &&
                (!search.Status.HasValue || p.Status == (int)search.Status.Value) &&
                p.Status != (int)DataStatus.Deleted);
            linq = _userAuthRepo.AuthFilter(linq, search.CurrentUser, search.CurrentUserRole) as IQueryable<ProductEntity>;
            if (!string.IsNullOrEmpty(search.Topic) &&
                 search.Topic.Trim().Length > 0)
            {
                linq = linq.Where(p => (from s in dbContext.Set<SpecialTopicEntity>()
                                        join ps in dbContext.Set<SpecialTopicProductRelationEntity>() on s.Id equals ps.SpecialTopic_Id
                                        where s.Name.StartsWith(search.Topic) && ps.Product_Id == p.Id
                                        select s).Any());
            }
            if (!string.IsNullOrEmpty(search.Promotion) &&
                search.Promotion.Trim().Length > 0)
            {
                linq = linq.Where(p => (from pr in dbContext.Set<PromotionEntity>()
                                        join ps in dbContext.Set<Promotion2ProductEntity>() on pr.Id equals ps.ProId
                                        where pr.Name.StartsWith(search.Promotion) && ps.ProdId == p.Id
                                        select pr).Any());

            }
            var linq2 = linq.Join(dbContext.Set<StoreEntity>().Where(s => string.IsNullOrEmpty(search.Store) || s.Name.StartsWith(search.Store)), o => o.Store_Id, i => i.Id, (o, i) => new { P = o, S = i })
               .Join(dbContext.Set<BrandEntity>().Where(b => string.IsNullOrEmpty(search.Brand) || b.Name.StartsWith(search.Brand)), o => o.P.Brand_Id, i => i.Id, (o, i) => new { P = o.P, S = o.S, B = i })
               .Join(dbContext.Set<UserEntity>(), o => o.P.CreatedUser, i => i.Id, (o, i) => new { P = o.P, S = o.S, B = o.B, C = i })
               .Join(dbContext.Set<TagEntity>().Where(t => string.IsNullOrEmpty(search.Tag) || t.Name.StartsWith(search.Tag)), o => o.P.Tag_Id, i => i.Id, (o, i) => new { P = o.P, S = o.S, B = o.B, C = o.C, T = i })
                .GroupJoin(dbContext.Set<ResourceEntity>().Where(r => r.SourceType == (int)SourceType.Product), o => o.P.Id, i => i.SourceId
                           , (o, i) => new { P = o.P, S = o.S, B = o.B, C = o.C, T = o.T, R = i });


            linq2 = linq2.OrderByDescending(l => l.P.CreatedDate);
           
            totalCount = linq2.Count();

            var skipCount = (request.PageIndex - 1) * request.PageSize;

            linq2 = skipCount == 0 ? linq2.Take(request.PageSize) : linq2.Skip(skipCount).Take(request.PageSize);


            var vo = from l in linq2.ToList()
                     select new ProductViewModel().FromEntity<ProductViewModel>(l.P, p =>
                     {
                         p.StoreName = l.S.Name;
                         p.TagName = l.T.Name;
                         p.BrandName = l.B.Name;
                         p.CreateUserName = l.C.Nickname;
                        
                         p.Resources = l.R.Select(r => new ResourceViewModel().FromEntity<ResourceViewModel>(r));
                     });

           

            var v = new ProductCollectionViewModel(request, totalCount) { Products = vo.ToList() };
            ViewBag.SearchOptions = search;
            return PartialView("_Selector", v);
        }
        [HttpPost]
        [UserAuthData]
        public JsonResult Delete([FetchProduct(KeyName = "id")]ProductEntity entity)
        {
            try
            {

                entity.UpdatedDate = DateTime.Now;
                entity.UpdatedUser = CurrentUser.CustomerId;
                entity.Status = (int)DataStatus.Deleted;

                _productRepository.Update(entity);
                return SuccessResponse();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return FailResponse();
            }

        }



    }
}
