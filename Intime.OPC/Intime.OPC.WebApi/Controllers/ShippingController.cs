using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Intime.OPC.Domain.Dto.Request;
using Intime.OPC.WebApi.Bindings;
using Intime.OPC.WebApi.Core;
using Intime.OPC.WebApi.Core.Filters;

namespace Intime.OPC.WebApi.Controllers
{
    [APIExceptionFilter]
    [RoutePrefix("api/shipping")]
    public class ShippingController : BaseController
    {
        /// <summary>
        /// 生成出库单
        /// </summary>
        /// <returns></returns>
        [Route("/shippingsaleorders")]
        [HttpPost]
        public IHttpActionResult PostOrder([FromBody]CreateShippingSaleOrderRequest request, [UserId]int userId)
        {
            //不同的销售单可以生成同一个出库单

            throw new NotImplementedException();
        }


        /// <summary>
        /// 获取出库单
        /// </summary>
        /// <returns></returns>
        [Route("/shippingsaleorders/{id:int}")]
        [HttpGet]
        public IHttpActionResult Get(int id, [UserId] int userId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取出库单
        /// </summary>
        /// <returns></returns>
        [Route("/shippingsaleorders")]
        [HttpGet]
        public IHttpActionResult GetList([FromUri]GetShippingSaleOrderRequest request, [UserId]int userId)
        {
            //按创建日期倒序
            throw new NotImplementedException();
        }

        /// <summary>
        /// 添加 物流信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Route("/shippingsaleorders/{id:int}")]
        [HttpPut]
        public IHttpActionResult Put(int id, [FromBody] PutShippingSaleOrderRequest request, [UserId] int userId)
        {
            throw new NotImplementedException();
        }
    }
}
