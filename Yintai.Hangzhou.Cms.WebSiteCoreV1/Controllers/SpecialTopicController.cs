using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.Cms.WebSiteCoreV1.Models;
using Yintai.Hangzhou.Cms.WebSiteCoreV1.Util;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Repository.Contract;
using Yintai.Hangzhou.Service.Contract;
using Yintai.Hangzhou.WebSupport.Binder;
using Yintai.Hangzhou.WebSupport.Mvc;

namespace Yintai.Hangzhou.Cms.WebSiteCoreV1.Controllers
{
    [AdminAuthorize]
    public class SpecialTopicController : UserController
    {
        private readonly ISpecialTopicRepository _specialTopicRepository;
        private IResourceService _resourceRepository;
        public SpecialTopicController(ISpecialTopicRepository specialTopicRepository,
            IResourceService resourceRepository)
        {
            this._specialTopicRepository = specialTopicRepository;
            _resourceRepository = resourceRepository;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List(PagerRequest request,SpecialTopicListSearchOption search)
        {
            int totalCount;
            var data = _specialTopicRepository.Get(e =>(!search.PId.HasValue || e.Id== search.PId.Value)
                                                       && (string.IsNullOrEmpty(search.Name) || e.Name.ToLower().StartsWith(search.Name.ToLower()))
                                                      && (!search.Status.HasValue || e.Status == (int)search.Status.Value)
                                                      && e.Status!=(int)DataStatus.Deleted
                                                , out totalCount
                                                , request.PageIndex
                                                , request.PageSize
                                                , e => {
                                                    if (!search.OrderBy.HasValue)
                                                        return e.OrderByDescending(o=>o.CreatedDate);
                                                    else
                                                    {
                                                        switch (search.OrderBy.Value)
                                                        {
                                                            case GenricOrder.OrderByCreateUser:
                                                                return e.OrderByDescending(o=>o.CreatedUser);
                                                            case GenricOrder.OrderByName:
                                                                return e.OrderByDescending(o=>o.Name);
                                                            case GenricOrder.OrderByCreateDate:
                                                            default:
                                                                return e.OrderByDescending(o=>o.CreatedDate);

                                                        }
                                                    }
                                                });
                                                      
            var vo = MappingManager.SpecialTopicViewMapping(data.ToList());
            
            var v = new SpecialTopicCollectionViewModel(request, totalCount) { SpecialTopics = vo.ToList() };
            ViewBag.SearchOptions = search;
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
                entity.Status = (int)DataStatus.Default;
                entity.CreatedDate = DateTime.Now;
                entity.UpdatedDate = DateTime.Now;
                using (TransactionScope ts = new TransactionScope())
                {

                    entity = this._specialTopicRepository.Insert(entity);
                    var ids = _resourceRepository.Save(ControllerContext.HttpContext.Request.Files
                        ,CurrentUser.CustomerId
                        ,-1, entity.Id
                        ,SourceType.SpecialTopic);
                    ts.Complete();
                }
                return RedirectToAction("List");
            
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
            newEntity.UpdatedDate = DateTime.Now;
            newEntity.UpdatedUser = base.CurrentUser.CustomerId;

            MappingManager.SpecialTopicEntityMapping(newEntity, entity);
            using (TransactionScope ts = new TransactionScope())
            {
                this._specialTopicRepository.Update(entity);
                if (ControllerContext.HttpContext.Request.Files.Count > 0 ) 
                {
                    foreach (string fileName in ControllerContext.HttpContext.Request.Files)
                    {
                        var file = ControllerContext.HttpContext.Request.Files[fileName];
                        if (file == null || file.ContentLength == 0)
                            continue;
                        //remove existing resource
                        var resourceParts=fileName.Split('_');
                        if (resourceParts.Length > 1)
                        {
                            int resourceId = int.Parse(resourceParts[1]);
                            _resourceRepository.Del(resourceId);
                        }
                        
                    }
                    //add new resource
                    _resourceRepository.Save(ControllerContext.HttpContext.Request.Files
                          , CurrentUser.CustomerId
                        , -1, entity.Id
                        , SourceType.SpecialTopic);
                }
                ts.Complete();
            }
            return RedirectToAction("List");

       }

        [HttpPost]
        public JsonResult Delete([FetchSpecialTopic(KeyName = "id")]SpecialTopicEntity entity)
        {
            try
            {
                entity.UpdatedDate = DateTime.Now;
                entity.UpdatedUser = CurrentUser.CustomerId;
                entity.Status = (int)DataStatus.Deleted;

                this._specialTopicRepository.Update(entity);
                return SuccessResponse();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return FailResponse();
            }
        }
        [HttpGet]
        public override JsonResult AutoComplete(string name)
        {
            return Json(_specialTopicRepository.AutoComplete(name).Where(entity => string.IsNullOrEmpty(name) ? true : entity.Name.StartsWith(name.Trim())).Take(10)
                , JsonRequestBehavior.AllowGet);
        }
    }
}
