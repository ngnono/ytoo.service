using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Yintai.Architecture.Common.Models;
using Yintai.Architecture.Framework.Extension;
using Yintai.Hangzhou.Cms.WebSiteCoreV1.Manager;
using Yintai.Hangzhou.Cms.WebSiteCoreV1.Models;
using Yintai.Hangzhou.Cms.WebSiteCoreV1.Util;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.Model.Filters;
using Yintai.Hangzhou.Repository.Contract;
using Yintai.Hangzhou.WebSupport.Binder;
using Yintai.Hangzhou.WebSupport.Mvc;

namespace Yintai.Hangzhou.Cms.WebSiteCoreV1.Controllers
{
    [AdminAuthorize]
    public class CommentController : UserController
    {
        private readonly ICommentRepository _commentRepository;
        private ICustomerRepository _customerRepository;
        private IProductRepository _productRepository;
        private IPromotionRepository _promotionRepository;
        private IResourceRepository _resourceRepository;
        public CommentController(ICommentRepository commentRepository
            ,ICustomerRepository customerRepository
            ,IPromotionRepository promotionRepository
            ,IProductRepository productRepository
            ,IResourceRepository resourceRepository)
        {
            this._commentRepository = commentRepository;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _promotionRepository = promotionRepository;
            _resourceRepository = resourceRepository;
        }

        public ActionResult Index(PagerRequest request, CommentSearchOption search)
        {
            return List(request, search);
        }

        public ActionResult List(PagerRequest request, CommentSearchOption search)
        {
            int totalCount;
            var data = _commentRepository.Search(request.PageIndex
                , request.PageSize
                ,out totalCount
                ,search);
           var linqResult = data.Join(_commentRepository.Context.Set<UserEntity>(),
                o => o.User_Id,
                i => i.Id,
                (o, i) => new { C = o, U = i })
                .GroupJoin(_commentRepository.Context.Set<ResourceEntity>().Where(r => r.SourceType == (int)SourceType.Comment),
                o => o.C.Id,
                i => i.SourceId,
                (o, i) => new { C = o.C, U = o.U, CR = i })
                .GroupJoin(_commentRepository.Context.Set<ResourceEntity>(),
                o => new { o.C.SourceId, o.C.SourceType },
                i => new { i.SourceId, i.SourceType },
                (o, i) => new { C = o.C, U = o.U, CR = o.CR, PR = i });
           var vo = from l in linqResult.ToList()
                    let p = l.C
                    let u = l.U
                    select new CommentViewModel
                    {
                        CommentResource = MappingManager.ResourceViewMapping(l.CR.FirstOrDefault())
                        ,
                        SourceResource = MappingManager.ResourceViewMapping(l.PR.FirstOrDefault())
                        ,
                        Content = p.Content
                        ,
                        CreatedDate = p.CreatedDate
                        ,
                        CreatedUser = p.CreatedUser
                        ,
                        Id = p.Id
                        ,
                        ReplyId = p.ReplyId
                        ,
                        ReplyUser = p.ReplyUser
                        ,
                        SourceId = p.SourceId
                        ,
                        SourceType = p.SourceType
                        ,
                        Status = p.Status
                        ,
                        UpdatedDate = p.UpdatedDate
                        ,
                        UpdatedUser = p.UpdatedUser
                        ,
                        User_Id = p.User_Id
                        ,
                        CommentUser = new CustomerViewModel
                        {
                            Id = u.Id,
                            Name = u.Name,
                            Nickname = u.Nickname

                        }
                    };
            /*
            var resourceQuerable = _commentRepository.Context.Set<ResourceEntity>().AsQueryable();
            var vo =
                     from u in _commentRepository.Context.Set<UserEntity>()
                     from p in data
                     where p.User_Id == u.Id
                     let commentResource =
                            (from r in resourceQuerable
                             where r.SourceId == p.Id && p.SourceType == (int)SourceType.Comment
                             select new ResourceViewModel
                             {
                                 Domain = r.Domain,
                                 Name = r.Name,
                                 Id = r.Id,
                                 ExtName = r.ExtName,
                                 Type = r.Type
                             }).FirstOrDefault()
                     let sourceResource =
                            (from r in resourceQuerable
                             where r.SourceId == p.SourceId && r.SourceType == p.SourceType
                             select new ResourceViewModel
                             {
                                 Domain = r.Domain,
                                 Name = r.Name,
                                 Id = r.Id,
                                 ExtName = r.ExtName,
                                 Type = r.Type
                             }).FirstOrDefault()
                     select new CommentViewModel
                     {
                         CommentResource =commentResource
                         ,
                         SourceResource = sourceResource 
                         ,
                         Content = p.Content
                         ,
                         CreatedDate = p.CreatedDate
                         ,
                         CreatedUser = p.CreatedUser
                         ,
                         Id = p.Id
                         ,
                         ReplyId = p.ReplyId
                         ,
                         ReplyUser = p.ReplyUser
                         ,
                         SourceId = p.SourceId
                         ,
                         SourceType = p.SourceType
                         ,
                         Status = p.Status
                         ,
                         UpdatedDate = p.UpdatedDate
                         ,
                         UpdatedUser = p.UpdatedUser
                         ,
                         User_Id = p.User_Id
                         ,
                         CommentUser = new CustomerViewModel
                         {
                             Id = u.Id,
                             Name = u.Name,
                             Nickname = u.Nickname

                         }
                     };
            */

            var v = new CommentCollectionViewModel(request, totalCount) { Comments = vo.ToList() };

            return View("List", v);
        }

        public ActionResult Details(int? id, [FetchComment(KeyName = "id")]CommentEntity entity)
        {
            if (id == null || entity == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View();
            }

            var vo = MappingManager.CommentViewMapping(entity);

            return View(vo);
        }

        public ActionResult Create(int? commentid,[FetchComment(KeyName = "commentId")]CommentEntity comment)
        {
            if (comment == null)
                RedirectToAction("List");

            return View(new CommentViewModel { 
                 ReplyId = comment.Id
            });
        }

        public ActionResult Delete(int? id, [FetchComment(KeyName = "id")]CommentEntity entity)
        {
            if (id == null || entity == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return View();
            }

            var vo = MappingManager.CommentViewMapping(entity);

            return View(vo);
        }

       
        [HttpPost]
        public ActionResult Create(FormCollection formCollection, CommentViewModel vo)
        {
            if (ModelState.IsValid)
            {
                
                var targetEntity = _commentRepository.Find(c => c.Id == vo.ReplyId);
                if (targetEntity ==null)
                {
                    ModelState.AddModelError(string.Empty,"没有指定回复的评论");
                    return View(vo);
                }
                var entity = MappingManager.CommentEntityMapping(vo);
                entity.ReplyUser = targetEntity.CreatedUser;
                entity.CreatedUser = CurrentUser.CustomerId;
                entity.CreatedDate = DateTime.Now;
                entity.Status = (int)DataStatus.Normal;
                entity.SourceId = targetEntity.SourceId;
                entity.SourceType = targetEntity.SourceType;
                entity.User_Id = CurrentUser.CustomerId;
                entity.UpdatedUser = CurrentUser.CustomerId;
                entity.UpdatedDate = DateTime.Now;

                entity = this._commentRepository.Insert(entity);

                return RedirectToAction("Details", new {id= entity.Id });
            }

            return View(vo);
        }

        [HttpPost]
        public JsonResult OffLine([FetchComment(KeyName = "id")]CommentEntity entity)
        {
            if (entity == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return FailResponse();
            }

            entity.UpdatedDate = DateTime.Now;
            entity.UpdatedUser = CurrentUser.CustomerId;
            entity.Status = (int)DataStatus.Default;

            this._commentRepository.Update(entity);

            return SuccessResponse();
        }
        [HttpPost]
        public JsonResult OnLine([FetchComment(KeyName = "id")]CommentEntity entity)
        {
            if (entity == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return FailResponse();
            }

            entity.UpdatedDate = DateTime.Now;
            entity.UpdatedUser = CurrentUser.CustomerId;
            entity.Status = (int)DataStatus.Normal;

            this._commentRepository.Update(entity);

            return SuccessResponse();
        }
      

        [HttpPost]
        public JsonResult Delete([FetchComment(KeyName = "id")]CommentEntity entity)
        {
            if (entity == null)
            {
                ModelState.AddModelError("", "参数验证失败.");
                return FailResponse();
            }

            entity.UpdatedDate = DateTime.Now;
            entity.UpdatedUser = CurrentUser.CustomerId;
            entity.Status = (int)DataStatus.Deleted;

            this._commentRepository.Delete(entity);

            return SuccessResponse();
        }

    }
}
