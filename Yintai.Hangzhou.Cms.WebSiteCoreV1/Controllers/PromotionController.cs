using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.Cms.WebSiteCoreV1.Manager;
using Yintai.Hangzhou.Cms.WebSiteCoreV1.Models;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Repository.Contract;
using Yintai.Hangzhou.WebSupport.Binder;
using Yintai.Hangzhou.WebSupport.Mvc;

namespace Yintai.Hangzhou.Cms.WebSiteCoreV1.Controllers
{
    [AdminAuthorize]
    public class PromotionController : UserController
    {
        private readonly IPromotionRepository _promotionRepository;
        public PromotionController(IPromotionRepository promotionRepository)
        {
            this._promotionRepository = promotionRepository;
        }

        public ActionResult Index(PagerRequest request, int? sort, string name, int? recommendUser, int? tagId)
        {
            return List(request, sort, name, recommendUser, tagId);
        }

        public ActionResult List(PagerRequest request, int? sort, string name, int? recommendUser, int? tagId)
        {
            int totalCount;
            var sortOrder = (PromotionSortOrder)(sort ?? (int)PromotionSortOrder.CreatedDateDesc);

            List<PromotionEntity> data;

            List<int> tag = null;
            if (tagId != null)
            {
                tag = new List<int>(tagId.Value);
            }

            if (!String.IsNullOrWhiteSpace(name) || recommendUser != null || tagId != null)
            {
                data = _promotionRepository.GetPagedListForSearch(request, out totalCount, sortOrder, null, null, null, recommendUser, null, name, tag, null);
            }
            else
            {
                data = _promotionRepository.GetPagedList(request, out totalCount, sortOrder, null, null, null, null);
            }

            var vo = MappingManager.PromotionViewMapping(data);

            var v = new PromotionCollectionViewModel(request, totalCount) { Promotions = vo.ToList() };

            return View("List", v);
        }

        public ActionResult Details(int? id, [FetchPromotion(KeyName = "id")]PromotionEntity entity)
        {
            if (id == null || entity == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View();
            }

            var vo = MappingManager.PromotionViewMapping(entity);

            return View(vo);
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Delete(int? id, [FetchPromotion(KeyName = "id")]PromotionEntity entity)
        {
            if (id == null || entity == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View();
            }

            var vo = MappingManager.PromotionViewMapping(entity);

            return View(vo);
        }

        public ActionResult Edit(int? id, [FetchPromotion(KeyName = "id")]PromotionEntity entity)
        {
            if (id == null || entity == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View();
            }

            var vo = MappingManager.PromotionViewMapping(entity);

            return View(vo);
        }

        [HttpPost]
        public ActionResult Create(FormCollection formCollection, PromotionViewModel vo)
        {
            if (ModelState.IsValid)
            {
                var entity = MappingManager.PromotionEntityMapping(vo);
                entity.CreatedUser = base.CurrentUser.CustomerId;
                entity.UpdatedUser = base.CurrentUser.CustomerId;
                entity.Status = (int)DataStatus.Normal;

                entity = this._promotionRepository.Insert(entity);

                return Success("/" + RouteData.Values["controller"] + "/edit/" + entity.Id.ToString(CultureInfo.InvariantCulture));
            }

            return View(vo);
        }

        [HttpPost]
        public ActionResult Edit(FormCollection formCollection, [FetchPromotion(KeyName = "id")]PromotionEntity entity, PromotionViewModel vo)
        {
            if (entity == null || !ModelState.IsValid)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View(vo);
            }

            var newEntity = MappingManager.PromotionEntityMapping(vo);
            newEntity.CreatedUser = entity.CreatedUser;
            newEntity.CreatedDate = entity.CreatedDate;
            newEntity.Status = entity.Status;
            newEntity.InvolvedCount = entity.InvolvedCount;
            newEntity.FavoriteCount = entity.FavoriteCount;
            newEntity.ShareCount = entity.ShareCount;
            newEntity.LikeCount = entity.LikeCount;
            newEntity.UpdatedDate = DateTime.Now;
            newEntity.UpdatedUser = base.CurrentUser.CustomerId;
            newEntity.RecommendUser = entity.RecommendUser;
            newEntity.RecommendSourceId = entity.RecommendSourceId;
            newEntity.RecommendSourceType = entity.RecommendSourceType;

            MappingManager.PromotionEntityMapping(newEntity, entity);

            this._promotionRepository.Update(entity);


            return Success("/" + RouteData.Values["controller"] + "/details/" + entity.Id.ToString(CultureInfo.InvariantCulture));
        }

        [HttpPost]
        public ActionResult Delete(FormCollection formCollection, [FetchPromotion(KeyName = "id")]PromotionEntity entity)
        {
            if (entity == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View();
            }

            entity.UpdatedDate = DateTime.Now;
            entity.UpdatedUser = CurrentUser.CustomerId;
            entity.Status = (int)DataStatus.Deleted;

            this._promotionRepository.Delete(entity);

            return Success("/" + RouteData.Values["controller"] + "/" + RouteData.Values["action"]);
        }
        [HttpGet]
        public override JsonResult AutoComplete(string name)
        {
            return Json(_promotionRepository.AutoComplete(name).Where(entity => string.IsNullOrEmpty(name) ? true : entity.Name.StartsWith(name.Trim())).Take(10)
                , JsonRequestBehavior.AllowGet);
        }
    }
}
