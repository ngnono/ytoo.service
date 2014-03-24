using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Yintai.Hangzhou.Contract.DTO.Response;
using Yintai.Hangzhou.WebSupport.Mvc;

namespace Yintai.Hangzhou.WebApiCore.Areas.Ims.Controllers
{
   public class StoreController:RestfulController
    {
       [RestfulAuthorize]
       public ActionResult My(int? authuid)
       {
           return this.RenderSuccess<IMSStoreDetailResponse>(m => m.Data = new IMSStoreDetailResponse()
           {
               Id = 1,
               Name = "mockup 店",
               GiftCardSaling = new IMSGiftCard() { 
                 Id = 1,
                  Desc = "3/8节特别定制礼品卡",
                   ImageUrl = ""
               },
                ComboSaling = new IMSCombo[]{new IMSCombo(){
                     Id = 1,
                      Desc = "mockup商品",
                      ImageUrl =""
                      

                }, new IMSCombo(){
                     Id = 2,
                      Desc = "mockup2商品",
                      ImageUrl =""
                      
                }}
           });
           
       }

       [RestfulAuthorize]
       public ActionResult Create(string invite_code,int? authuid)
       {
           return this.RenderSuccess<IMSStoreDetailResponse>(m => m.Data = new IMSStoreDetailResponse()
           {
               Id = 1,
               Name = "mockup 店",
               GiftCardSaling = new IMSGiftCard()
               {
                   Id = 1,
                   Desc = "3/8节特别定制礼品卡",
                   ImageUrl = ""
               },
               ComboSaling = new IMSCombo[]{new IMSCombo(){
                     Id = 1,
                      Desc = "mockup商品",
                      ImageUrl =""
                      

                }, new IMSCombo(){
                     Id = 2,
                      Desc = "mockup2商品",
                      ImageUrl =""
                      
                }}
           });

       }

       [RestfulAuthorize]
       public ActionResult Detail(int id, int? authuid)
       {
           return this.RenderSuccess<IMSStoreDetailResponse>(m => m.Data = new IMSStoreDetailResponse()
           {
               Id = 1,
               Name = "mockup 店",
               GiftCardSaling = new IMSGiftCard()
               {
                   Id = 1,
                   Desc = "3/8节特别定制礼品卡",
                   ImageUrl = ""
               },
               ComboSaling = new IMSCombo[]{new IMSCombo(){
                     Id = 1,
                      Desc = "mockup商品",
                      ImageUrl =""
                      

                }, new IMSCombo(){
                     Id = 2,
                      Desc = "mockup2商品",
                      ImageUrl =""
                      
                }}
           });

       }
    }
}
