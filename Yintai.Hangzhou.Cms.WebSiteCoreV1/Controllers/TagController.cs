using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.Cms.WebSiteCoreV1.Manager;
using Yintai.Hangzhou.Cms.WebSiteCoreV1.Models;
using Yintai.Hangzhou.Cms.WebSiteCoreV1.Util;
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

                return RedirectToAction("Details", new { id = entity.Id });
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

            entity.Description = vo.Description??string.Empty;
            entity.Name = vo.Name;
            entity.SortOrder = vo.SortOrder;
            entity.UpdatedDate = DateTime.Now;
            entity.UpdatedUser = CurrentUser.CustomerId;
            this._tagRepository.Update(entity);
            return RedirectToAction("Details", new { id = entity.Id });
        }

        [HttpPost]
        public JsonResult Delete([FetchTag(KeyName = "id")]TagEntity entity)
        {
            if (entity == null)
            {
              
                return FailResponse();
            }

            entity.UpdatedDate = DateTime.Now;
            entity.UpdatedUser = CurrentUser.CustomerId;
            entity.Status = (int)DataStatus.Deleted;

            this._tagRepository.Delete(entity);

            return SuccessResponse();
        }
        [HttpGet]
        public override JsonResult AutoComplete(string name)
        {
            return Json(_tagRepository.AutoComplete(name).Where(entity => string.IsNullOrEmpty(name) ? true : entity.Name.StartsWith(name.Trim())).Take(10)
                ,JsonRequestBehavior.AllowGet);
        }
    }
}
