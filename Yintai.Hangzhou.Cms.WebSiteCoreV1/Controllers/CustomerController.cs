using com.intime.fashion.common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Mvc;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.Cms.WebSiteCoreV1.Manager;
using Yintai.Hangzhou.Cms.WebSiteCoreV1.Models;
using Yintai.Hangzhou.Cms.WebSiteCoreV1.Util;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Model.Filters;
using Yintai.Hangzhou.Repository.Contract;
using Yintai.Hangzhou.Service.Contract;
using Yintai.Hangzhou.WebSupport.Binder;
using Yintai.Hangzhou.WebSupport.Mvc;

namespace Yintai.Hangzhou.Cms.WebSiteCoreV1.Controllers
{
    [AdminAuthorize]
    public class CustomerController : UserController
    {
        private readonly ICustomerRepository _customerRepository;
        private IResourceService _resourceService;
        private IResourceRepository _resourceRepository;
        public CustomerController(ICustomerRepository customerRepository
            ,IResourceService resourceService
            ,IResourceRepository resourceRepository)
        {
            _customerRepository = customerRepository;
            _resourceService = resourceService;
            _resourceRepository = resourceRepository;
        }

        public ActionResult Index(PagerRequest request, int? sort, string mobile, string nickName)
        {
            return View();
        }

        public ActionResult List(PagerRequest request, CustomerListSearchOption search)
        {
            int totalCount;
            var data = _customerRepository.Get(e => (!search.PId.HasValue || e.Id == search.PId.Value)
                                                    && (string.IsNullOrEmpty(search.Name) || e.Name.ToLower().StartsWith(search.Name.ToLower()) ||
                                                        e.Nickname.ToLower().StartsWith(search.Name.ToLower()))
                                                    && e.Status != (int)DataStatus.Deleted
                                                    && (string.IsNullOrEmpty(search.Mobile) || e.Mobile == search.Mobile)
                                                    && (string.IsNullOrEmpty(search.Email) || e.EMail.ToLower().StartsWith(search.Email.ToLower()))
                                                  
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
                                                              return e.OrderByDescending(o => o.Name);
                                                          case GenericOrder.OrderByCreateDate:
                                                          default:
                                                              return e.OrderByDescending(o => o.CreatedDate);

                                                      }
                                                  }
                                              });
            
         
            var vo = MappingManager.CustomerViewMapping(MappingManager.UserModelMapping(data.ToList()).ToList());

            var v = new CustomerCollectionViewModel(request, totalCount) { Customers = vo.ToList() };

            return View("List", v);
        }

        public ActionResult Details(int? id, [FetchUser(KeyName = "id")]UserModel entity)
        {
            if (id == null || entity == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View();
            }

            var vo = MappingManager.CustomerViewMapping(entity);

            return View(vo);
        }

        public ActionResult Create()
        {
            return View();
        }


        public ActionResult Edit(int? id, [FetchUser(KeyName = "id")]UserModel entity)
        {
            if (id == null || entity == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View();
            }

            var vo = MappingManager.CustomerViewMapping(entity);

            return View(vo);
        }

        [HttpPost]
        public ActionResult Create(FormCollection formCollection, CustomerViewModel vo)
        {
            if (ModelState.IsValid)
            {
                var entity = MappingManager.UserEntityMapping(MappingManager.UserModelMapping(vo));
                entity.CreatedUser = base.CurrentUser.CustomerId;
                entity.UpdatedUser = base.CurrentUser.CustomerId;
                entity.Status = (int)DataStatus.Normal;
                entity.LastLoginDate = DateTime.Now;
                entity.Password = SecurityHelper.ComputeHash(entity.Password);
                using (var ts = new TransactionScope())
                {
                    entity = this._customerRepository.Insert(entity);
                    if (ControllerContext.HttpContext.Request.Files.Count > 0)
                    {
                       var resources = this._resourceService.Save(ControllerContext.HttpContext.Request.Files
                              , CurrentUser.CustomerId
                            , -1, entity.Id
                            , SourceType.CustomerPortrait);
                       if (resources != null &&
                           resources.Count() > 0)
                       {
                           entity.Logo = resources[0].AbsoluteUrl;
                           _customerRepository.Update(entity);
                           ts.Complete();
                           return RedirectToAction("Details", new { id = entity.Id });
                       }
                    }

                    
                }
               
            }

            return View(vo);
        }

        [HttpPost]
        public ActionResult Edit(FormCollection formCollection, [FetchUser(KeyName = "id")]UserModel model, CustomerViewModel vo)
        {

            if (model == null || !ModelState.IsValid)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View(vo);
            }

            var entity = _customerRepository.GetItem(model.Id);
            entity.Mobile = vo.Mobile;
            entity.Name = vo.Name;
            entity.Nickname = vo.Nickname;
            if (string.Compare(vo.Password, entity.Password, false) != 0)
                entity.Password = SecurityHelper.ComputeHash(entity.Password);
            entity.UpdatedDate = DateTime.Now;
           entity.UpdatedUser = CurrentUser.CustomerId;
            entity.UserLevel = vo.UserLevel;
            entity.Description = vo.Description;
            entity.EMail = vo.EMail;
            using (var ts = new TransactionScope())
            {

                if (ControllerContext.HttpContext.Request.Files.Count > 0)
                {
                    var oldImage = _resourceRepository.Get(r => r.SourceType == (int)SourceType.CustomerPortrait &&
                         r.SourceId == entity.Id).FirstOrDefault();
                    if (oldImage!=null)
                     _resourceService.Del(oldImage.Id);
                    var resources = this._resourceService.Save(ControllerContext.HttpContext.Request.Files
                           , CurrentUser.CustomerId
                         , -1, entity.Id
                         , SourceType.CustomerPortrait);
                    if (resources != null &&
                        resources.Count() > 0)
                    {
                        entity.Logo = resources[0].AbsoluteUrl;     
                    }

                }
                this._customerRepository.Update(entity);

                ts.Complete();
                       
                return RedirectToAction("Details", new { id = entity.Id });
            }
          }

        [HttpPost]
        public JsonResult Delete([FetchUser(KeyName = "id")]UserModel model)
        {
            if (model == null)
            {
                return FailResponse();
            }

            var entity = _customerRepository.GetItem(model.Id);
            entity.UpdatedDate = DateTime.Now;
            entity.UpdatedUser = CurrentUser.CustomerId;
            entity.Status = (int)DataStatus.Deleted;

            this._customerRepository.Delete(entity);

            return SuccessResponse();
        }

        [HttpGet]
        public override JsonResult AutoComplete(string name)
        {
            return Json(_customerRepository.AutoComplete(name).Where(entity => entity.UserLevel != (int)UserLevel.User && entity.UserLevel != (int)UserLevel.None)
            .Where(entity => String.IsNullOrEmpty(name) ? true : 
                            (entity.Nickname.StartsWith(name.Trim()) 
                                    || entity.Mobile.StartsWith(name.Trim())
                                    || entity.Name.StartsWith(name.Trim())))
            .Take(10)
                , JsonRequestBehavior.AllowGet);
        }
    }
}
