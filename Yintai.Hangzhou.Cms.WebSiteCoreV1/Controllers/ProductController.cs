using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.Cms.WebSiteCoreV1.Models;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;
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

        public ProductController(IProductRepository productRepository)
        {
            this._productRepository = productRepository;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List(PagerRequest request, int? sort, string name, int? recommendUser, int? tagId, int? brandId)
        {
            int totalCount;
            var sortOrder = (ProductSortOrder)(sort ?? 0);
            var timestamp = new Timestamp();

            List<int> tag = null;
            if (tagId != null)
            {
                tag = new List<int>(tagId.Value);
            }

            List<ProductEntity> data;

            if (!String.IsNullOrWhiteSpace(name) || recommendUser != null || tagId != null || brandId != null)
            {
                data = _productRepository.GetPagedListForSearch(request, out totalCount, sortOrder, timestamp, name, recommendUser, tag, brandId);
            }
            else
            {
                data = _productRepository.GetPagedList(request, out totalCount, sortOrder, timestamp, null, null, null);
            }

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

                entity = this._productRepository.Insert(entity);

                return Success("/" + RouteData.Values["controller"] + "/edit/" + entity.Id.ToString(CultureInfo.InvariantCulture));
            }

            return View(vo);
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

            this._productRepository.Update(entity);


            return Success("/" + RouteData.Values["controller"] + "/details/" + entity.Id.ToString(CultureInfo.InvariantCulture));
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
