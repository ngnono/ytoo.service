using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Yintai.Hangzhou.Cms.WebSiteCoreV1.Models;
using Yintai.Hangzhou.Cms.WebSiteCoreV1.Util;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Repository.Contract;
using Yintai.Hangzhou.Service.Contract;
using Yintai.Hangzhou.WebSupport.Mvc;

namespace Yintai.Hangzhou.Cms.WebSiteCoreV1.Controllers
{
    [AdminAuthorize]
    public class RoleController : UserController
    {
        private IRoleRepository _roleService;
        private IAdminAccessRightRepository _rightService;
        private ICustomerRepository _customerService;
        public RoleController(IRoleRepository roleService, IAdminAccessRightRepository rightService,ICustomerRepository customerService)
        {
            _roleService = roleService;
            _rightService = rightService;
            _customerService = customerService;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult List()
        {
            return View(_roleService.FindAll());
        }
        public ActionResult Edit(int? Id)
        {
            var entity =_roleService.Find(Id);
            ViewBag.Rights = _rightService.FindAll();

            return View(new RoleViewModel().FromEntity<RoleEntity,RoleViewModel>(entity));
        }
        public ActionResult AuthorizeUser()
        {
            return View(from user in _roleService.FindAllUsersHavingRoles()
                        select new UserRoleViewModel().FromEntity<UserEntity, UserRoleViewModel>(user));

        }
        public ActionResult EditAuthorize(int Id)
        {
            ViewBag.Roles = from role in _roleService.FindAll()
                            select new RoleViewModel().FromEntity<RoleEntity, RoleViewModel>(role);
            return View(new UserRoleViewModel().FromEntity<UserEntity,UserRoleViewModel>(_customerService.Find(Id)));
        }
        [HttpPost]
        public ActionResult EditAuthorize(int User, ImplicitRightViewModel roles)
        {
            _roleService.UpdateWithUserRelation(User, roles.RoleRightDisplay);
            ViewBag.Roles = from role in _roleService.FindAll()
                            select new RoleViewModel().FromEntity<RoleEntity, RoleViewModel>(role);
            ViewBag.IsUpdateSuccess = true;
            return View(new UserRoleViewModel().FromEntity<UserEntity,UserRoleViewModel>(_customerService.Find(User)));
        }
        public JsonResult DeleteAuthorize(int Id)
        {
            _roleService.DeleteRolesOfUserId(Id);
            return Json(new  { 
                 Success=true
            });
        }
        public ActionResult CreateAuthorize()
        {
            ViewBag.Roles = from role in _roleService.FindAll()
                            select new RoleViewModel().FromEntity<RoleEntity, RoleViewModel>(role);
            return View();
        }
        [HttpPost]
        public ActionResult CreateAuthorize(int User,ImplicitRightViewModel roles)
        {
            _roleService.InsertWithUserRelation(User, roles.RoleRightDisplay);
            ViewBag.Roles = from role in _roleService.FindAll()
                            select new RoleViewModel().FromEntity<RoleEntity, RoleViewModel>(role);
            return Redirect("AuthorizeUser");
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                _roleService.Insert(model.ToEntity<RoleViewModel, RoleEntity>());
                return RedirectToAction("List");
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(RoleViewModel entity)
        {
            if (ModelState.IsValid)
            {
               
                var newEntity =_roleService.UpdateWithRights(entity.ToEntity<RoleViewModel,RoleEntity>());
                ViewBag.Rights = _rightService.FindAll();
                ViewBag.IsUpdateSuccess = true;
                entity = entity.FromEntity<RoleEntity, RoleViewModel>(newEntity);
            }
            return View(entity);
        }

        public JsonResult Delete(int? Id)
        {
            var entity = _roleService.Find(Id);
            _roleService.Delete(entity);
            return Json(new { 
                Success = true
            });
        }
        public ActionResult RightList()
        {

            return View(_rightService.FindAll());
        }
        public ActionResult EditRight(int Id)
        { 
            return View(_rightService.Find(Id));
        }
        [HttpPost]
        public ActionResult EditRight(AdminAccessRightEntity model)
        {
            if (ModelState.IsValid)
            {
                _rightService.Update(model);
                ViewBag.IsUpdateSuccess = true;
            }
            return View(model);
        }
        public JsonResult DeleteRight(int Id)
        {
            _rightService.Delete(_rightService.Find(Id));
            return Json(new { 
                 Success = true
            });
        }
        public ActionResult ImportRight()
        {
            ViewBag.ImplicitRights = ReflectPublichActionsInCurrentDll();
            return View();
        }
        [HttpPost]
        public ActionResult ImportRight(ImplicitRightViewModel model)
        {
            if (model.RoleRightDisplay == null ||
                model.RoleRightDisplay.Length == 0)
            {
                ViewBag.IsUpdateSuccess = false;
            }
            else
            {
                var entity = from selected in model.RoleRightDisplay
                             let ar = selected.Split('\\')
                             select new AdminAccessRightEntity()
                             {
                                 ControllName = ar[0]
                                 ,
                                 ActionName = ar[1]
                                 ,
                                 InUser = CurrentUser.CustomerId
                             };
                _rightService.InsertIfNotPresent(entity);
                ViewBag.IsUpdateSucccess = true;
            }
            ViewBag.ImplicitRights = ReflectPublichActionsInCurrentDll();
            return View(model);
        }
        private IEnumerable<AdminAccessRightEntity> ReflectPublichActionsInCurrentDll()
        {
            int id = 0;
            foreach (var controller in Assembly.GetExecutingAssembly().GetTypes().Where(type => type.IsClass && typeof(UserController).IsAssignableFrom(type)))
            {
                foreach (var action in controller.GetMethods(BindingFlags.Public|BindingFlags.IgnoreCase|BindingFlags.Instance)
                                        .Where(method=>!method.IsSpecialName&&
                                              typeof(ActionResult).IsAssignableFrom(method.ReturnType))
                                        .Distinct(new MethodInfoComparer()))
                {
                    if (controller.GetCustomAttribute<AdminAuthorizeAttribute>() != null ||
                        action.GetCustomAttribute<AdminAuthorizeAttribute>(false) != null)
                        yield return new AdminAccessRightEntity()
                        {
                            Id = id++
                            ,
                            ControllName = controller.Name.Replace("Controller", string.Empty)
                            ,
                            ActionName = action.Name
                        };
                    else
                        continue;
                }
            }
            yield break;
        }

        private class MethodInfoComparer : EqualityComparer<MethodInfo>
        {

            public override bool Equals(MethodInfo b1, MethodInfo b2)
            {
                if (string.Compare(b1.Name, b2.Name, true) == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }


            public override int GetHashCode(MethodInfo b1)
            {
                return b1.Name.GetHashCode();
            }

        }
    }

    
}
