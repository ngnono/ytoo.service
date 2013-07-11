using CrystalDecisions.Shared;
using Newtonsoft.Json;
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
    public class CouponCodeReportController:UserController
    {
        private IPromotionRepository _proRepo;
        public CouponCodeReportController(IPromotionRepository proRepo)
        {
            _proRepo = proRepo;
        }

        public ActionResult Search()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Search(PagerRequest request, CouponUsageOption search)
        {

            if (!ModelState.IsValid)
            {
                ViewBag.SearchOptions = search;
                return View();
            }
            var prods = doSearch(search);
            var v = new Pager<CouponCodeUsageViewModel>(request, prods.Count()) { Data = prods.ToList() };
            ViewBag.SearchOptions = search;
            return View(v);
        }
        public ActionResult Download(string reportname, string option)
        {
            CouponUsageOption search = JsonConvert.DeserializeObject<CouponUsageOption>(option);
            if (null == search)
                throw new ArgumentNullException("option");
            return RenderReport(reportname, r =>
            {
                r.SetDataSource(doSearch(search).ToList());

            }, ExportFormatType.Excel);
        }
        private IEnumerable<CouponCodeUsageViewModel> doSearch(CouponUsageOption search)
        {
            var dbContext = _proRepo.Context;
            var clinq = from cl in dbContext.Set<CouponLogEntity>()
                        where cl.Type==(int)CouponType.Promotion
                        join sr in dbContext.Set<StoreRealEntity>() on cl.ConsumeStoreNo equals sr.StoreNo into cls
                        from csl in cls.DefaultIfEmpty()
                        select new { CL = cl, CS = csl };
            var linq= dbContext.Set<CouponHistoryEntity>()
                    .Join(dbContext.Set<PromotionEntity>(),o=>o.FromPromotion,i=>i.Id,(o,i)=>new {C=o,P=i})
                    .Join(dbContext.Set<UserEntity>(),o=>o.C.User_Id,i=>i.Id,(o,i)=>new {C=o.C,P=o.P,U=i})
                    .GroupJoin(clinq, o => o.C.CouponId, i => i.CL.Code, (o, i) => new { C = o.C, P = o.P, U = o.U, CLs = i })
                    .Where(s=>(!search.PromotionId.HasValue 
                                    || s.C.FromPromotion == search.PromotionId.Value) &&
                               (!search.CreateDateFrom.HasValue || s.C.CreatedDate>=search.CreateDateFrom.Value) &&
                               (!search.CreateDateTo.HasValue || s.C.CreatedDate <= search.CreateDateTo.Value));
            if (search.Status.HasValue)
            {
                switch (search.Status.Value)
                { 
                    case (int)CouponStatus.Expired:
                        linq = linq.Where(l => l.C.ValidEndDate <= DateTime.Now && l.C.Status == (int)CouponStatus.Normal);
                        break;
                    case (int)CouponStatus.Normal:
                        linq = linq.Where(l => l.C.Status == (int)CouponStatus.Normal && l.C.ValidEndDate > DateTime.Now);
                        break;
                    case (int)CouponStatus.Used:
                        linq = linq.Where(l => l.C.Status == (int)CouponStatus.Used);
                        break;
                    case (int)CouponStatus.Deleted:
                        linq = linq.Where(l => l.C.Status == (int)CouponStatus.Deleted);
                        break;
                }
            }
            linq = linq.OrderByDescending(s=>s.C.CreatedDate);
           

            var result = linq.ToList().Select(l => new CouponCodeUsageViewModel()
                {
                    Code = l.C.CouponId,
                     PromotionName = l.P.Name,
                     CustomerPhone = l.U.Mobile,
                      CustomerNick = l.U.Nickname,
                      PromotionDate = string.Format("{0}-{1}",l.P.StartDate,l.P.EndDate),
                      CreateDate = l.C.CreatedDate,
                    Logs = l.CLs.OrderByDescending(cl => cl.CL.CreateDate)
                             .Select(cl => new CouponLogViewModel().FromEntity<CouponLogViewModel>(cl.CL, p =>
                             {
                                 p.StoreName = cl.CS == null ? string.Empty : cl.CS.Name;
                             }))
                });
            return result;
        }
    }
}