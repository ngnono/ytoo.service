using System;
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
    public class ProductPropertyValueController:UserController
    {
       private ITagRepository _tagRepo;
        private ICategoryPropertyRepository _tagpropertyRepo;
        private ICategoryPropertyValueRepository _tagpropertyvalueRepo;
        private IProductPropertyRepository _prodpropertyRepo;
        private IProductPropertyValueRepository _prodpropertyvalRepo;
        private IProductRepository _prodRepo;
        private IUserAuthRepository _userauthRepo;
        public ProductPropertyValueController(ITagRepository tagRepo,
            ICategoryPropertyRepository tagpropertyRepo,
            ICategoryPropertyValueRepository tagpropertyvalRepo,
            IProductPropertyValueRepository prodpropertyvalRepo,
            IProductPropertyRepository prodpropertyRepo,
            IProductRepository prodRepo,
            IUserAuthRepository userauthRepo
        )
        {
            _tagRepo = tagRepo;
            _tagpropertyRepo = tagpropertyRepo;
            _tagpropertyvalueRepo = tagpropertyvalRepo;
            _prodpropertyRepo = prodpropertyRepo;
            _prodpropertyvalRepo = prodpropertyvalRepo;
            _prodRepo = prodRepo;
            _userauthRepo = userauthRepo;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult List(PagerRequest request,ProductPropertyValueSearchOption search)
        {
            int totalCount;
            var prodLinq = _userauthRepo.AuthFilter(_prodRepo.GetAll(), CurrentUser.CustomerId, CurrentUser.Role) as IQueryable<ProductEntity>;
            var linq = _prodpropertyvalRepo.Get(pv => pv.Status != (int)DataStatus.Deleted)
                        .Join(_prodpropertyRepo.Get(p => p.Status != (int)DataStatus.Deleted), o => o.PropertyId, i => i.Id, (o, i) => new { PV = o, P = i })
                        .Join(prodLinq, o => o.P.ProductId, i => i.Id, (o, i) => new { PV = o.PV, P = o.P, T = i })
                        .Where(l => (string.IsNullOrEmpty(search.PropertyDesc) || l.P.PropertyDesc.StartsWith(search.PropertyDesc)) &&
                                   (string.IsNullOrEmpty(search.ValueDesc) || l.PV.ValueDesc.StartsWith(search.ValueDesc)) &&
                                   (!search.ProductId.HasValue || l.P.ProductId == search.ProductId.Value));
            linq = linq.WhereWithPageSort(
                                                 out totalCount
                                                , request.PageIndex
                                                , request.PageSize
                                                , e =>
                                                {
                                                    return e.OrderByDescending(l => l.P.ProductId).ThenByDescending(l => l.P.UpdateDate);
                                                });
             var vo = new List<ProductPropertyViewModel>();
            foreach (var l in linq)
            {
                var existProperty = vo.Find(t => t.ProductId == l.P.ProductId);
                var newValue = new TagPropertyValueViewModel().FromEntity<TagPropertyValueViewModel>(l.PV, p =>
                {
                    p.PropertyDesc = l.P.PropertyDesc;
                    p.SortOrder = l.P.SortOrder??0;
                    p.PropertyId = l.P.Id;
                    p.ValueId = l.PV.Id;
                   
                });
                if (existProperty != null)
                {
                    existProperty.Values.Add(newValue);
                    
                } else
                {
                    vo.Add(new ProductPropertyViewModel().FromEntity<ProductPropertyViewModel>(l.P, p =>
                    {
  
                        p.ProductName = l.T.Name;
                        p.Values = new List<TagPropertyValueViewModel>();
                        p.Values.Add(newValue);
                    }));
                }
    
            }
              
            var v = new Pager<ProductPropertyViewModel>(request, totalCount) { Data =vo };

            return View("List", v);
        }

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(FormCollection formCollection, ProductPropertyViewModel vo)
        {
            if (!ModelState.IsValid)
            {
                return View(vo);
            }
            var propertyExist = _prodpropertyRepo.Get(t => t.ProductId == vo.ProductId && t.Status != (int)DataStatus.Deleted).Any();
            if (propertyExist)
            {
                ModelState.AddModelError("", "商品已经设置属性，使用修改功能来更新！");
                return View(vo);
            }
            using (var ts = new TransactionScope())
            {
                string propertyflag = null;
                ProductPropertyEntity newProperty = null;
                foreach (var pv in vo.Values.OrderBy(v => v.PropertyDesc))
                {
                    if (propertyflag != pv.PropertyDesc)
                    {
                        propertyflag = pv.PropertyDesc;
                        newProperty = _prodpropertyRepo.Insert(new ProductPropertyEntity()
                        {
                            ProductId = vo.ProductId,
                            PropertyDesc = pv.PropertyDesc,
                            Status = (int)DataStatus.Normal,
                            SortOrder = 0,
                            UpdateDate = DateTime.Now,
                            UpdateUser = CurrentUser.CustomerId
                        });
                    }
                    _prodpropertyvalRepo.Insert(new ProductPropertyValueEntity()
                    {
                        CreateDate = DateTime.Now,
                        PropertyId = newProperty.Id,
                        Status = (int)DataStatus.Normal,
                        UpdateDate = DateTime.Now,
                        ValueDesc = pv.ValueDesc

                    });

                }
                ts.Complete();
            }

            return RedirectToAction("Edit", new { id = vo.ProductId });

        }

        public ActionResult Edit(int id)
        {
            var entity = _prodpropertyvalRepo.Get(pv => pv.Status != (int)DataStatus.Deleted)
                        .Join(_prodpropertyRepo.Get(p => p.Status != (int)DataStatus.Deleted && p.ProductId == id), o => o.PropertyId, i => i.Id, (o, i) => new { PV = o, P = i })
                        .Join(_prodRepo.GetAll(), o => o.P.ProductId, i => i.Id, (o, i) => new { PV = o.PV, P = o.P, T = i });
            ProductPropertyViewModel vo = new ProductPropertyViewModel();
            vo.Values = new List<TagPropertyValueViewModel>();
            foreach (var l in entity)
            {
                vo.ProductId = l.P.ProductId;
                vo.ProductName = l.T.Name;
                var newValue = new TagPropertyValueViewModel().FromEntity<TagPropertyValueViewModel>(l.PV, p =>
                {
                    p.PropertyDesc = l.P.PropertyDesc;
                    p.SortOrder = l.P.SortOrder??0;
                    p.PropertyId = l.P.Id;
                    p.ValueId = l.PV.Id;
                });
                vo.Values.Add(newValue);
            }

            return View(vo);
        }

      

        [HttpPost]
        public ActionResult Edit(FormCollection formCollection, ProductPropertyViewModel vo)
        {
            if (!ModelState.IsValid)
            {
                return View(vo);
            }

            using (var ts = new TransactionScope())
            {
                //step1: find the properties to delete
                IEnumerable<string> newProperty = vo.Values.Where(o=>o.Status!=(int)DataStatus.Deleted).Select(o => o.PropertyDesc).Distinct();
                IQueryable<ProductPropertyEntity> catProperties = _prodpropertyRepo.Get(t => t.ProductId == vo.ProductId && t.Status != (int)DataStatus.Deleted);
                IEnumerable<string> existProperty = catProperties.Select(o => o.PropertyDesc);
                IEnumerable<string> toDeleted = existProperty.Except(newProperty);

                foreach (var diff in toDeleted)
                {
                    var todeleteProperty = catProperties.Where(p => p.PropertyDesc == diff).FirstOrDefault();
                    if (todeleteProperty != null)
                    {
                        todeleteProperty.Status = (int)DataStatus.Deleted;
                        todeleteProperty.UpdateDate = DateTime.Now;
                        _prodpropertyRepo.Update(todeleteProperty);
                        //cascad delete values
                        foreach (var deleteValue in _prodpropertyvalRepo.Get(t => t.PropertyId == todeleteProperty.Id && t.Status != (int)DataStatus.Deleted))
                        {
                            deleteValue.Status = (int)DataStatus.Deleted;
                            deleteValue.UpdateDate = DateTime.Now;
                            _prodpropertyvalRepo.Update(deleteValue);
                        }
                    }
                }
                // step2: find the properties to insert
                foreach(var diff in newProperty.Except(existProperty))
                {
                    var insertProperty = _prodpropertyRepo.Insert(new ProductPropertyEntity(){
                         ProductId = vo.ProductId,
                             PropertyDesc = diff,
                              SortOrder =0,
                               Status=(int)DataStatus.Normal,
                                UpdateDate = DateTime.Now,
                                UpdateUser =CurrentUser.CustomerId

                    });
                    //insert values
                    foreach(var insertValue in vo.Values.Where(o=>o.PropertyDesc == diff && o.Status!=(int)DataStatus.Deleted))
                    {
                        _prodpropertyvalRepo.Insert(new ProductPropertyValueEntity(){
                             CreateDate = DateTime.Now,
                               PropertyId = insertProperty.Id,
                                 Status = (int)DataStatus.Normal,
                                  UpdateDate = DateTime.Now,
                                    ValueDesc = insertValue.ValueDesc
                        });
                    }
                }
                //step3: update exist properties
                foreach(var diff in newProperty.Intersect(existProperty))
                {
                    var propertyEntity = catProperties.Where(p => p.PropertyDesc == diff).FirstOrDefault();
                    IEnumerable<string> newValue = vo.Values.Where(o=>o.Status!=(int)DataStatus.Deleted && o.PropertyDesc==diff).Select(o => o.ValueDesc).Distinct();
                    IQueryable<ProductPropertyValueEntity> catPropertyValues = _prodpropertyvalRepo.Get(t => t.PropertyId == propertyEntity.Id && t.Status != (int)DataStatus.Deleted);
                     IEnumerable<string> existValue = catPropertyValues.Select(o => o.ValueDesc);
                    foreach(var diffvalue in existValue.Except(newValue))
                    {
                        var todeletevalue = catPropertyValues.Where(o=>o.ValueDesc == diffvalue).FirstOrDefault();
                        todeletevalue.Status = (int)DataStatus.Deleted;
                        todeletevalue.UpdateDate = DateTime.Now;
                        _prodpropertyvalRepo.Update(todeletevalue);
                    }
                    foreach(var diffvalue in newValue.Except(existValue))
                    {
                        _prodpropertyvalRepo.Insert(new ProductPropertyValueEntity(){
                             CreateDate = DateTime.Now,
                              UpdateDate = DateTime.Now,
                               Status =(int)DataStatus.Normal,
                                ValueDesc = diffvalue,
                                 PropertyId = propertyEntity.Id
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

            var entity = _prodpropertyvalRepo.Find(id);

            entity.UpdateDate = DateTime.Now;
            entity.Status = (int)DataStatus.Deleted;

            _prodpropertyvalRepo.Update(entity);

            return SuccessResponse();
        }
    }
}