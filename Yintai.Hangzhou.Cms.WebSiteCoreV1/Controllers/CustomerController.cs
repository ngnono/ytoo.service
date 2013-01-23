using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.Cms.WebSiteCoreV1.Manager;
using Yintai.Hangzhou.Cms.WebSiteCoreV1.Models;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Repository.Contract;
using Yintai.Hangzhou.WebSupport.Binder;
using Yintai.Hangzhou.WebSupport.Mvc;

namespace Yintai.Hangzhou.Cms.WebSiteCoreV1.Controllers
{
    [AdminAuthorize]
    public class CustomerController : UserController
    {
        private readonly ICustomerRepository _customerRepository;
        public CustomerController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public ActionResult Index(PagerRequest request, int? sort, string mobile, string nickName)
        {
            return List(request, sort, mobile, nickName);
        }

        public ActionResult List(PagerRequest request, int? sort, string mobile, string nickName)
        {
            int totalCount;
            var sortOrder = (CustomerSortOrder)(sort ?? 0);
            List<UserEntity> data;
            if (String.IsNullOrWhiteSpace(mobile) && String.IsNullOrWhiteSpace(nickName))
            {
                data = _customerRepository.GetPagedList(request, out totalCount, sortOrder);
            }
            else
            {
                data = _customerRepository.GetPagedListForNickName(request, out totalCount, sortOrder, nickName.Trim(), mobile.Trim());
            }

            var vo = MappingManager.CustomerViewMapping(MappingManager.UserModelMapping(data).ToList());

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

        public ActionResult Delete(int? id, [FetchUser(KeyName = "id")]UserModel entity)
        {
            if (id == null || entity == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View();
            }

            var vo = MappingManager.CustomerViewMapping(entity);

            return View(vo);
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

                entity = this._customerRepository.Insert(entity);

                return Success("/" + RouteData.Values["controller"] + "/edit/" + entity.Id.ToString(CultureInfo.InvariantCulture));
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

            var newEntity = MappingManager.UserEntityMapping(MappingManager.UserModelMapping(vo));
            newEntity.CreatedUser = entity.CreatedUser;
            newEntity.CreatedDate = entity.CreatedDate;
            newEntity.Status = entity.Status;

            MappingManager.UserEntityMapping(newEntity, entity);

            this._customerRepository.Update(entity);


            return Success("/" + RouteData.Values["controller"] + "/details/" + entity.Id.ToString(CultureInfo.InvariantCulture));
        }

        [HttpPost]
        public ActionResult Delete(FormCollection formCollection, [FetchUser(KeyName = "id")]UserModel model)
        {
            if (model == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View();
            }

            var entity = _customerRepository.GetItem(model.Id);

            entity.UpdatedDate = DateTime.Now;
            entity.UpdatedUser = CurrentUser.CustomerId;
            entity.Status = (int)DataStatus.Deleted;

            this._customerRepository.Delete(entity);

            return Success("/" + RouteData.Values["controller"] + "/" + RouteData.Values["action"]);
        }
    }
}
