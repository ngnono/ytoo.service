using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Yintai.Architecture.Common.Models;
using Yintai.Architecture.Framework.Extension;
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
    public class CommentController : UserController
    {
        private readonly ICommentRepository _commentRepository;
        public CommentController(ICommentRepository commentRepository)
        {
            this._commentRepository = commentRepository;
        }

        public ActionResult Index(PagerRequest request, int? sort, int? sourceId, int? sourceType, int? userId)
        {
            return List(request, sort, sourceId, sourceType, userId);
        }

        public ActionResult List(PagerRequest request, int? sort, int? sourceId, int? sourceType, int? userId)
        {
            int totalCount;
            var sortOrder = (CommentSortOrder)(sort ?? 0);

            List<CommentEntity> data;

            if (sourceId != null || sourceType != null || userId != null)
            {
                data = _commentRepository.GetPagedList(request, out totalCount, sortOrder, sourceId, EnumExtension.Parser<SourceType>(sourceType ?? 0), userId);
            }
            else
            {
                data = _commentRepository.GetPagedList(request, out totalCount, sortOrder);
            }

            var vo = MappingManager.CommentViewMapping(data);

            var v = new CommentCollectionViewModel(request, totalCount) { Comments = vo.ToList() };

            return View("List", v);
        }

        public ActionResult Details(int? id, [FetchComment(KeyName = "id")]CommentEntity entity)
        {
            if (id == null || entity == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View();
            }

            var vo = MappingManager.CommentViewMapping(entity);

            return View(vo);
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Delete(int? id, [FetchComment(KeyName = "id")]CommentEntity entity)
        {
            if (id == null || entity == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View();
            }

            var vo = MappingManager.CommentViewMapping(entity);

            return View(vo);
        }

        public ActionResult Edit(int? id, [FetchComment(KeyName = "id")]CommentEntity entity)
        {
            if (id == null || entity == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View();
            }

            var vo = MappingManager.CommentViewMapping(entity);

            return View(vo);
        }

        [HttpPost]
        public ActionResult Create(FormCollection formCollection, CommentViewModel vo)
        {
            if (ModelState.IsValid)
            {
                var entity = MappingManager.CommentEntityMapping(vo);
                entity.CreatedUser = base.CurrentUser.CustomerId;
                entity.UpdatedUser = base.CurrentUser.CustomerId;
                entity.Status = (int)DataStatus.Normal;

                entity = this._commentRepository.Insert(entity);

                return Success("/" + RouteData.Values["controller"] + "/edit/" + entity.Id.ToString(CultureInfo.InvariantCulture));
            }

            return View(vo);
        }

        [HttpPost]
        public ActionResult Edit(FormCollection formCollection, [FetchComment(KeyName = "id")]CommentEntity entity, CommentViewModel vo)
        {
            if (entity == null || !ModelState.IsValid)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View(vo);
            }

            var newEntity = MappingManager.CommentEntityMapping(vo);
            newEntity.CreatedUser = entity.CreatedUser;
            newEntity.CreatedDate = entity.CreatedDate;
            newEntity.Status = entity.Status;

            MappingManager.CommentEntityMapping(newEntity, entity);

            this._commentRepository.Update(entity);


            return Success("/" + RouteData.Values["controller"] + "/details/" + entity.Id.ToString(CultureInfo.InvariantCulture));
        }

        [HttpPost]
        public ActionResult Delete(FormCollection formCollection, [FetchComment(KeyName = "id")]CommentEntity entity)
        {
            if (entity == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View();
            }

            entity.UpdatedDate = DateTime.Now;
            entity.UpdatedUser = CurrentUser.CustomerId;
            entity.Status = (int)DataStatus.Deleted;

            this._commentRepository.Delete(entity);

            return Success("/" + RouteData.Values["controller"] + "/" + RouteData.Values["action"]);
        }

    }
}
