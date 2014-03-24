using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Yintai.Hangzhou.Contract.DTO.Request;
using Yintai.Hangzhou.Contract.DTO.Response;
using Yintai.Hangzhou.Model.Enums;
using Yintai.Hangzhou.WebSupport.Mvc;

namespace Yintai.Hangzhou.WebApiCore.Areas.Ims.Controllers
{
    public class OrderController:RestfulController
    {
        [RestfulAuthorize]
        public ActionResult My(PagerInfoRequest request)
        {
            var mockupResponse = new List<dynamic>();
            mockupResponse.Add(new
            {
                order_no = "2000110000",
                amount = 100.1,
                create_date = "2014-3-25",
                status = "paid",
                status_id = (int)OrderStatus.Paid,
                products = new List<dynamic>(){new {
                    id = 1,
                    name = "order detail product 1",
                    price = 100.1,
                    image=""
                },new { id = 2,
                    name = "order detail product 2",
                    price = 100.2,
                    image=""}}
                
            });
            mockupResponse.Add(new
            {
                order_no = "2000110001",
                amount = 100.1,
                create_date = "2014-3-24",
                status = "paid",
                status_id = (int)OrderStatus.Paid,
                products = new List<dynamic>(){new {
                    id = 1,
                    name = "order detail product 1",
                    price = 100.1,
                    image=""
                },new { id = 2,
                    name = "order detail product 2",
                    price = 100.2,
                    image=""}}
            });

            var response = new PagerInfoResponse<dynamic>(request.PagerRequest, mockupResponse.Count)
            {
                Items = mockupResponse
            };
            return this.RenderSuccess<PagerInfoResponse<dynamic>>(c => c.Data = response);
        }
    }
}
