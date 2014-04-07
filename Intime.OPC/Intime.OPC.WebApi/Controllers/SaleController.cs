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
        public IHttpActionResult GetSaleOrderDetails(string saleOrderNo)
        {
            return DoFunction(() =>
            {
                int userId = GetCurrentUserID();
                return _saleService.GetSaleOrderDetails(saleOrderNo, userId,1,1000).Result;
            }, "读取销售单详情失败");
        }

        [HttpPost]
        public IHttpActionResult WriteSaleRemark([FromBody]OPC_SaleComment comment)
        {
            return DoFunction(() =>
            {
                int userId = GetCurrentUserID();
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
        public IHttpActionResult SetSaleOrderFinishPrintSale([FromBody]IEnumerable<string> saleOrderNos, [UserId] int? userId)
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
        public IHttpActionResult SetSaleOrderPrintSale([FromBody]IEnumerable<string> saleOrderNos)
        {
            return base.DoFunction(() =>
            {
                int userId = GetCurrentUserID();
                foreach (string saleOrderNo in saleOrderNos)
                {
                    _saleService.PrintSale(saleOrderNo, userId);
                }
                return true;
            }, "打印销售单失败");
        }

        /// <summary>
        ///     销售提货
        /// </summary>
        /// <param name="saleOrderNos">The sale order nos.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPut]
        public IHttpActionResult SetSaleOrderPickUp([FromBody]IEnumerable<string> saleOrderNos, [UserId] int? userId)
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
        public IHttpActionResult SetSaleOrderShipInStorage([FromBody]IEnumerable<string> saleOrderNos, [UserId] int? userId)
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
        public IHttpActionResult SetSaleOrderPrintInvoice([FromBody]IEnumerable<string> saleOrderNos, [UserId] int? userId)
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
        public IHttpActionResult SetSaleOrderPrintExpress([FromBody]string shippingCode)
        {
            //todo 增加销售单号
            IList<OPC_ShippingSale> lst = _shippingSaleService.GetByShippingCode(shippingCode,1,10000).Result;
            if (lst == null || lst.Count == 0)
            {
                return BadRequest("发货单不存在");
            }
            var sd = lst.FirstOrDefault();
            var lstSale = _saleService.GetByShippingCode(shippingCode);
            
            int userId = base.GetCurrentUserID();
            foreach (var sale in lstSale)
            {
                try
                {
                    _saleService.PrintExpress(sale.SaleOrderNo, userId);
                    _shippingSaleService.PrintExpress(sale.SaleOrderNo, userId);
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
        /// 已发货
        /// </summary>
        /// <param name="shippingCodes">发货单编码</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPut]
        public IHttpActionResult SetSaleOrderShipped([FromBody]IEnumerable<string> shippingCodes)
        {
            return base.DoAction(() =>
            {
                int userId = GetCurrentUserID();
                foreach (var shippingCode in shippingCodes)
                {
                    var lstSales = _saleService.GetByShippingCode(shippingCode);
                    foreach (var sale in lstSales)
                    {
                        _saleService.Shipped(sale.SaleOrderNo, userId);
                        _shippingSaleService.Shipped(sale.SaleOrderNo, userId);
                    }
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
        public IHttpActionResult SetSaleOrderStockOut([FromBody] IEnumerable<string> saleOrderNos, [UserId] int? userId)
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
            string orderNo, int pageIndex, int pageSize)
        {
            return base.DoFunction(() =>
            {
                int userId = GetCurrentUserID();
                return _saleService.GetNoPickUp(saleOrderNo, userId, orderNo, startDate, endDate, pageIndex, pageSize);
            }, "读取未提货数据失败");
        }

        [HttpGet]
        public IHttpActionResult GetSalePickup(string orderCode, string saleOrderNo, DateTime startDate, DateTime endDate, int pageIndex, int pageSize)
        {

            return DoFunction(() =>
            {
                var userId = GetCurrentUserID();
                return _saleService.GetPickUp(saleOrderNo, orderCode, startDate, endDate,userId, pageIndex, pageSize);
            },
                "查询快递单信息失败");
        }

        [HttpGet]
        public IHttpActionResult GetShipped(string saleOrderNo, string orderNo,  DateTime startDate, DateTime endDate, int pageIndex, int pageSize)
        {

            return DoFunction(() =>
            {
                var userId = GetCurrentUserID();
                return _saleService.GetShipped(orderNo, userId, saleOrderNo, startDate, endDate, pageIndex, pageSize);
            },
                "查询快递单信息失败");
        }
        /// <summary>
        ///     获得已完成 打印销售单 的数据
        /// </summary>
        /// <param name="saleId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetSalePrintSale(DateTime startDate, DateTime endDate, string saleOrderNo,
            string orderNo, int pageIndex, int pageSize)
        {

            return DoFunction(() =>
            {
                var userId = GetCurrentUserID();
                return _saleService.GetPrintSale(saleOrderNo, userId, orderNo, startDate, endDate, pageIndex, pageSize);
            }, "读取已完成打印销售单的销售单数据失败");
        }

        /// <summary>
        ///     获得已完成 打印发货单 的数据
        /// </summary>
        /// <param name="saleId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetSalePrintInvoice(DateTime startDate, DateTime endDate,
            string orderNo, string saleOrderNo, int pageIndex, int pageSize)
        {
            return base.DoFunction(() =>
            {
                int userId = GetCurrentUserID();
                return _saleService.GetPrintInvoice(saleOrderNo, userId, orderNo, startDate, endDate, pageIndex, pageSize);
            }, "读取已完成打印发货单的销售单数据失败");
        }

        /// <summary>
        ///     获得已完成 打印快递单 的数据
        /// </summary>
        /// <param name="saleId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetSalePrintExpress(DateTime startDate, DateTime endDate, 
            string orderNo,string saleOrderNo, int pageIndex, int pageSize)
        {
            return base.DoFunction(() =>
            {
                int userId = GetCurrentUserID();
                return _saleService.GetPrintExpress(null, userId, orderNo, startDate, endDate, pageIndex, pageSize);
            }, "读取已完成打印快递单的销售单数据失败");
        }

        /// <summary>
        ///     获得已完成 物流入库 的数据
        /// </summary>
        /// <param name="saleId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetSaleShipInStorage(DateTime startDate, DateTime endDate, string saleOrderNo,
            string orderNo, int pageIndex, int pageSize)
        {
            return base.DoFunction(() =>
            {
                int userId = GetCurrentUserID();
                return _saleService.GetShipInStorage(saleOrderNo, userId, orderNo, startDate, endDate, pageIndex, pageSize);
            }, "读取物流入库数据失败");
        }

        [HttpGet]
        public IHttpActionResult GetSaleByOrderNo(string orderID,int pageIndex, int pageSize)
        {
            return DoFunction(() =>
            {
                int userId = GetCurrentUserID();
                return  _saleService.GetByOrderNo(orderID,userId,pageIndex,pageSize);
                
            }, "读取销售单数据失败");
            
        }

        #endregion
    }
}