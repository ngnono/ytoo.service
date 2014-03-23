using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository;
using Intime.OPC.Service;
using Intime.OPC.WebApi.Core.Security;
using Intime.OPC.WebApi.Models;

namespace Intime.OPC.WebApi.Controllers
{
    /// <summary>
    ///     账户相关接口
    /// </summary>
    public class TransController : ApiController
    {
        private readonly ITransService _transService;
       

        public TransController(ITransService transService)
        {
            _transService = transService;
        }

       
        [HttpGet]
        public IHttpActionResult SelectSales(string startDate, string endDate, string orderNo, string saleOrderNo)
        {
            return Ok(_transService.Select( startDate,  endDate,  orderNo,  saleOrderNo));
        }
        [HttpPut]
        public IHttpActionResult Finish([FromBody] Dictionary<string, string> sale)
        {

            //TODO:check params
            if (_transService.Finish(sale))
            {
                return Ok();
            }

            return InternalServerError();
        }

        [HttpGet]
        public IHttpActionResult SelectSaleDetail(IEnumerable<string> ids)
        {
            return Ok(_transService.SelectSaleDetail(ids));
        }

        [HttpGet]
        public IHttpActionResult SelectMark([FromUri] string orderNo)
        {
            var result = _transService.GetRemarksByOrderNo(orderNo);
            return Ok(result);
        }
    }
}