using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.Cms.WebSiteCoreV1.Models;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Model.Filters;
using Yintai.Hangzhou.Repository.Contract;
using Yintai.Hangzhou.WebSupport.Binder;
using Yintai.Hangzhou.WebSupport.Mvc;
using MappingManager = Yintai.Hangzhou.Cms.WebSiteCoreV1.Manager.MappingManager;

namespace Yintai.Hangzhou.Cms.WebSiteCoreV1.Controllers
{
    [AdminAuthorize]
    public class ProductController : UserController
    {
        private readonly IProductRepository _productRepository;
        private readonly ISpecialTopicProductRelationRepository _stprRepository;
        private readonly IPromotionProductRelationRepository _pprRepository;

        public ProductController(IProductRepository productRepository, ISpecialTopicProductRelationRepository specialTopicProductRelationRepository, IPromotionProductRelationRepository promotionProductRelationRepository)
        {
            _productRepository = productRepository;
            _stprRepository = specialTopicProductRelationRepository;
            _pprRepository = promotionProductRelationRepository;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List(PagerRequest request, int? sort, string name, int? recommendUser, int? tagId, int? brandId, int? topicId, int? promotionId, int? status)
        {
            int totalCount;
            var sortOrder = (ProductSortOrder)(sort ?? 0);

            List<int> tag = null;
            if (tagId != null)
            {
                tag = new List<int>(tagId.Value);
            }

            var data = _productRepository.GetPagedList(request, out totalCount, sortOrder, new ProductFilter
                {
                    BrandId = brandId,
                    DataStatus = status == null ? new DataStatus?() : (DataStatus)status,
                    ProductName = name,
                    PromotionId = promotionId,
                    RecommendUser = recommendUser,
                    TagIds = tag,
                    Timestamp = null,
                    TopicId = topicId
                });

            var vo = MappingManager.ProductViewMapping(data);

            var v = new ProductCollectionViewModel(request, totalCount) { Products = vo.ToList() };

            return View("List", v);
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

        public ActionResult Delete(int? id, [FetchProduct(KeyName = "id")]ProductEntity entity)
        {
            if (id == null || entity == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View();
            }

            var vo = MappingManager.ProductViewMapping(entity);

            return View(vo);
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
                entity.CreatedUser = base.CurrentUser.CustomerId;
                entity.UpdatedUser = base.CurrentUser.CustomerId;
                entity.Status = (int)DataStatus.Normal;
                entity.CreatedDate = DateTime.Now;
                entity.UpdatedDate = DateTime.Now;

                using (var ts = new TransactionScope())
                {
                    entity = this._productRepository.Insert(entity);
                    SaveT(entity.Id, StringsToInts(vo.TopicIds, ","));
                    SaveP(entity.Id, StringsToInts(vo.PromotionIds, ","));

                    ts.Complete();
                }

                return Success("/" + RouteData.Values["controller"] + "/edit/" + entity.Id.ToString(CultureInfo.InvariantCulture));
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
                    r1.UpdatedUser = base.CurrentUser.CustomerId;

                    _stprRepository.Delete(r1);
                }
            }

            foreach (var i in topicList)
            {
                _stprRepository.Insert(new SpecialTopicProductRelationEntity()
                {
                    CreatedDate = DateTime.Now,
                    CreatedUser = base.CurrentUser.CustomerId,
                    Product_Id = pId,
                    SpecialTopic_Id = i,
                    Status = (int)DataStatus.Normal,
                    UpdatedDate = DateTime.Now,
                    UpdatedUser = base.CurrentUser.CustomerId
                });
            }
        }

        private void SaveP(int pId, IEnumerable<int> promotionList)
        {
            if (promotionList == null)
            {
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

            foreach (var i in promotionList)
            {
                _pprRepository.Insert(new Promotion2ProductEntity()
                {
                    Status = (int)DataStatus.Normal,
                    ProdId = pId,
                    ProId = i
                });
            }
        }

        [HttpPost]
        public ActionResult Edit(FormCollection formCollection, [FetchProduct(KeyName = "id")]ProductEntity entity, ProductViewModel vo)
        {
            if (entity == null || !ModelState.IsValid)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View(vo);
            }


            var newEntity = MappingManager.ProductViewMapping(vo);
            newEntity.CreatedUser = entity.CreatedUser;
            newEntity.CreatedDate = entity.CreatedDate;
            newEntity.Status = entity.Status;
            newEntity.InvolvedCount = entity.InvolvedCount;
            newEntity.FavoriteCount = entity.FavoriteCount;
            newEntity.ShareCount = entity.ShareCount;

            newEntity.UpdatedDate = DateTime.Now;
            newEntity.UpdatedUser = base.CurrentUser.CustomerId;
            newEntity.RecommendUser = entity.RecommendUser;

            MappingManager.ProductEntityMapping(newEntity, entity);

            using (var ts = new TransactionScope())
            {
                _productRepository.Update(entity);
                //关联关系

                SaveT(entity.Id, StringsToInts(vo.TopicIds, ","));
                SaveP(entity.Id, StringsToInts(vo.PromotionIds, ","));

                ts.Complete();
            }

            return Success("/" + RouteData.Values["controller"] + "/details/" + entity.Id.ToString(CultureInfo.InvariantCulture));
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
        public ActionResult Delete(FormCollection formCollection, [FetchProduct(KeyName = "id")]ProductEntity entity)
        {
            if (entity == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View();
            }

            entity.UpdatedDate = DateTime.Now;
            entity.UpdatedUser = CurrentUser.CustomerId;
            entity.Status = (int)DataStatus.Deleted;

            this._productRepository.Delete(entity);


            return Success("/" + RouteData.Values["controller"] + "/" + RouteData.Values["action"]);
        }

        [HttpPost]
        public ActionResult SetOrder(FormCollection formCollection, [FetchProduct(KeyName = "id")]ProductEntity entity, int? order)
        {
            var jsonR = new JsonResult();

            if (order == null || entity == null)
            {
                jsonR.Data = new ExecuteResult { Message = "参数错误", StatusCode = StatusCode.ClientError };
            }
            else
            {
                var t = _productRepository.SetSortOrder(entity, order.Value, CurrentUser.CustomerId);

                if (t != null)
                {
                    jsonR.Data = new ExecuteResult { Message = "OK", StatusCode = StatusCode.Success };
                }
                else
                {
                    jsonR.Data = new ExecuteResult { Message = "返回为空", StatusCode = StatusCode.UnKnow };
                }
            }

            return jsonR;
        }
    }
}
