using Intime.OPC.Domain.Exception;
using Intime.OPC.Service;
using Intime.OPC.WebApi.Bindings;
using Microsoft.Owin.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using System.Web.Http.Results;

namespace Intime.OPC.WebApi.Controllers
{
    /// <summary>
    ///    Sale相关接口
    /// </summary>
    public class SaleController : ApiController
    {
        private readonly ISaleService _saleService;
        private readonly ILogger _logger;

        public SaleController(ISaleService saleService,ILogger logger)
        {
            _saleService = saleService;
            _logger = logger;
        }

        [HttpGet]
        public IHttpActionResult SelectSale()
        {
            //TODO:check params
            return Ok(_saleService.Select());
        }


        [HttpGet]
        public IHttpActionResult SelectRemarks(string saleId,[UserId] int userId)
        {
            //todo data Ahorization
            return Ok(_saleService.GetRemarksBySaleNo(saleId));
        }

        [HttpGet]
        public IHttpActionResult GetSaleOrderDetails(string saleOrderNo, [UserId] int userId )
        {
            try
            {
                return Ok(_saleService.GetSaleOrderDetails(saleOrderNo, userId));
            }
            catch (SaleOrderNotExistsException e)
            {
                _logger.WriteError(e.Message);
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex.Message);
                return InternalServerError();
            }
        }

        [HttpPut]
        public IHttpActionResult SetSaleOrderPrinted(IEnumerable<string> saleOrderNos, [UserId] int? userId)
        {
            if (!userId.HasValue)
            {
                return new StatusCodeResult(HttpStatusCode.Unauthorized,this);
            }
            foreach (var orderNo in saleOrderNos)
            {
                try
                {
                    this._saleService.PrintSale(orderNo, userId.Value);
                }
                catch (SaleOrderNotExistsException ex)
                {
                    _logger.WriteError(ex.Message);
                    continue;
                }
                catch (Exception e)
                {
                    _logger.WriteError(e.Message);
                    return InternalServerError();
                }
            }
            return Ok();
        }      
    }
}
