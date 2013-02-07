using System;
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
    public class TagController : UserController
    {
        private readonly ITagRepository _tagRepository;
        public TagController(ITagRepository tagRepository)
        {
            this._tagRepository = tagRepository;
        }

        public ActionResult Index(PagerRequest request, int? sort)
        {
            return List(request, sort);
        }

        public ActionResult List(PagerRequest request, int? sort)
        {
            int totalCount;
            var sortOrder = (TagSortOrder)(sort ?? 0);

            var data = _tagRepository.GetPagedList(request, out totalCount, sortOrder);
            var vo = MappingManager.TagViewMapping(data);

            var v = new TagCollectionViewModel(request, totalCount) { Tags = vo.ToList() };

            return View("List", v);
        }

        public ActionResult Details(int? id, [FetchTag(KeyName = "id")]TagEntity entity)
        {
            if (id == null || entity == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View();
            }

            var vo = MappingManager.TagViewMapping(entity);

            return View(vo);
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Delete(int? id, [FetchTag(KeyName = "id")]TagEntity entity)
        {
            if (id == null || entity == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View();
            }

            var vo = MappingManager.TagViewMapping(entity);

            return View(vo);
        }

        public ActionResult Edit(int? id, [FetchTag(KeyName = "id")]TagEntity entity)
        {
            if (id == null || entity == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View();
            }

            var vo = MappingManager.TagViewMapping(entity);

            return View(vo);
        }

        [HttpPost]
        public ActionResult Create(FormCollection formCollection, TagViewModel vo)
        {
            if (ModelState.IsValid)
            {
                var entity = MappingManager.TagEntityMapping(vo);
                entity.CreatedUser = base.CurrentUser.CustomerId;
                entity.UpdatedUser = base.CurrentUser.CustomerId;
                entity.Status = (int)DataStatus.Normal;

                entity = this._tagRepository.Insert(entity);

                return Success("/" + RouteData.Values["controller"] + "/edit/" + entity.Id.ToString(CultureInfo.InvariantCulture));
            }

            return View(vo);
        }

        [HttpPost]
        public ActionResult Edit(FormCollection formCollection, [FetchTag(KeyName = "id")]TagEntity entity, TagViewModel vo)
        {
            if (entity == null || !ModelState.IsValid)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View(vo);
            }

            var newEntity = MappingManager.TagEntityMapping(vo);
            newEntity.CreatedUser = entity.CreatedUser;
            newEntity.CreatedDate = entity.CreatedDate;
            newEntity.Status = entity.Status;

            MappingManager.TagEntityMapping(newEntity, entity);

            this._tagRepository.Update(entity);


            return Success("/" + RouteData.Values["controller"] + "/details/" + entity.Id.ToString(CultureInfo.InvariantCulture));
        }

        [HttpPost]
        public ActionResult Delete(FormCollection formCollection, [FetchTag(KeyName = "id")]TagEntity entity)
        {
            if (entity == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View();
            }

            entity.UpdatedDate = DateTime.Now;
            entity.UpdatedUser = CurrentUser.CustomerId;
            entity.Status = (int)DataStatus.Deleted;

            this._tagRepository.Delete(entity);

            return Success("/" + RouteData.Values["controller"] + "/" + RouteData.Values["action"]);
        }
        [HttpGet]
        public override JsonResult AutoComplete(string name)
        {
            return Json(_tagRepository.GetListForAll().Where(entity => string.IsNullOrEmpty(name) ? true : entity.Name.StartsWith(name.Trim())).Take(10)
                ,JsonRequestBehavior.AllowGet);
        }
    }
}
