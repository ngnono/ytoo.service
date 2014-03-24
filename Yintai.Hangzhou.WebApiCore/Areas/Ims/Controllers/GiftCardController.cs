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
   public class GiftCardController:RestfulController
    {
       [RestfulAuthorize]
       public ActionResult My(int authuid)
       {
           return this.RenderSuccess<dynamic>(c => c.Data = new
           {
               is_binded = true,
               amount = 1000.1

           });
       }
       [RestfulAuthorize]
       public ActionResult Create(string phone,string pwd)
       {
           return this.RenderSuccess<dynamic>(null);
       }

       [RestfulAuthorize]
       public ActionResult Items()
       {
           return this.RenderSuccess<dynamic>(c => c.Data = new
           {
               id = 1,
               image = "",
               desc = "mockup gifts",
               items = new List<dynamic>() { 
                new {id = 1,unit_price = 500,price = 400},
                new {id = 2,unit_price = 600,price = 500},
                new {id = 3,unit_price = 700,price = 600},
                new {id = 4,unit_price = 800,price = 700}
               }
           });
       }
       [RestfulAuthorize]
       public ActionResult List(Yintai.Hangzhou.Contract.DTO.Request.IMSGiftcardListRequest request)
       {
           var mockupResponse = new List<dynamic>();
           mockupResponse.Add(new
           {
               
             card_no = "test111111",
               amount = 100.1,
               purchase_date = "2013-04-24",
               charge_date = "2013-4-24",
               status_i = 1,
               verify_phone = "139000000",
               send_date = "2013-4-24",
               receive_date = "2013-4-24"
           });
           mockupResponse.Add(new
           {
               card_no = "test111112",
               amount = 100.1,
               purchase_date = "2013-04-24",
               charge_date = "2013-4-24",
               status_i = 1,
               verify_phone = "139000000",
               send_date = "2013-4-24",
               receive_date = "2013-4-24"
           });

           var response = new PagerInfoResponse<dynamic>(request.PagerRequest, mockupResponse.Count)
           {
               Items = mockupResponse
           };
           return this.RenderSuccess<PagerInfoResponse<dynamic>>(c => c.Data = response);
       }

       [RestfulAuthorize]
       public ActionResult Recharge(string charge_no)
       {
           return this.RenderSuccess<dynamic>(null);
       }

       [RestfulAuthorize]
       public ActionResult Send(Yintai.Hangzhou.Contract.DTO.Request.IMSGiftcardSendRequest request)
       {
           return this.RenderSuccess<dynamic>(null);
       }
       [RestfulAuthorize]
       public ActionResult Change_Pwd(Yintai.Hangzhou.Contract.DTO.Request.IMSGiftcardChangePwdRequest request)
       {
           return this.RenderSuccess<dynamic>(null);
       }
       [RestfulAuthorize]
       public ActionResult Reset_Pwd(Yintai.Hangzhou.Contract.DTO.Request.IMSGiftcardResetPwdRequest request)
       {
           return this.RenderSuccess<dynamic>(null);
       }
       [RestfulAuthorize]
       public ActionResult Refuse(string charge_no)
       {
           return this.RenderSuccess<dynamic>(null);
       }
    }
}
