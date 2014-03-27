using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using System.Web.Http.Results;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Exception;
using Intime.OPC.Domain.Models;
using Intime.OPC.Service;
using Intime.OPC.WebApi.Bindings;

namespace Intime.OPC.WebApi.Controllers
{
    /// <summary>
    ///     Sale相关接口
    /// </summary>
    public class SaleController : ApiController
    {
        private readonly ISaleService _saleService;

        public SaleController(ISaleService saleService)
        {
            _saleService = saleService;
        }

        [HttpGet]
        public IHttpActionResult GetSaleRemarks(string saleId, [UserId] int userId)
        {
            //todo data Ahorization
            return Ok(_saleService.GetRemarksBySaleNo(saleId));
        }

        [HttpGet]
        public IHttpActionResult GetSaleOrderDetails(string saleOrderNo, [UserId] int userId)
        {
            try
            {
                return Ok(_saleService.GetSaleOrderDetails(saleOrderNo, userId));
            }
            catch (SaleOrderNotExistsException e)
            {
                //_logger.WriteError(e.Message);
                return BadRequest();
            }
            catch (Exception ex)
            {
                //_logger.WriteError(ex.Message);
                return InternalServerError();
            }
        }

        [HttpPost]
        public IHttpActionResult WriteSaleRemark(OPC_SaleComment comment, [UserId] int userId)
        {
            try
            {
                comment.CreateDate = DateTime.Now;
                comment.CreateUser = userId;
                comment.UpdateDate = DateTime.Now;
                comment.UpdateUser = userId;
                _saleService.WriteSaleRemark(comment);
                return Ok();
            }
            catch (SaleOrderNotExistsException ex)
            {
                //todo 增加处理方式
                // _logger.WriteError(ex.Message);
                return BadRequest();
            }
            catch (Exception e)
            {
                // _logger.WriteError(e.Message);
                return InternalServerError();
            }
        }

        #region 更新状态

        /// <summary>
        ///     完成打印销售单
        /// </summary>
        /// <param name="saleOrderNos"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPut]
        public IHttpActionResult SetSaleOrderFinishPrintSale(IEnumerable<string> saleOrderNos, [UserId] int? userId)
        {
            if (!userId.HasValue)
            {
                // todo 获得单品系统的收银流水号
                return new StatusCodeResult(HttpStatusCode.Unauthorized, this);
            }
            foreach (string orderNo in saleOrderNos)
            {
                try
                {
                    _saleService.FinishPrintSale(orderNo, userId.Value);
                }
                catch (SaleOrderNotExistsException ex)
                {
                    //_logger.WriteError(ex.Message);
                }
                catch (Exception e)
                {
                    //_logger.WriteError(e.Message);
                    return InternalServerError();
                }
            }
            return Ok();
        }

        /// <summary>
        ///     打印销售单
        /// </summary>
        /// <param name="saleOrderNos"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPut]
        public IHttpActionResult SetSaleOrderPrintSale(IEnumerable<string> saleOrderNos, [UserId] int? userId)
        {
            if (!userId.HasValue)
            {
                return new StatusCodeResult(HttpStatusCode.Unauthorized, this);
            }
            foreach (string orderNo in saleOrderNos)
            {
                try
                {
                    _saleService.PrintSale(orderNo, userId.Value);
                }
                catch (SaleOrderNotExistsException ex)
                {
                    // _logger.WriteError(ex.Message);
                }
                catch (Exception e)
                {
                    //_logger.WriteError(e.Message);
                    return InternalServerError();
                }
            }
            return Ok();
        }

        /// <summary>
        ///     销售提货
        /// </summary>
        /// <param name="saleOrderNos">The sale order nos.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPut]
        public IHttpActionResult SetSaleOrderPickUp(IEnumerable<string> saleOrderNos, [UserId] int? userId)
        {
            if (!userId.HasValue)
            {
                return new StatusCodeResult(HttpStatusCode.Unauthorized, this);
            }
            foreach (string orderNo in saleOrderNos)
            {
                try
                {
                    _saleService.SetSaleOrderPickUp(orderNo, userId.Value);
                }
                catch (SaleOrderNotExistsException ex)
                {
                    // _logger.WriteError(ex.Message);
                }
                catch (Exception e)
                {
                    //  _logger.WriteError(e.Message);
                    return InternalServerError();
                }
            }
            return Ok();
        }

        /// <summary>
        ///     物流入库
        /// </summary>
        /// <param name="saleOrderNos">The sale order nos.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPut]
        public IHttpActionResult SetSaleOrderShipInStorage(IEnumerable<string> saleOrderNos, [UserId] int? userId)
        {
            if (!userId.HasValue)
            {
                return new StatusCodeResult(HttpStatusCode.Unauthorized, this);
            }
            foreach (string orderNo in saleOrderNos)
            {
                try
                {
                    _saleService.SetShipInStorage(orderNo, userId.Value);
                }
                catch (SaleOrderNotExistsException ex)
                {
                    //  _logger.WriteError(ex.Message);
                }
                catch (Exception e)
                {
//_logger.WriteError(e.Message);
                    return InternalServerError();
                }
            }
            return Ok();
        }

        /// <summary>
        ///     打印发货单
        /// </summary>
        /// <param name="saleOrderNos">The sale order nos.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPut]
        public IHttpActionResult SetSaleOrderPrintInvoice(IEnumerable<string> saleOrderNos, [UserId] int? userId)
        {
            if (!userId.HasValue)
            {
                return new StatusCodeResult(HttpStatusCode.Unauthorized, this);
            }
            foreach (string orderNo in saleOrderNos)
            {
                try
                {
                    _saleService.PrintInvoice(orderNo, userId.Value);
                }
                catch (SaleOrderNotExistsException ex)
                {
                    //_logger.WriteError(ex.Message);
                }
                catch (Exception e)
                {
                    //_logger.WriteError(e.Message);
                    return InternalServerError();
                }
            }
            return Ok();
        }

        /// <summary>
        ///     打印快递单
        /// </summary>
        /// <param name="saleOrderNos">The sale order nos.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPut]
        public IHttpActionResult SetSaleOrderPrintExpress(IEnumerable<string> saleOrderNos, [UserId] int? userId)
        {
            if (!userId.HasValue)
            {
                return new StatusCodeResult(HttpStatusCode.Unauthorized, this);
            }
            foreach (string orderNo in saleOrderNos)
            {
                try
                {
                    _saleService.PrintExpress(orderNo, userId.Value);
                }
                catch (SaleOrderNotExistsException ex)
                {
                    // _logger.WriteError(ex.Message);
                }
                catch (Exception e)
                {
                    // _logger.WriteError(e.Message);
                    return InternalServerError();
                }
            }
            return Ok();
        }

        /// <summary>
        ///     已发货
        /// </summary>
        /// <param name="saleOrderNos">The sale order nos.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPut]
        public IHttpActionResult SetSaleOrderShipped(IEnumerable<string> saleOrderNos, [UserId] int? userId)
        {
            if (!userId.HasValue)
            {
                return new StatusCodeResult(HttpStatusCode.Unauthorized, this);
            }
            foreach (string orderNo in saleOrderNos)
            {
                try
                {
                    _saleService.Shipped(orderNo, userId.Value);
                }
                catch (SaleOrderNotExistsException ex)
                {
                    // _logger.WriteError(ex.Message);
                }
                catch (Exception e)
                {
                    // _logger.WriteError(e.Message);
                    return InternalServerError();
                }
            }
            return Ok();
        }

        /// <summary>
        ///     缺货
        /// </summary>
        /// <param name="saleOrderNos"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPut]
        public IHttpActionResult SetSaleOrderStockOut(IEnumerable<string> saleOrderNos, [UserId] int? userId)
        {
            if (!userId.HasValue)
            {
                return new StatusCodeResult(HttpStatusCode.Unauthorized, this);
            }
            foreach (string orderNo in saleOrderNos)
            {
                try
                {
                    _saleService.StockOut(orderNo, userId.Value);
                }
                catch (SaleOrderNotExistsException ex)
                {
                    // _logger.WriteError(ex.Message);
                }
                catch (Exception e)
                {
                    // _logger.WriteError(e.Message);
                    return InternalServerError();
                }
            }
            return Ok();
        }

        #endregion

        #region 查询

        /// <summary>
        ///     获得未提货的数据
        /// </summary>
        /// <param name="saleId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetSaleNoPickUp(DateTime startDate, DateTime endDate, string saleOrderNo,
            string orderNo, [UserId] int userId)
        {
            try
            {
                return Ok(_saleService.GetNoPickUp(saleOrderNo, userId, saleOrderNo, startDate, endDate));
            }
            catch (SaleOrderNotExistsException e)
            {
                // _logger.WriteError(e.Message);
                return BadRequest();
            }
            catch (Exception ex)
            {
                //_logger.WriteError(ex.Message);
                return InternalServerError();
            }
        }

        /// <summary>
        ///     获得已完成 打印销售单 的数据
        /// </summary>
        /// <param name="saleId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetSalePrintSale(DateTime startDate, DateTime endDate, string saleOrderNo, string orderNo,
            [UserId] int userId)
        {
            try
            {
                return Ok(_saleService.GetPrintSale(saleOrderNo, userId, orderNo, startDate, endDate));
            }
            catch (SaleOrderNotExistsException e)
            {
                //_logger.WriteError(e.Message);
                return BadRequest();
            }
            catch (Exception ex)
            {
                //_logger.WriteError(ex.Message);
                return InternalServerError();
            }
        }

        /// <summary>
        ///     获得已完成 打印发货单 的数据
        /// </summary>
        /// <param name="saleId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetSalePrintInvoice(string startDate, string endDate, string saleOrderNo,
            string orderNo, [UserId] int userId)
        {
            try
            {
                DateTime dtStart = DateTime.MinValue;
                bool bl = DateTime.TryParse(startDate, out dtStart);

                DateTime dtEnd = DateTime.Now;
                bl = DateTime.TryParse(endDate, out dtEnd);
                return Ok(_saleService.GetPrintInvoice(saleOrderNo, userId, orderNo, dtStart, dtEnd));
            }
            catch (SaleOrderNotExistsException e)
            {
                //_logger.WriteError(e.Message);
                return BadRequest();
            }
            catch (Exception ex)
            {
                //_logger.WriteError(ex.Message);
                return InternalServerError();
            }
        }

        /// <summary>
        ///     获得已完成 打印快递单 的数据
        /// </summary>
        /// <param name="saleId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetSalePrintExpress(string startDate, string endDate, string saleOrderNo,
            string orderNo, [UserId] int userId)
        {
            try
            {
                DateTime dtStart = DateTime.MinValue;
                bool bl = DateTime.TryParse(startDate, out dtStart);

                DateTime dtEnd = DateTime.Now;
                bl = DateTime.TryParse(endDate, out dtEnd);
                return Ok(_saleService.GetPrintExpress(saleOrderNo, userId, orderNo, dtStart, dtEnd));
            }
            catch (SaleOrderNotExistsException e)
            {
                //_logger.WriteError(e.Message);
                return BadRequest();
            }
            catch (Exception ex)
            {
                //_logger.WriteError(ex.Message);
                return InternalServerError();
            }
        }

        /// <summary>
        ///     获得已完成 物流入库 的数据
        /// </summary>
        /// <param name="saleId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetSaleShipInStorage(string startDate, string endDate, string saleOrderNo,
            string orderNo, [UserId] int userId)
        {
            try
            {
                DateTime dtStart = DateTime.MinValue;
                bool bl = DateTime.TryParse(startDate, out dtStart);

                DateTime dtEnd = DateTime.Now;
                bl = DateTime.TryParse(endDate, out dtEnd);
                return Ok(_saleService.GetShipInStorage(saleOrderNo, userId, orderNo, dtStart, dtEnd));
            }
            catch (SaleOrderNotExistsException e)
            {
                //_logger.WriteError(e.Message);
                return BadRequest();
            }
            catch (Exception ex)
            {
                //_logger.WriteError(ex.Message);
                return InternalServerError();
            }
        }

        [HttpGet]
        public IHttpActionResult GetSaleByOrderNo(string orderID)
        {
            try
            {
                IList<SaleDto> d = _saleService.GetByOrderNo(orderID);
                return Ok(d);
            }
            catch (OrderNoIsNullException ex)
            {
                //todo 增加处理方式
                // _logger.WriteError(ex.Message);
                return NotFound();
            }
            catch (Exception e)
            {
                // _logger.WriteError(e.Message);
                return InternalServerError();
            }
        }

        #endregion
    }
}