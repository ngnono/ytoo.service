using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Yintai.Hangzhou.Contract.DTO.Request;
using Yintai.Hangzhou.Contract.DTO.Response;
using Yintai.Hangzhou.WebSupport.Mvc;

namespace Yintai.Hangzhou.WebApiCore.Areas.Ims.Controllers
{
    public class AssistantController : RestfulController
    {
        [RestfulAuthorize]
        public ActionResult Gift_Cards(PagerInfoRequest request)
        {
            var mockupResponse = new List<dynamic>();
            mockupResponse.Add(new {
                id = 1,
                desc = "mockup 礼品卡",
                image = "",
                is_online = true
            });
            var response = new PagerInfoResponse<dynamic>(request.PagerRequest, mockupResponse.Count)
            {
                Items = mockupResponse
            };
            return this.RenderSuccess<PagerInfoResponse<dynamic>>(c => c.Data = response);
        }

        [RestfulAuthorize]
        public ActionResult Combos(PagerInfoRequest request)
        {
            var mockupResponse = new List<dynamic>();
            mockupResponse.Add(new
            {
                id = 1,
                desc = "mockup搭配",
                image = "",
                price = 100.1,
                is_online = true
            });
            mockupResponse.Add(new
            {
                id = 2,
                desc = "mockup2搭配",
                image = "",
                price = 101.1,
                is_online = true
            });
            var response = new PagerInfoResponse<dynamic>(request.PagerRequest, mockupResponse.Count)
            {
                Items = mockupResponse
            };
            return this.RenderSuccess<PagerInfoResponse<dynamic>>(c => c.Data = response);
        }

        [RestfulAuthorize]
        public ActionResult Products(PagerInfoRequest request)
        {
            var mockupResponse = new List<dynamic>();
            mockupResponse.Add(new
            {
                id = 1,
                desc = "已拍商品1",
                price = 100.1,
                image = "",
                is_online = true
            });
            mockupResponse.Add(new
            {
                id = 1,
                desc = "已拍商品2",
                price = 100.2,
                image = "",
                is_online = true
            });
            var response = new PagerInfoResponse<dynamic>(request.PagerRequest, mockupResponse.Count)
            {
                Items = mockupResponse
            };
            return this.RenderSuccess<PagerInfoResponse<dynamic>>(c => c.Data = response);
        }

        [RestfulAuthorize]
        public ActionResult SalesCodes(PagerInfoRequest request)
        {
            var mockupResponse = new List<dynamic>();
            mockupResponse.Add("mockupsalescode1");
            mockupResponse.Add("mockupsalescode2");
           
            var response = new PagerInfoResponse<dynamic>(request.PagerRequest, mockupResponse.Count)
            {
                Items = mockupResponse
            };
            return this.RenderSuccess<PagerInfoResponse<dynamic>>(c => c.Data = response);
        }

        [RestfulAuthorize]
        public ActionResult Brands(PagerInfoRequest request)
        {
            var mockupResponse = new List<dynamic>();
            mockupResponse.Add(new { 
                id = 1,
                name = "mockup品牌1"
            });
            mockupResponse.Add(new
            {
                id = 2,
                name = "mockup品牌2"
            });

            var response = new PagerInfoResponse<dynamic>(request.PagerRequest, mockupResponse.Count)
            {
                Items = mockupResponse
            };
            return this.RenderSuccess<PagerInfoResponse<dynamic>>(c => c.Data = response);
        }

        [RestfulAuthorize]
        public ActionResult Category_Sizes(PagerInfoRequest request)
        {
            var mockupResponse = new List<dynamic>();
            mockupResponse.Add(new
            {
                id = 1,
                name = "mockup分类1",
                sizes = new List<dynamic>() { new { id = 1, name = "mockup尺码1" }, new { id = 2, name = "mockup尺码2" } }
            });
            mockupResponse.Add(new
            {
                id = 1,
                name = "mockup分类2",
                sizes = new List<dynamic>() { new { id = 1, name = "mockup尺码21" }, new { id = 2, name = "mockup尺码22" } }
            });

            var response = new PagerInfoResponse<dynamic>(request.PagerRequest, mockupResponse.Count)
            {
                Items = mockupResponse
            };
            return this.RenderSuccess<PagerInfoResponse<dynamic>>(c => c.Data = response);
        }

        [RestfulAuthorize]
        public ActionResult Income(int authuid)
        {
            return this.RenderSuccess<dynamic>(c => c.Data = new { 
                received_amount = 1000,
                frozen_amount = 100,
                request_amount = 200,
                avail_amount = 300

            });
        }

        [RestfulAuthorize]
        public ActionResult Income_Received(PagerInfoRequest request, int authuid)
        {
            var mockupResponse = new List<dynamic>();
            mockupResponse.Add(new
            {
                id = 1,
                create_date = "2014-3-22",
                bankname = "建设银行",
                cardno = "622222222xxxxxx2222",
                amount = 100.1
            });
            mockupResponse.Add(new
            {
                id = 1,
                create_date = "2014-3-22",
                bankname = "建设银行",
                cardno = "622222222xxxxxx2223",
                amount = 101.1
            });

            var response = new PagerInfoResponse<dynamic>(request.PagerRequest, mockupResponse.Count)
            {
                Items = mockupResponse
            };
            return this.RenderSuccess<PagerInfoResponse<dynamic>>(c => c.Data = response);
        }

        [RestfulAuthorize]
        public ActionResult Order_GiftCards(PagerInfoRequest request, int authuid)
        {
            var mockupResponse = new List<dynamic>();
            mockupResponse.Add(new
            {
               
                create_date = "2014-3-22",
                amount = 100.1
            });
            mockupResponse.Add(new
            {
                create_date = "2014-3-22",
                amount = 101.1
            });

            var response = new PagerInfoResponse<dynamic>(request.PagerRequest, mockupResponse.Count)
            {
                Items = mockupResponse
            };
            return this.RenderSuccess<PagerInfoResponse<dynamic>>(c => c.Data = response);
        }

        [RestfulAuthorize]
        public ActionResult Orders(PagerInfoRequest request, int authuid)
        {
            var mockupResponse = new List<dynamic>();
            mockupResponse.Add(new
            {

                create_date = "2014-3-22",
                amount = 100.1,
                order_no="20100000001",
                status = "已支付"
            });
            mockupResponse.Add(new
            {
                create_date = "2014-3-23",
                amount = 100.1,
                order_no = "20100000002",
                status = "已支付"
            });

            var response = new PagerInfoResponse<dynamic>(request.PagerRequest, mockupResponse.Count)
            {
                Items = mockupResponse
            };
            return this.RenderSuccess<PagerInfoResponse<dynamic>>(c => c.Data = response);
        }

        [RestfulAuthorize]
        public ActionResult Update(Yintai.Hangzhou.Contract.DTO.Request.IMSStoreDetailUpdateRequest request)
        {

            return this.RenderSuccess<dynamic>(c => c.Data = new { 
                name = "mockup update",
                phone = "130000000"
            });
        }

        [RestfulAuthorize]
        public ActionResult Update_Logo(FormCollection form)
        {

            return this.RenderSuccess<dynamic>(c => c.Data = new
            {
                logo = ""
            });
        }

        [RestfulAuthorize]
        public ActionResult Update_Template(int templateId, int authuid)
        {

            return this.RenderSuccess<dynamic>(null);
        }

        [RestfulAuthorize]
        public ActionResult Feedback(string comment, int authuid)
        {

            return this.RenderSuccess<dynamic>(null);
        }

        [RestfulAuthorize]
        public ActionResult Favor(Yintai.Hangzhou.Contract.DTO.Request.IMSAssistantFavorRequest request)
        {

            return this.RenderSuccess<dynamic>(null);
        }

        [RestfulAuthorize]
        public ActionResult Unfavor(Yintai.Hangzhou.Contract.DTO.Request.IMSAssistantFavorRequest request)
        {

            return this.RenderSuccess<dynamic>(null);
        }
    }
}
