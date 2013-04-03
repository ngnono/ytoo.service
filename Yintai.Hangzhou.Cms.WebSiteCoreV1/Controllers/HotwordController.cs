using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.Cms.WebSiteCoreV1.Models;
using Yintai.Hangzhou.Cms.WebSiteCoreV1.Util;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Model.Filters;
using Yintai.Hangzhou.Repository.Contract;

namespace Yintai.Hangzhou.Cms.WebSiteCoreV1.Controllers
{
    [AdminAuthorize]
    public class HotwordController:UserController
    {
         private readonly IHotWordRepository _hotwordRepo;
         private IBrandRepository _brandRepo;

        public HotwordController(IHotWordRepository hotwordRepo,
            IBrandRepository brandRepo)
        {
            _hotwordRepo = hotwordRepo;
            _brandRepo = brandRepo;
           
        }
        public ActionResult Index(PagerRequest request,HotwordSearchOption search)
        {
            return List(request,search);
        }

        public ActionResult List(PagerRequest request,HotwordSearchOption search)
        {
            int totalCount;
            var data = _hotwordRepo.Get(e => (!search.Type.HasValue || e.Type == search.Type.Value)
                                               && (string.IsNullOrEmpty(search.Word) || e.Word.Contains(search.Word))
                                               && e.Status != (int)DataStatus.Deleted
                                             , out totalCount
                                             , request.PageIndex
                                             , request.PageSize
                                             , e =>
                                             {
                                                 if (!search.OrderBy.HasValue)
                                                     return e.OrderByDescending(o => o.CreatedDate);
                                                 else
                                                 {
                                                     switch (search.OrderBy.Value)
                                                     {
                                                         case GenericOrder.OrderByCreateUser:
                                                             return e.OrderByDescending(o => o.CreatedUser);
                                                         case GenericOrder.OrderByName:
                                                             return e.OrderByDescending(o => o.Word);
                                                         case GenericOrder.OrderByCreateDate:
                                                         default:
                                                             return e.OrderByDescending(o => o.CreatedDate);

                                                     }
                                                 }
                                                
                                             });

            var models = from d in data.ToList()
                         select new HotwordViewModel().FromEntity<HotWordEntity, HotwordViewModel>(d);

            return View("List", new Pager<HotwordViewModel>(request, totalCount) { Data=models});
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View();
            }
            var entity = _hotwordRepo.Find(id.Value);
            if (entity == null)
            {
                throw new ApplicationException("entity not exists!");
            }

            return View(new HotwordViewModel().FromEntity<HotWordEntity,HotwordViewModel>(entity));
        }

        [HttpPost]
        public ActionResult Create(FormCollection formCollection, HotwordViewModel vo)
        {
            if (ModelState.IsValid)
            {
                if (vo.Type == (int)HotWordType.BrandStruct)
                {
                    vo.BrandName = _brandRepo.Find(vo.BrandId).Name;
                }
                var entity = vo.ToEntity<HotwordViewModel,HotWordEntity>();
                entity.CreatedUser = base.CurrentUser.CustomerId;
                entity.CreatedDate = DateTime.Now;
                entity.UpdatedUser = base.CurrentUser.CustomerId;
                entity.UpdatedDate = DateTime.Now;
                _hotwordRepo.Insert(entity);
                return RedirectToAction("Edit", new { Id = entity.Id});
            }

            return View(vo);
        }

        [HttpPost]
        public ActionResult Edit(FormCollection formCollection, HotwordViewModel vo)
        {
            if (ModelState.IsValid)
            {
                var entity = _hotwordRepo.Find(vo.Id);
                if (entity == null)
                    throw new ApplicationException("entity not exists!");
                entity.SortOrder = vo.SortOrder;
                entity.Status = vo.Status;
                entity.Type = vo.Type;
                entity.UpdatedDate = DateTime.Now;
                entity.UpdatedUser = CurrentUser.CustomerId;
                entity.Word = vo.Word;
                if (entity.Type == (int)HotWordType.BrandStruct)
                {
                    vo.BrandName = _brandRepo.Find(vo.BrandId).Name;
                    entity.Word = vo.BrandString;
                }
                _hotwordRepo.Update(entity);
                return RedirectToAction("List");
            }
            return View(vo);

        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            var entity = _hotwordRepo.Find(id);
            if (entity == null)
            {

                return FailResponse();
            }

            entity.UpdatedDate = DateTime.Now;
            entity.UpdatedUser = CurrentUser.CustomerId;
            entity.Status = (int)DataStatus.Deleted;

            _hotwordRepo.Update(entity);
            return SuccessResponse();
        }
    }
}
