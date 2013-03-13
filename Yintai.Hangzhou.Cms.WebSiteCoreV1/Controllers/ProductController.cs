using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;
using Yintai.Architecture.Common.Models;
using Yintai.Architecture.Framework.ServiceLocation;
using Yintai.Hangzhou.Cms.WebSiteCoreV1.Dto.Product;
using Yintai.Hangzhou.Cms.WebSiteCoreV1.Models;
using Yintai.Hangzhou.Cms.WebSiteCoreV1.Util;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Model.Filters;
using Yintai.Hangzhou.Repository.Contract;
using Yintai.Hangzhou.Service.Contract;
using Yintai.Hangzhou.WebSupport.Binder;
using MappingManager = Yintai.Hangzhou.Cms.WebSiteCoreV1.Manager.MappingManager;

namespace Yintai.Hangzhou.Cms.WebSiteCoreV1.Controllers
{
    [AdminAuthorize]
    public class ProductController : UserController
    {
        private readonly IProductRepository _productRepository;
        private readonly ISpecialTopicProductRelationRepository _stprRepository;
        private readonly IPromotionProductRelationRepository _pprRepository;
        private IStoreRepository _storeRepository;
        private IResourceService _resourceService;

        public ProductController(IProductRepository productRepository
            , ISpecialTopicProductRelationRepository specialTopicProductRelationRepository
            , IPromotionProductRelationRepository promotionProductRelationRepository
            ,IStoreRepository storeRepository
            ,IResourceService resourceService)
        {
            _productRepository = productRepository;
            _stprRepository = specialTopicProductRelationRepository;
            _pprRepository = promotionProductRelationRepository;
            _storeRepository = storeRepository;
            _resourceService = resourceService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List(ProductSearchOptionViewModel search, PagerRequest request)
        {
            int totalCount;
            IQueryable<ProductEntity> data = _productRepository.Search(
                request.PageIndex
                , request.PageSize
                , out totalCount
                , MappingManager.MapCommon<ProductSearchOptionViewModel,ProductSearchOption>(search)
                );
           

            var vo = MappingManager.ProductViewMapping(data);

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
                entity.RecommendUser = CurrentUser.CustomerId;
                entity.RecommendSourceId = entity.RecommendUser;
                entity.RecommendSourceType = (int)RecommendSourceType.Default;
                entity.UpdatedDate = DateTime.Now;
                entity.Status = (int)DataStatus.Default;
                using (var ts = new TransactionScope())
                {
                    entity = _productRepository.Insert(entity);
                    SaveT(entity.Id, StringsToInts(vo.TopicIds, ","));
                    SaveP(entity.Id, StringsToInts(vo.PromotionIds, ","));
                    _resourceService.Save(ControllerContext.HttpContext.Request.Files
                       , CurrentUser.CustomerId
                       , -1, entity.Id
                       , SourceType.Product);
                    ts.Complete();
                }

                return RedirectToAction("Edit", new {id=@entity.Id });
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
            entity.RecommendedReason = vo.RecommendedReason;
            entity.SortOrder = vo.SortOrder;
            entity.Price = vo.Price;
            entity.Status = vo.Status;
            entity.Store_Id = vo.Store_Id;
            entity.Tag_Id = vo.Tag_Id;
            entity.Brand_Id = vo.Brand_Id;

            using (var ts = new TransactionScope())
            {
                _productRepository.Update(entity);
                //关联关系

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

            return RedirectToAction("Details",new {id=vo.Id});
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
        [HttpPost]
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
