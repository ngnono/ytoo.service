using System;
using System.Linq;
using System.Web.Mvc;
using Yintai.Architecture.Common.Data.EF;
using Yintai.Architecture.Common.Models;
using Yintai.Hangzhou.Cms.WebSiteV1.Models;
using Yintai.Hangzhou.Cms.WebSiteV1.Util;
using Yintai.Hangzhou.Data.Models;

namespace Yintai.Hangzhou.Cms.WebSiteV1.Controllers
{
     [AdminAuthorize]
    public class GiftCardController : UserController
     {
         private IEFRepository<IMS_GiftCardEntity> _cardRepo;
         private IEFRepository<IMS_GiftCardItemEntity> _itemRepo;
         public GiftCardController(
             IEFRepository<IMS_GiftCardEntity> cardRepo,
             IEFRepository<IMS_GiftCardItemEntity> itemRepo)
         {
             _cardRepo = cardRepo;
             _itemRepo = itemRepo;

         }
        public ActionResult List(PagerRequest request)
         {
             int page = 1;
             int pagesize = 20;
             if (request.PageIndex > 0)
             {
                 page = request.PageIndex;
             }
             if (request.PageSize > 0 && request.PageSize <= 40)
             {
                 pagesize = request.PageSize;
             }

             var cards = _cardRepo.Get(x=>x.Id > 0)
                 .OrderByDescending(x => x.UpdateDate)
                 .Skip((page - 1)*pagesize)
                 .Take(pagesize).ToList();

             return this.View("List", new GiftCardModel(request,_cardRepo.GetAll().Count()) {CardList = cards});
         }

         public ActionResult Detail(int id)
         {
             var card = _cardRepo.Find(x => x.Id == id);
             if (card == null)
             {
                 RedirectToAction("List");
             }
             var items = _itemRepo.Get(x => x.GiftCardId == id);
             return this.View("Details", new GiftCardItemViewModel() {Card = card, CardItems = items.ToList()});
         }

         [HttpGet]
         public ActionResult Create()
         {
             return this.View();
         }

          [HttpPost]
         public ActionResult Create(GiftCardEntityModel card)
         {
              if (ModelState.IsValid)
              {
                  var cardEntity = this._cardRepo.Insert(new IMS_GiftCardEntity()
                  {
                      Name = card.Name,
                      Status = card.IsPublished ? 1 : 0,
                      UpdateDate = DateTime.Now
                  });
                  if (cardEntity == null || cardEntity.Id <= 0)
                  {
                      return this.View("Create", card);
                  }
              }
              else
              {
                  return this.View("Create", card);
              }
              return RedirectToAction("List");
         }

         [HttpPost]
         public JsonResult Up(int id)
         {
             return this.SetItemStatus(id, 1);
         }

         [HttpPost]
         public JsonResult Down(int id)
         {
             return this.SetItemStatus(id, 0);
         }

         [HttpPost]
         public JsonResult Online(int id)
         {
             return this.SetStatus(id, 1);
         }

         [HttpPost]
         public JsonResult Offline(int id)
         {
             return this.SetStatus(id, 0);
         }

         private JsonResult SetStatus(int id, int status)
         {
             if (status > 1 || status < 0)
             {
                 return this.FailResponse(string.Format("无效的状态值{0}", status));
             }

             var card = _cardRepo.Find(x => x.Id == id);

             if (card == null)
             {
                 return this.FailResponse("不存在的礼品卡");
             }

             var cardItems = _itemRepo.Get(x => x.GiftCardId == id).ToList();
             foreach (var item in cardItems)
             {
                 item.Status = status;
                 _itemRepo.Update(item);
             }
             card.Status = status;
             _cardRepo.Update(card);
             return this.SuccessResponse();
         }

         private JsonResult SetItemStatus(int id, int status)
         {
             if (status > 1 || status < 0)
             {
                 return this.FailResponse(string.Format("无效的状态值{0}", status));
             }

             var item = _itemRepo.Find(x => x.Id == id);
             if(item == null)
             {
                 return this.FailResponse("不存在的子礼品卡");
             }
             item.Status = status;
             _itemRepo.Update(item);

             if (status == 0 && !_itemRepo.Get(x => x.GiftCardId == item.Id && x.Status == status).Any())
             {
                 var card = _cardRepo.Find(x => x.Id == item.GiftCardId);
                 if (card != null)
                 {
                     card.Status = status;
                     _cardRepo.Update(card);
                 }
             }
             return this.SuccessResponse();
         }
     }
}