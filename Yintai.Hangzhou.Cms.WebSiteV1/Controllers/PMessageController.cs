using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.Cms.WebSiteV1.Models;
using Yintai.Hangzhou.Cms.WebSiteV1.Util;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Repository.Contract;

namespace Yintai.Hangzhou.Cms.WebSiteV1.Controllers
{
    [AdminAuthorize]
    public class PMessageController:UserController
    {
        private IPMessageRepository _pmessageRepo;
        public PMessageController(IPMessageRepository pmessageRepo)
        {
            _pmessageRepo = pmessageRepo;
        }
        public ActionResult List()
        {
            return View();
        }
        [HttpPost]
        public ActionResult List(PMessageSearchOption search, PagerRequest request)
        {
            return View(search);
        }
        [HttpPost]
        public JsonResult ListP(PMessageSearchOption search, PagerRequest request)
        {
            int totalCount;

            var linq = Context.Set<PMessageEntity>().Where(p => (!search.FromDate.HasValue || p.CreateDate>=search.FromDate.Value) &&
                (!search.ToDate.HasValue || p.CreateDate == search.ToDate.Value));
            if ((CurrentUser.Role & (int)UserRole.Admin) != (int)UserRole.Admin)
            {
                linq = linq.Where(l => l.FromUser == CurrentUser.CustomerId || l.ToUser == CurrentUser.CustomerId);
            }
            var linq2 = linq.GroupBy(l => new { F = l.FromUser, T = l.ToUser })
                                      .Select(l => new { F = l.Key.F, T = l.Key.T, MsgID = l.Max(m => m.Id) });
            var linq3 = linq2.Where(l=>!linq2.Any(l2=>l2.F==l.T && l2.T == l.F && l2.MsgID>l.MsgID))
                        .Join(Context.Set<PMessageEntity>(),o=>o.MsgID,i=>i.Id,(o,i)=>new {F=o.F,T=o.T,MsgID=o.MsgID,P=i})
                        .Join(Context.Set<UserEntity>(), o => o.F, i => i.Id, (o, i) => new { P = o, FromUser = i })
                        .Join(Context.Set<UserEntity>(), o => o.P.T, i => i.Id, (o, i) => new { P = o.P, FromUser = o.FromUser, ToUser = i})
                        .OrderByDescending(l=>l.P.MsgID);
                       
            totalCount = linq2.Count();

            var skipCount = (request.PageIndex - 1) * request.PageSize;

            var linq4 = skipCount == 0 ? linq2.Take(request.PageSize) : linq2.Skip(skipCount).Take(request.PageSize);


            var vo = from l in linq3.ToList()
                     select new PMessageViewModel().FromEntity<PMessageViewModel>(l.P.P, p =>
                     {
                         p.FromUser = l.P.F;
                         p.ToUser = l.P.T;
                         p.FromUserModel = new CustomerViewModel().FromEntity<CustomerViewModel>(l.FromUser);
                         p.ToUserModel = new CustomerViewModel().FromEntity<CustomerViewModel>(l.ToUser);

                     });

            var v = new Pager<PMessageViewModel>(request, totalCount) { Data = vo.ToList() };
            return Json(v);
        }

        public ActionResult Reply(int FromUser, int ToUser)
        {
            if (!CurrentUser.IsAdmin)
            {
                if (CurrentUser.CustomerId != FromUser && CurrentUser.CustomerId != ToUser)
                {
                    throw new ArgumentException("not authorized");
                }
            }
            var linq = Context.Set<PMessageEntity>()
                .Where(l => (l.FromUser == FromUser && l.ToUser == ToUser) ||
                        (l.FromUser == ToUser && l.ToUser == FromUser))
                .Join(Context.Set<UserEntity>(),o=>o.FromUser,i=>i.Id,(o,i)=>new {P=o,FromUser=i})
                .Join(Context.Set<UserEntity>(),o=>o.P.ToUser,i=>i.Id,(o,i)=>new {P=o.P,FromUser=o.FromUser,ToUser=i})
                .OrderByDescending(l => l.P.Id)
                .ToList()
                .Select(l=>new PMessageViewModel().FromEntity<PMessageViewModel>(l.P,p=>{
                    p.FromUserModel = new CustomerViewModel().FromEntity<CustomerViewModel>(l.FromUser);
                    p.ToUserModel = new CustomerViewModel().FromEntity<CustomerViewModel>(l.ToUser);
                }));

            ViewBag.RecentMsg = linq;
            return View(new PMessageViewModel() { 
                 FromUser = FromUser,
                  ToUser = ToUser
            });
        }
        [HttpPost]
        public ActionResult Reply(PMessageViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var fromUser = CurrentUser.CustomerId;
            var toUser = model.FromUser==fromUser?model.ToUser:model.FromUser;
            _pmessageRepo.Insert(new PMessageEntity() { 
                 FromUser = fromUser,
                 ToUser = toUser,
                  CreateDate = DateTime.Now,
                   IsAuto = false,
                    IsVoice = false,
                     TextMsg = model.TextMsg
            });
            return RedirectToAction("reply", new { FromUser = model.FromUser, ToUser = model.ToUser });
        }


    }
}