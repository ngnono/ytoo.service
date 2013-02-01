using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.Cms.WebSiteCoreV1.Models;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Repository.Contract;
using Yintai.Hangzhou.WebSupport.Binder;
using Yintai.Hangzhou.WebSupport.Mvc;

namespace Yintai.Hangzhou.Cms.WebSiteCoreV1.Controllers
{
    [AdminAuthorize]
    public class SpecialTopicController : UserController
    {
        private readonly ISpecialTopicRepository _specialTopicRepository;
        public SpecialTopicController(ISpecialTopicRepository specialTopicRepository)
        {
            this._specialTopicRepository = specialTopicRepository;
        }

        public ActionResult Index(PagerRequest request, int? sort)
        {
            return List(request, sort);
        }

        public ActionResult List(PagerRequest request, int? sort)
        {
            int totalCount;
            var sortOrder = (SpecialTopicSortOrder)(sort ?? 0);

            var data = _specialTopicRepository.GetPagedList(request, out totalCount, sortOrder, null);
            var vo = MappingManager.SpecialTopicViewMapping(data);

            var v = new SpecialTopicCollectionViewModel(request, totalCount) { SpecialTopics = vo.ToList() };

            return View("List", v);
        }

        public ActionResult Details(int? id, [FetchSpecialTopic(KeyName = "id")]SpecialTopicEntity entity)
        {
            if (id == null || entity == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View();
            }

            var vo = MappingManager.SpecialTopicViewMapping(entity);

            return View(vo);
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Delete(int? id, [FetchSpecialTopic(KeyName = "id")]SpecialTopicEntity entity)
        {
            if (id == null || entity == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View();
            }

            var vo = MappingManager.SpecialTopicViewMapping(entity);

            return View(vo);
        }

        public ActionResult Edit(int? id, [FetchSpecialTopic(KeyName = "id")]SpecialTopicEntity entity)
        {
            if (id == null || entity == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View();
            }

            var vo = MappingManager.SpecialTopicViewMapping(entity);

            return View(vo);
        }

        [HttpPost]
        public ActionResult Create(FormCollection formCollection, SpecialTopicViewModel vo)
        {
            if (ModelState.IsValid)
            {
                var entity = MappingManager.SpecialTopicEntityMapping(vo);
                entity.CreatedUser = base.CurrentUser.CustomerId;
                entity.UpdatedUser = base.CurrentUser.CustomerId;
                entity.Status = (int)DataStatus.Normal;
                entity.CreatedDate = DateTime.Now;
                entity.UpdatedDate = DateTime.Now;

                entity = this._specialTopicRepository.Insert(entity);

                return Success("/" + RouteData.Values["controller"] + "/edit/" + entity.Id.ToString(CultureInfo.InvariantCulture));
            }

            return View(vo);
        }

        [HttpPost]
        public ActionResult Edit(FormCollection formCollection, [FetchSpecialTopic(KeyName = "id")]SpecialTopicEntity entity, SpecialTopicViewModel vo)
        {
            if (entity == null || !ModelState.IsValid)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View(vo);
            }

            var newEntity = MappingManager.SpecialTopicEntityMapping(vo);
            newEntity.CreatedUser = entity.CreatedUser;
            newEntity.CreatedDate = entity.CreatedDate;
            newEntity.Status = entity.Status;
            newEntity.UpdatedDate = DateTime.Now;
            newEntity.UpdatedUser = base.CurrentUser.CustomerId;

            MappingManager.SpecialTopicEntityMapping(newEntity, entity);

            this._specialTopicRepository.Update(entity);


            return Success("/" + RouteData.Values["controller"] + "/details/" + entity.Id.ToString(CultureInfo.InvariantCulture));
        }

        [HttpPost]
        public ActionResult Delete(FormCollection formCollection, [FetchSpecialTopic(KeyName = "id")]SpecialTopicEntity entity)
        {
            if (entity == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View();
            }

            entity.UpdatedDate = DateTime.Now;
            entity.UpdatedUser = CurrentUser.CustomerId;
            entity.Status = (int)DataStatus.Deleted;

            this._specialTopicRepository.Delete(entity);

            return Success("/" + RouteData.Values["controller"] + "/" + RouteData.Values["action"]);
        }

    }
}
