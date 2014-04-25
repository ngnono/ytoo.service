using com.intime.fashion.common;
using com.intime.fashion.common.Wxpay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using Yintai.Architecture.Common.Data.EF;
using Yintai.Hangzhou.Cms.WebSiteV1.Models;
using Yintai.Hangzhou.Data.Models;
using Yintai.Hangzhou.Model.Enums;

namespace Yintai.Hangzhou.Cms.WebSiteV1.Controllers
{

    public class WxmenuController : UserController
    {
        private IEFRepository<WX_MenuEntity> _menuRepo;
        public WxmenuController(IEFRepository<WX_MenuEntity> menuRepo)
        {
            _menuRepo = menuRepo;
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Ajax_Root()
        {
            var linq = Context.Set<WX_MenuEntity>().Where(w => w.Status == (int)DataStatus.Normal && !w.ParentId.HasValue)
                       .GroupJoin(Context.Set<WX_MenuEntity>().Where(wm => wm.Status == (int)DataStatus.Normal),
                                    o => o.Id,
                                    i => i.ParentId,
                                    (o, i) => new { W = o, Second = i })
                       .OrderBy(w => w.W.Pos)
                       .ToList()
                       .Select(w => new WxMenuItemViewModel()
                       {
                           Id = w.W.WKey,
                           Text = w.W.Name,
                           Data = w.Second.Count() > 0 ? null : new WxMenuDataViewModel() { Url = w.W.Url },
                           Children = w.Second.Select(ws => new WxMenuItemViewModel()
                           {
                               Id = ws.WKey,
                               Text = ws.Name,
                               Data = new WxMenuDataViewModel() { Url = ws.Url }
                           })
                       });


            return new JsonNetResult(linq);
        }
        public JsonResult Ajax_Persist(WxMenuViewModel menu)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.Where(v => v.Errors.Count() > 0).First();
                return this.FailResponse(error.Errors.First().ErrorMessage);
            }
            if (menu.Items.Count() < 1)
                return FailResponse("必须至少设置一个菜单");

            using (var ts = new TransactionScope())
            {
                _menuRepo.Delete(m => true);
                int i = 0;
                foreach (var item in menu.Items)
                {
                    var thisMenu = _menuRepo.Insert(new WX_MenuEntity()
                    {
                        WKey = item.Id,
                        ActionType = 1,
                        Name = item.Text,
                        Pos = i++,
                        Status = (int)DataStatus.Normal,
                        UpdateDate = DateTime.Now,
                        Url = item.Data == null ? string.Empty : item.Data.Url
                    });
                    if (item.Children != null)
                    {
                        foreach (var secondItem in item.Children)
                        {
                            _menuRepo.Insert(new WX_MenuEntity()
                            {
                                WKey = secondItem.Id,
                                ActionType = 1,
                                Name = secondItem.Text,
                                Pos = 0,
                                Status = (int)DataStatus.Normal,
                                UpdateDate = DateTime.Now,
                                Url = secondItem.Data == null ? string.Empty : secondItem.Data.Url,
                                ParentId = thisMenu.Id
                            });
                        }
                    }
                }
                ts.Complete();
            }
            return SuccessResponse();
        }

        public JsonResult Ajax_Publish()
        {
            var linq = Context.Set<WX_MenuEntity>().Where(w => w.Status == (int)DataStatus.Normal && !w.ParentId.HasValue)
                       .GroupJoin(Context.Set<WX_MenuEntity>().Where(wm => wm.Status == (int)DataStatus.Normal),
                                    o => o.Id,
                                    i => i.ParentId,
                                    (o, i) => new { W = o, Second = i })
                       .OrderBy(w => w.W.Pos)
                       .ToList();
            var buttons = new List<WxMenuItemBase>();
            foreach (var item in linq)
            {
                if (item.Second.Count() > 0)
                {
                    buttons.Add(new WxMenuItemSub()
                    {
                        Name = item.W.Name,
                        SubButtons = item.Second.Select(si => new WxMenuItemView()
                        {
                            Url = si.Url,
                            Name = si.Name,
                            Type = "view",

                        })
                    });
                }
                else
                {
                    buttons.Add(new WxMenuItemView()
                    {
                        Url = item.W.Url,
                        Name = item.W.Name,
                        Type = "view"
                    });
                }
            }
            WxServiceHelper.RefreshMenu(new WxMenu()
            {
                Buttons = buttons
            },
                  null, null);
            return SuccessResponse();
        }
    }
}
