using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
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
    public class PropertyValueController:UserController
    {
        private ITagRepository _tagRepo;
        private ICategoryPropertyRepository _tagpropertyRepo;
        private ICategoryPropertyValueRepository _tagpropertyvalueRepo;
        public PropertyValueController(ITagRepository tagRepo,
            ICategoryPropertyRepository tagpropertyRepo,
            ICategoryPropertyValueRepository tagpropertyvalRepo)
        {
            _tagRepo = tagRepo;
            _tagpropertyRepo = tagpropertyRepo;
            _tagpropertyvalueRepo = tagpropertyvalRepo;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult List(PagerRequest request,TagPropertyValueSearchOption search)
        {
            int totalCount;
            var linq = _tagpropertyvalueRepo.Get(pv => pv.Status != (int)DataStatus.Deleted)
                        .Join(_tagpropertyRepo.Get(p => p.Status != (int)DataStatus.Deleted), o => o.PropertyId, i => i.Id, (o, i) => new { PV = o, P = i })
                        .Join(_tagRepo.GetAll(), o => o.P.CategoryId, i => i.Id, (o, i) => new { PV = o.PV, P = o.P, T = i })
                        .Where(l => (!search.PId.HasValue || l.PV.Id == search.PId.Value) &&
                                   (string.IsNullOrEmpty(search.PropertyDesc) || l.P.PropertyDesc.StartsWith(search.PropertyDesc)) &&
                                   (string.IsNullOrEmpty(search.ValueDesc) || l.PV.ValueDesc.StartsWith(search.ValueDesc)) &&
                                   (!search.CategoryId.HasValue || l.P.CategoryId == search.CategoryId.Value));
            linq = linq.WhereWithPageSort(
                                                 out totalCount
                                                , request.PageIndex
                                                , request.PageSize
                                                , e =>
                                                {
                                                    return e.OrderByDescending(l => l.P.CategoryId).ThenByDescending(l => l.P.CreatedDate);
                                                });
             var vo = new List<TagPropertyViewModel>();
            foreach (var l in linq)
            {
                var existProperty = vo.Find(t=>t.CategoryId == l.P.CategoryId);
                var newValue = new TagPropertyValueViewModel().FromEntity<TagPropertyValueViewModel>(l.PV,p=>{
                    p.PropertyDesc = l.P.PropertyDesc;
                    p.SortOrder = l.P.SortOrder;
                    p.PropertyId = l.P.Id;
                    p.ValueId = l.PV.Id;
                   
                });
                if (existProperty != null)
                {
                    existProperty.Values.Add(newValue);
                    
                } else
                {
                    vo.Add(new TagPropertyViewModel().FromEntity<TagPropertyViewModel>(l.P,p=>{
  
                        p.CategoryName = l.T.Name;
                        p.Values = new List<TagPropertyValueViewModel>();
                        p.Values.Add(newValue);
                    }));
                }
    
            }
              
            var v = new Pager<TagPropertyViewModel>(request, totalCount) { Data =vo };

            return View("List", v);
        }

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(FormCollection formCollection, TagPropertyViewModel vo)
        {
            if (!ModelState.IsValid)
            {
                return View(vo);
            }
            var propertyExist = _tagpropertyRepo.Get(t => t.CategoryId == vo.CategoryId && t.Status != (int)DataStatus.Deleted).Any();
            if (propertyExist)
            {
                ModelState.AddModelError("", "分类已经设置属性，使用修改功能来更新！");
                return View(vo);
            }
            using (var ts = new TransactionScope())
            {
                string propertyflag = null;
                CategoryPropertyEntity newProperty = null;
                foreach (var pv in vo.Values.OrderBy(v => v.PropertyDesc))
                {
                    if (propertyflag != pv.PropertyDesc)
                    {
                        propertyflag = pv.PropertyDesc;
                        newProperty = _tagpropertyRepo.Insert(new CategoryPropertyEntity()
                        {
                            CreatedDate = DateTime.Now,
                            CategoryId = vo.CategoryId,
                            CreatedUser = CurrentUser.CustomerId,
                            IsVisible = true,
                            PropertyDesc = pv.PropertyDesc,
                            Status = (int)DataStatus.Normal,
                            SortOrder = 0,
                            UpdatedDate = DateTime.Now,
                            UpdatedUser = CurrentUser.CustomerId
                        });
                    }
                    _tagpropertyvalueRepo.Insert(new CategoryPropertyValueEntity()
                    {
                        CreateDate = DateTime.Now,
                        CreateUser = CurrentUser.CustomerId,
                        PropertyId = newProperty.Id,
                        SortOrder = 0,
                        Status = (int)DataStatus.Normal,
                        UpdateDate = DateTime.Now,
                        UpdateUser = CurrentUser.CustomerId,
                        ValueDesc = pv.ValueDesc

                    });

                }
                ts.Complete();
            }

            return RedirectToAction("Edit", new { id = vo.CategoryId });

        }

        public ActionResult Edit(int id)
        {
            var entity = _tagpropertyvalueRepo.Get(pv => pv.Status != (int)DataStatus.Deleted)
                        .Join(_tagpropertyRepo.Get(p => p.Status != (int)DataStatus.Deleted && p.CategoryId == id), o => o.PropertyId, i => i.Id, (o, i) => new { PV = o, P = i })
                        .Join(_tagRepo.GetAll(), o => o.P.CategoryId, i => i.Id, (o, i) => new { PV = o.PV, P = o.P, T = i });
            TagPropertyViewModel vo = new TagPropertyViewModel();
            vo.Values = new List<TagPropertyValueViewModel>();
            foreach (var l in entity)
            {
                vo.CategoryId = l.P.CategoryId;
                vo.CategoryName = l.T.Name;
                var newValue = new TagPropertyValueViewModel().FromEntity<TagPropertyValueViewModel>(l.PV, p =>
                {
                    p.PropertyDesc = l.P.PropertyDesc;
                    p.SortOrder = l.P.SortOrder;
                    p.PropertyId = l.P.Id;
                    p.ValueId = l.PV.Id;
                });
                vo.Values.Add(newValue);
            }

            return View(vo);
        }

      

        [HttpPost]
        public ActionResult Edit(FormCollection formCollection, TagPropertyViewModel vo)
        {
            if (!ModelState.IsValid)
            {
                return View(vo);
            }

            using (var ts = new TransactionScope())
            {
                //step1: find the properties to delete
                IEnumerable<string> newProperty = vo.Values.Where(o=>o.Status!=(int)DataStatus.Deleted).Select(o => o.PropertyDesc).Distinct();
                IQueryable<CategoryPropertyEntity> catProperties = _tagpropertyRepo.Get(t => t.CategoryId == vo.CategoryId && t.Status != (int)DataStatus.Deleted);
                IEnumerable<string> existProperty = catProperties.Select(o => o.PropertyDesc);
                IEnumerable<string> toDeleted = existProperty.Except(newProperty);

                foreach (var diff in toDeleted)
                {
                    var todeleteProperty = catProperties.Where(p => p.PropertyDesc == diff).FirstOrDefault();
                    if (todeleteProperty != null)
                    {
                        todeleteProperty.Status = (int)DataStatus.Deleted;
                        todeleteProperty.UpdatedDate = DateTime.Now;
                        _tagpropertyRepo.Update(todeleteProperty);
                        //cascad delete values
                        foreach (var deleteValue in _tagpropertyvalueRepo.Get(t => t.PropertyId == todeleteProperty.Id && t.Status != (int)DataStatus.Deleted))
                        {
                            deleteValue.Status = (int)DataStatus.Deleted;
                            deleteValue.UpdateDate = DateTime.Now;
                            _tagpropertyvalueRepo.Update(deleteValue);
                        }
                    }
                }
                // step2: find the properties to insert
                foreach(var diff in newProperty.Except(existProperty))
                {
                    var insertProperty = _tagpropertyRepo.Insert(new CategoryPropertyEntity(){
                         CategoryId = vo.CategoryId,
                          CreatedDate = DateTime.Now,
                           CreatedUser = CurrentUser.CustomerId,
                            IsVisible = true,
                             PropertyDesc = diff,
                              SortOrder =0,
                               Status=(int)DataStatus.Normal,
                                UpdatedDate = DateTime.Now,
                                UpdatedUser =CurrentUser.CustomerId

                    });
                    //insert values
                    foreach(var insertValue in vo.Values.Where(o=>o.PropertyDesc == diff && o.Status!=(int)DataStatus.Deleted))
                    {
                        _tagpropertyvalueRepo.Insert(new CategoryPropertyValueEntity(){
                             CreateDate = DateTime.Now,
                              CreateUser = CurrentUser.CustomerId,
                               PropertyId = insertProperty.Id,
                                SortOrder =0,
                                 Status = (int)DataStatus.Normal,
                                  UpdateDate = DateTime.Now,
                                   UpdateUser = CurrentUser.CustomerId,
                                    ValueDesc = insertValue.ValueDesc
                        });
                    }
                }
                //step3: update exist properties
                foreach(var diff in newProperty.Intersect(existProperty))
                {
                    var propertyEntity = catProperties.Where(p => p.PropertyDesc == diff).FirstOrDefault();
                    IEnumerable<string> newValue = vo.Values.Where(o=>o.Status!=(int)DataStatus.Deleted && o.PropertyDesc==diff).Select(o => o.ValueDesc).Distinct();
                    IQueryable<CategoryPropertyValueEntity> catPropertyValues = _tagpropertyvalueRepo.Get(t => t.PropertyId == propertyEntity.Id && t.Status != (int)DataStatus.Deleted);
                     IEnumerable<string> existValue = catPropertyValues.Select(o => o.ValueDesc);
                    foreach(var diffvalue in existValue.Except(newValue))
                    {
                        var todeletevalue = catPropertyValues.Where(o=>o.ValueDesc == diffvalue).FirstOrDefault();
                        todeletevalue.Status = (int)DataStatus.Deleted;
                        todeletevalue.UpdateDate = DateTime.Now;
                        _tagpropertyvalueRepo.Update(todeletevalue);
                    }
                    foreach(var diffvalue in newValue.Except(existValue))
                    {
                        _tagpropertyvalueRepo.Insert(new CategoryPropertyValueEntity(){
                             CreateDate = DateTime.Now,
                              UpdateDate = DateTime.Now,
                               Status =(int)DataStatus.Normal,
                                ValueDesc = diffvalue,
                                 PropertyId = propertyEntity.Id,
                                  UpdateUser = CurrentUser.CustomerId,
                                   SortOrder = 0,
                                    CreateUser = CurrentUser.CustomerId
                        });
                    }
                }
                ts.Complete();

            }
           
            return RedirectToAction("list");
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {

            var entity = _tagpropertyvalueRepo.Find(id);

            entity.UpdateDate = DateTime.Now;
            entity.UpdateUser = CurrentUser.CustomerId;
            entity.Status = (int)DataStatus.Deleted;

            _tagpropertyvalueRepo.Update(entity);

            return SuccessResponse();
        }


    }
}