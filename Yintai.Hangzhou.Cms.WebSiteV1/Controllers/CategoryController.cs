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
    public class CategoryController:UserController
    {
        private ICategoryRepository _categoryRepo;
        private ICategoryMapRepository _catmapRepo;
       
       
        public CategoryController(ICategoryRepository categoryRepo,ICategoryMapRepository catmapRepo)
        {
            _categoryRepo = categoryRepo;
            _catmapRepo = catmapRepo;
        }
        [HttpPost]
        public JsonResult ListP(CategorySearchOption search, PagerRequest request)
        {
            int totalCount;
            var linq = Context.Set<CategoryEntity>().Where(p => (string.IsNullOrEmpty(search.Name) || p.Name.Contains(search.Name)) &&
                (!search.PId.HasValue || p.ExCatId == search.PId.Value) &&
                p.Status != (int)DataStatus.Deleted);
           
            var linq2 = linq.GroupJoin(Context.Set<CategoryMapEntity>().Where(u => u.Status != (int)DataStatus.Deleted),
                    o => o.ExCatId,
                    i => i.CatId,
                    (o, i) => new { O = o, C = i })
                 ;

            totalCount = linq2.Count();

            var skipCount = (request.PageIndex - 1) * request.PageSize;

            var linq3 = skipCount == 0 ? linq2.Take(request.PageSize) : linq2.Skip(skipCount).Take(request.PageSize);

            var vo = from l in linq3.ToList()
                     select new CategoryViewModel().FromEntity<CategoryViewModel>(l.O, p =>
                     {
                         p.ShowCategories = l.C.Select(c => new ShowCategoryViewModel() { 
                            ShowChannel = c.ShowChannel
                         }).ToList() ;
                     });

            var v = new Pager<CategoryViewModel>(request, totalCount) { Data = vo.ToList() };
            return Json(v);
        }
        public ActionResult List(CategorySearchOption search)
        {

            return View(search);
        }
        public ActionResult Map(int id)
        {
           var catEntity = Context.Set<CategoryEntity>().Where(c => c.ExCatId == id && c.Status != (int)DataStatus.Deleted).FirstOrDefault();
           if (catEntity == null)
               throw new ArgumentException("id");
           var model = new CategoryViewModel().FromEntity<CategoryViewModel>(catEntity, p => {
               p.ShowCategories = new List<ShowCategoryViewModel>();
               foreach (var cmap in Context.Set<CategoryMapEntity>().Where(cm => cm.CatId == catEntity.ExCatId && cm.Status != (int)DataStatus.Deleted))
               {
                   p.ShowCategories.Add(new ShowCategoryViewModel() { 
                     ShowChannel = cmap.ShowChannel,
                      ShowCategoryId = cmap.ChannelCatId,
                       ShowCategoryName = Context.Set<TagEntity>().Find(cmap.ChannelCatId).Name,
                       Id = cmap.Id

             
                   });
                       
               }
           });
           return View(model);
        }

        [HttpPost]
        public ActionResult Map(CategoryViewModel model)
        {
            if (!ModelState.IsValid) {
                return View(model);
            }
            foreach (var map in model.ShowCategories)
            {
                var entity = Context.Set<CategoryMapEntity>().Find(map.Id);
                if (entity == null && map.Status!=(int)DataStatus.Deleted)
                {
                    _catmapRepo.Insert(new CategoryMapEntity()
                    {
                        CatId = model.ExCatId,
                        ShowChannel = map.ShowChannel,
                         ChannelCatId= map.ShowCategoryId
                          
                    });
                }
                else if (entity!=null && map.Status!=(int)DataStatus.Deleted)
                {
                    entity.ChannelCatId = map.ShowCategoryId;
                    entity.UpdateDate = DateTime.Now;
                    _catmapRepo.Update(entity);
                }
                else if (entity != null)
                {
                    entity.Status = (int)DataStatus.Deleted;
                    entity.UpdateDate = DateTime.Now;
                    _catmapRepo.Update(entity);
                }
            }
            return View("map", new { id = model.ExCatId });
        }

        public ActionResult SearchP(string channel, int catId)
        {
            if (string.IsNullOrEmpty(channel) || catId==0)
            {
                throw new ArgumentException("param not correct");
            }
            var tag = Context.Set<TagEntity>().Find(catId);
            return Json(new { Name = tag.Name});
        }
    }
}