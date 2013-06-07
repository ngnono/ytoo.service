using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.Cms.WebSiteV1.Models;
using Yintai.Hangzhou.Cms.WebSiteV1.Util;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Model.Filters;
using Yintai.Hangzhou.Repository.Contract;

namespace Yintai.Hangzhou.Cms.WebSiteV1.Controllers
{
    [AdminAuthorize]
    public class UserAuthController:UserController
    {
        private IUserAuthRepository _authRepo;
        private IStoreRepository _storeRepo;
        private IBrandRepository _brandRep;
        private ICustomerRepository _customerRepo;

        public UserAuthController(IUserAuthRepository authRepo
            ,IStoreRepository storeRepo
            ,IBrandRepository brandRepo
            ,ICustomerRepository customerRepo)
        {
            _authRepo = authRepo;
            _storeRepo = storeRepo;
            _brandRep = brandRepo;
            _customerRepo = customerRepo;
           
        }
        public ActionResult Index(PagerRequest request,UserAuthSearchOption search)
        {
            return List(request,search);
        }

        public ActionResult List(PagerRequest request, UserAuthSearchOption search)
        {
            int totalCount;
            var data = _authRepo.Get(e => (!search.Type.HasValue || e.Type == search.Type.Value)
                                            && (!search.BrandId.HasValue || e.BrandId == search.BrandId.Value)
                                            && (!search.StoreId.HasValue || e.StoreId == search.StoreId.Value)
                                            && (!search.UserId.HasValue || e.UserId == search.UserId.Value)
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
                                                         case GenericOrder.OrderByCreateDate:
                                                         default:
                                                             return e.OrderByDescending(o => o.CreatedDate);

                                                     }
                                                 }
                                                
                                             });

            var models =  data.Join(_customerRepo.GetAll(),o=>o.UserId,i=>i.Id,(o,i)=>new {UA=o,U=i})
                           .GroupJoin(_storeRepo.GetAll(),o=>o.UA.StoreId,i=>i.Id,(o,i)=>new {UA=o.UA,U=o.U,S=i.FirstOrDefault()})
                           .GroupJoin(_brandRep.GetAll(),o=>o.UA.BrandId,i=>i.Id,(o,i)=>new {UA=o.UA,U=o.U,S=o.S,B=i.FirstOrDefault()})
                           .ToList()
                           .Select(o=>new UserAuthViewModel(){
                             Id = o.UA.Id
                             , BrandId = o.UA.BrandId
                             , StoreId = o.UA.StoreId
                             , Type = o.UA.Type
                             , BrandName = o.B==null?"所有":o.B.Name
                             , UserId = o.UA.UserId
                             ,UserNick = o.U.Nickname
                             , StoreName = o.S == null ? "所有" : o.S.Name
                             ,Status = o.UA.Status.Value
                           });
                              

            return View("List", new Pager<UserAuthViewModel>(request, totalCount) { Data=models.ToList()});
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
            var entity = _authRepo.Find(id.Value);
            if (entity == null)
            {
                throw new ApplicationException("entity not exists!");
            }

            return View(new UserAuthViewModel().FromEntity<UserAuthViewModel>(entity));
        }

        [HttpPost]
        public ActionResult Create(FormCollection formCollection, UserAuthViewModel vo)
        {
            if (ModelState.IsValid)
            {
                if (vo.Type == (int)AuthDataType.Promotion)
                    vo.BrandId = 0;
                var entity = vo.ToEntity<UserAuthEntity>();
                entity.CreatedUser = base.CurrentUser.CustomerId;
                entity.CreatedDate = DateTime.Now;
                entity.Status = (int)DataStatus.Normal;
                _authRepo.Insert(entity);
                return RedirectToAction("Edit", new { Id = entity.Id});
            }

            return View(vo);
        }

        [HttpPost]
        public ActionResult Edit(FormCollection formCollection, UserAuthViewModel vo)
        {
            if (ModelState.IsValid)
            {
                if (vo.Type == (int)AuthDataType.Promotion)
                    vo.BrandId = 0;
                var entity = _authRepo.Find(vo.Id);
                if (entity == null)
                    throw new ApplicationException("entity not exists!");
                entity.Status = vo.Status;
                entity.Type = vo.Type;
                entity.UserId = vo.UserId;
                entity.StoreId = vo.StoreId;
                entity.BrandId = vo.BrandId;
               
                _authRepo.Update(entity);
                return RedirectToAction("List");
            }
            return View(vo);

        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            var entity = _authRepo.Find(id);
            if (entity == null)
            {

                return FailResponse();
            }

            _authRepo.Delete(entity);
            return SuccessResponse();
        }
    }
}
