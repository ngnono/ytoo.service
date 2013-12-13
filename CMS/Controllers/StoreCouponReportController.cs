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
    public class StoreCouponReportController : UserController
    {
        private IStoreCouponsRepository _storecouponRepo;
        public StoreCouponReportController(IStoreCouponsRepository storecouponRepo)
        {
            _storecouponRepo = storecouponRepo;
        }

        public ActionResult Search()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Search(PagerRequest request, StoreCouponSequenceOption search)
        {

            if (!ModelState.IsValid)
            {
                ViewBag.SearchOptions = search;
                return View();
            }
            var prods = doSearch(search);
            var v = new Pager<StoreCouponSequenceViewModel>(request, prods.Count()) { Data = prods.ToList() };
            ViewBag.SearchOptions = search;
            return View(v);
        }
        public ActionResult Download(string reportname, string option)
        {
            StoreCouponSequenceOption search = JsonConvert.DeserializeObject<StoreCouponSequenceOption>(option);
            if (null == search)
                throw new ArgumentNullException("option");
            return RenderReport(reportname, r =>
            {
                r.SetDataSource(doSearch(search).ToList());

            }, ExportFormatType.Excel);
        }
        private IEnumerable<StoreCouponSequenceViewModel> doSearch(StoreCouponSequenceOption search)
        {
            var dbContext = _storecouponRepo.Context;
            var clinq = from cl in dbContext.Set<CouponLogEntity>()
                        join sr in dbContext.Set<StoreRealEntity>() on cl.ConsumeStoreNo equals sr.StoreNo into cls
                        from csl in cls.DefaultIfEmpty()
                        select new {CL = cl,CS = csl};
            var linq= dbContext.Set<StoreCouponEntity>()
                    .Join(dbContext.Set<StorePromotionEntity>(),o=>o.StorePromotionId,i=>i.Id,(o,i)=>new {SC=o,SP=i})
                    .GroupJoin(clinq
                            ,o=>o.SC.Code,i=>i.CL.Code,(o,i)=>new {SC = o.SC,SP=o.SP,CL = i})
                    .Where(s=>(string.IsNullOrEmpty(search.PromotionName) 
                                    || s.SP.Name.StartsWith(search.PromotionName)) &&
                               (!search.CreateDateFrom.HasValue || s.SC.CreateDate>=search.CreateDateFrom.Value) &&
                               (!search.CreateDateTo.HasValue || s.SC.CreateDate <= search.CreateDateTo.Value) &&
                               (!search.PointFrom.HasValue || s.SC.Points>=search.PointFrom.Value) &&
                               (!search.PointTo.HasValue || s.SC.Points < search.PointTo.Value) &&
                               (!search.CustomerId.HasValue || s.SC.UserId == search.CustomerId.Value) &&
                               (string.IsNullOrEmpty(search.StoreNo) || s.CL.Any(c=>c.CL.ConsumeStoreNo==search.StoreNo)) &&
                               (!search.ConsumeDateFrom.HasValue || s.CL.Any(c=>c.CL.CreateDate>=search.ConsumeDateFrom.Value && c.CL.ActionType==(int)CouponActionType.Consume)) &&
                               (!search.ConsumeDateTo.HasValue || s.CL.Any(c=>c.CL.CreateDate<search.ConsumeDateTo.Value && c.CL.ActionType ==(int)CouponActionType.Consume)) &&
                               (string.IsNullOrEmpty(search.ReceiptNo) || s.CL.Any(c=>c.CL.ReceiptNo==search.ReceiptNo)));
            if (search.Status.HasValue)
            {
                switch(search.Status.Value)
                {
                    case StoreCouponSequenceStatus.CreateNotConsume:
                        linq = linq.Where(l=>l.SC.Status == (int)CouponStatus.Normal && l.SC.ValidEndDate>DateTime.Now);
                        break;
                    case StoreCouponSequenceStatus.Consumed:
                        linq = linq.Where(l=>l.SC.Status == (int)CouponStatus.Used);
                        break;
                    case StoreCouponSequenceStatus.Expired:
                        linq = linq.Where(l=>l.SC.Status == (int)CouponStatus.Normal && l.SC.ValidEndDate<=DateTime.Now);
                        break;
                    case StoreCouponSequenceStatus.Rebated:
                           linq = linq.Where(l=>l.SC.Status == (int)CouponStatus.Normal 
                                                && l.SC.ValidEndDate>DateTime.Now
                                                && l.CL.Any()
                                                && l.CL.OrderByDescending(cl=>cl.CL.CreateDate).FirstOrDefault().CL.ActionType == (int)CouponActionType.Rebate);
                        break;
                    case StoreCouponSequenceStatus.Void:
                         linq = linq.Where(l=>l.SC.Status == (int)CouponStatus.Deleted);
                        break;
                }

            }
            var sort = search.Sort.HasValue?search.Sort.Value:StoreCouponSequenceSort.ByCreateDate;
            if (search.Sort.HasValue)
            {
               switch(sort)
               {
                   case StoreCouponSequenceSort.ByCreateDate:
                      linq= linq.OrderByDescending(l=>l.SC.CreateDate);
                       break;
                   case StoreCouponSequenceSort.ByAmount:
                     linq=  linq.OrderByDescending(l=>l.SC.Amount);
                       break;
                   case StoreCouponSequenceSort.ByStatus:
                       linq = linq.OrderByDescending(l=>l.SC.Status);
                       break;
               }
            }

            var result = linq.ToList().Select(l => new StoreCouponSequenceViewModel()
                {
                    Code = l.SC.Code,
                     Amount = l.SC.Amount.Value,
                     CustomerId = l.SC.UserId.Value,
                     CreateDate = l.SC.CreateDate.Value,
                      RawStatus =l.SC.Status.Value,
                       UpdateDate = l.SC.UpdateDate.Value,
                        PromotionName = l.SP.Name,
                        ValidEndDate = l.SC.ValidEndDate.Value,
                         Logs = l.CL.OrderByDescending(cl=>cl.CL.CreateDate)
                                .Select(cl=>new CouponLogViewModel().FromEntity<CouponLogViewModel>(cl.CL,p=>{
                                    p.StoreName = cl.CS==null?string.Empty: cl.CS.Name;
                                }))
                });
            return result;
        }
    }
}