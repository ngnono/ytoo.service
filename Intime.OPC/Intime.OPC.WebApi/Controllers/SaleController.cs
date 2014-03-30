using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Results;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Exception;
using Intime.OPC.Domain.Models;
using Intime.OPC.Service;
using Intime.OPC.WebApi.Bindings;
using Intime.OPC.WebApi.Core;

namespace Intime.OPC.WebApi.Controllers
{
    /// <summary>
    ///     Sale相关接口
    /// </summary>
    public class SaleController : BaseController
    {
        private readonly ISaleService _saleService;
        private readonly IShippingSaleService _shippingSaleService;

        public SaleController(ISaleService saleService, IShippingSaleService shippingSaleService)
        {
            _saleService = saleService;
            _shippingSaleService = shippingSaleService;
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
            return DoFunction(() => { return _saleService.GetSaleOrderDetails(saleOrderNo, userId); }, "读取销售单详情失败");
        }

        [HttpPost]
        public IHttpActionResult WriteSaleRemark(OPC_SaleComment comment, [UserId] int userId)
        {
            return DoFunction(() =>
            {
                comment.CreateDate = DateTime.Now;
                comment.CreateUser = userId;
                comment.UpdateDate = DateTime.Now;
                comment.UpdateUser = userId;
                return _saleService.WriteSaleRemark(comment);
            }, "添加销售单备注失败");
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
        public IHttpActionResult SetSaleOrderPrintSale(IEnumerable<string> saleOrderNos)
        {
            return base.DoFunction(() =>
            {
                int userId = GetCurrentUserID();
                foreach (string saleOrderNo in saleOrderNos)
                {
                    _saleService.PrintSale(saleOrderNo, userId);
                }
                return Ok();
            }, "打印销售单失败");
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
                    //return new StatusCodeResult(HttpStatusCode.BadRequest,new HttpRequestMessage());
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
                    GetLog().Error(e);
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
        public IHttpActionResult SetSaleOrderPrintExpress(string shippingCode)
        {
            IList<OPC_ShippingSale> lst = _shippingSaleService.GetByShippingCode(shippingCode);
            if (lst == null || lst.Count == 0)
            {
                return BadRequest("发货单不存在");
            }
            IEnumerable<string> saleOrderNos = lst.Select(t => t.SaleOrderNo).Distinct();
            int userId = base.GetCurrentUserID();
            foreach (string orderNo in saleOrderNos)
            {
                try
                {
                    _saleService.PrintExpress(orderNo, userId);
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
        public IHttpActionResult SetSaleOrderShipped(IEnumerable<string> saleOrderNos)
        {
            return base.DoAction(() =>
            {
                int userId = GetCurrentUserID();
                foreach (string orderNo in saleOrderNos)
                {
                    _saleService.Shipped(orderNo, userId);
                }
            }, "设置已发货状态失败！");
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
            string orderNo)
        {
            return base.DoFunction(() =>
            {
                int userId = GetCurrentUserID();
                return _saleService.GetNoPickUp(saleOrderNo, userId, orderNo, startDate, endDate);
            }, "读取未提货数据失败");
        }

        /// <summary>
        ///     获得已完成 打印销售单 的数据
        /// </summary>
        /// <param name="saleId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetSalePrintSale(DateTime startDate, DateTime endDate, string saleOrderNo,
            string orderNo,
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
        public IHttpActionResult GetSalePrintInvoice(DateTime startDate, DateTime endDate, string saleOrderNo,
            string orderNo)
        {
            return base.DoFunction(() =>
            {
                int userId = GetCurrentUserID();
                return _saleService.GetPrintInvoice(saleOrderNo, userId, orderNo, startDate, endDate);
            }, "读取打印发货单数据失败");
        }

        /// <summary>
        ///     获得已完成 打印快递单 的数据
        /// </summary>
        /// <param name="saleId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetSalePrintExpress(DateTime startDate, DateTime endDate, string saleOrderNo,
            string orderNo)
        {
            return base.DoFunction(() =>
            {
                int userId = GetCurrentUserID();
                return _saleService.GetPrintExpress(saleOrderNo, userId, orderNo, startDate, endDate);
            }, "读取打印快递单数据失败");
        }

        /// <summary>
        ///     获得已完成 物流入库 的数据
        /// </summary>
        /// <param name="saleId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetSaleShipInStorage(DateTime startDate, DateTime endDate, string saleOrderNo,
            string orderNo)
        {
            return base.DoFunction(() =>
            {
                int userId = GetCurrentUserID();
                return _saleService.GetShipInStorage(saleOrderNo, userId, orderNo, startDate, endDate);
            }, "读取物流入库数据失败");
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