using System;
using System.Web;
using System.Web.Http;
using Intime.OPC.Domain.Models;
using Intime.OPC.Service;
using Intime.OPC.WebApi.Core.Security;
using Intime.OPC.WebApi.Models;

namespace Intime.OPC.WebApi.Controllers
{
    /// <summary>
    ///    Sale相关接口
    /// </summary>
    public class SaleController : ApiController
    {
        private readonly ISaleService _saleService;

        public SaleController(ISaleService saleService)
        {
            _saleService = saleService;
        }
 
         [HttpPut]
        public IHttpActionResult UpdateSatus([FromBody] OPC_Sale sale)
        {

            //TODO:check params
            if (_saleService.UpdateSatus(sale))
            {
                return Ok();
            }

            return InternalServerError();
        }
        [HttpGet]
        public IHttpActionResult SelectSale()
        {

            //TODO:check params
            return Ok(_saleService.Select());
        }


        [HttpGet]
        public IHttpActionResult SelectRemarks(string  saleID)
        {
            return Ok(  _saleService.GetRemarksBySaleNo(saleID));
        }
    }
}
