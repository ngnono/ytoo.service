using System.Web.Http.Results;
using Intime.OPC.Domain;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Dto.Request;
using Intime.OPC.Domain.Exception;
using Intime.OPC.Domain.Models;
using Intime.OPC.Service;
using Intime.OPC.Service.Contract;
using Intime.OPC.WebApi.Bindings;
using Intime.OPC.WebApi.Core;
using Intime.OPC.WebApi.Core.MessageHandlers.AccessToken;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace Intime.OPC.WebApi.Controllers
{
    /// <summary>
    ///     Sale相关接口
    /// </summary>
    public class SaleController : BaseController
    {
        private readonly ISaleService _saleService;
        private readonly IShippingSaleService _shippingSaleService;
        private readonly ISaleOrderService _saleOrderService;
        private log4net.ILog _log = log4net.LogManager.GetLogger(typeof(SaleController));

        public SaleController(ISaleService saleService, IShippingSaleService shippingSaleService, ISaleOrderService saleOrderService)
        {
            _saleService = saleService;
            _shippingSaleService = shippingSaleService;
            _saleOrderService = saleOrderService;
        }

        [HttpPost]
        public IHttpActionResult GetSaleRemarks(string saleId, [UserId] int userId)
        {
            return DoFunction(() => _saleOrderService.GetSaleComments(saleId, userId));
        }

        [HttpPost]
        public IHttpActionResult GetSaleOrderDetails(string saleOrderNo, [UserId] int uid)
        {
            return DoFunction(() =>
            {
                var saleDetails = _saleOrderService.GetSaleDetails(saleOrderNo);
                return new PageResult<SaleDetailDto>(saleDetails, saleDetails.Count).Result;
            });
        }

        [HttpPost]
        public IHttpActionResult WriteSaleRemark([FromBody]OPC_SaleComment comment, [UserId] int uid)
        {
            return DoFunction(() =>
            {
                _saleService.UserId = uid;
                int userId = uid;
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
        public IHttpActionResult SetSaleOrderFinishPrintSale([FromBody]IEnumerable<string> saleOrderNos, [UserId] int userId)
        {
            _saleService.UserId = userId;

            foreach (string orderNo in saleOrderNos)
            {
                try
                {
                    _saleService.FinishPrintSale(orderNo, userId);
                }
                catch (SaleOrderNotExistsException ex)
                {
                    GetLog().Error(ex);
                    return StatusCode(HttpStatusCode.NotFound);
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
        ///     打印销售单
        /// </summary>
        /// <param name="saleOrderNos"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPut]
        public IHttpActionResult SetSaleOrderPrintSale([FromBody]IEnumerable<string> saleOrderNos, [UserId] int uid)
        {
            return base.DoFunction(() =>
            {
                _saleService.UserId = uid;
                foreach (string saleOrderNo in saleOrderNos)
                {
                    _saleService.PrintSale(saleOrderNo, uid);
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
        public IHttpActionResult SetSaleOrderPickUp([FromBody]IEnumerable<string> saleOrderNos, [UserId] int userId)
        {
            _saleService.UserId = userId;

            foreach (string orderNo in saleOrderNos)
            {
                try
                {
                    _saleService.SetSaleOrderPickUp(orderNo, userId);
                }
                catch (SaleOrderNotExistsException ex)
                {
                    GetLog().Error(ex);
                    return StatusCode(HttpStatusCode.NotFound);
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
        ///     物流入库
        /// </summary>
        /// <param name="saleOrderNos">The sale order nos.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPut]
        public IHttpActionResult SetSaleOrderShipInStorage([FromBody]IEnumerable<string> saleOrderNos, [UserId] int userId)
        {
            _saleService.UserId = userId;
            foreach (string orderNo in saleOrderNos)
            {
                try
                {
                    _saleService.SetShipInStorage(orderNo, userId);
                }
                catch (SaleOrderNotExistsException ex)
                {
                    GetLog().Error(ex);
                    return StatusCode(HttpStatusCode.NotFound);
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
        public IHttpActionResult SetSaleOrderPrintInvoice([FromBody]IEnumerable<string> saleOrderNos, [UserId] int userId)
        {
            _saleService.UserId = userId;
            foreach (string orderNo in saleOrderNos)
            {
                try
                {
                    _saleService.PrintInvoice(orderNo, userId);
                }
                catch (SaleOrderNotExistsException ex)
                {
                    GetLog().Error(ex);
                    return StatusCode(HttpStatusCode.NotFound);
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
        ///     打印快递单
        /// </summary>
        /// <param name="saleOrderNos">The sale order nos.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPut]
        public IHttpActionResult SetSaleOrderPrintExpress([FromBody]string shippingCode, [UserId] int uid)
        {
            _saleService.UserId = uid;
            _shippingSaleService.UserId = uid;
            //todo 增加销售单号
            IList<OPC_ShippingSale> lst = _shippingSaleService.GetByShippingCode(shippingCode, 1, 10000).Result;
            if (lst == null || lst.Count == 0)
            {
                return BadRequest("发货单不存在");
            }
            var sd = lst.FirstOrDefault();
            var lstSale = _saleService.GetByShippingCode(shippingCode);

            foreach (var sale in lstSale)
            {
                try
                {
                    _saleService.PrintExpress(sale.SaleOrderNo, uid);
                    _shippingSaleService.PrintExpress(sale.SaleOrderNo, uid);
                }
                catch (SaleOrderNotExistsException ex)
                {
                    GetLog().Error(ex);
                    return StatusCode(HttpStatusCode.NotFound);
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
        /// 已发货
        /// </summary>
        /// <param name="shippingCodes">发货单编码</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPut]
        public IHttpActionResult SetSaleOrderShipped([FromBody]IEnumerable<string> shippingCodes, [UserId] int uid)
        {
            return DoAction(() =>
            {
                _saleService.UserId = uid;
                _shippingSaleService.UserId = uid;
                foreach (var shippingCode in shippingCodes)
                {
                    var lstSales = _saleService.GetByShippingCode(shippingCode);
                    foreach (var sale in lstSales)
                    {
                        _saleService.Shipped(sale.SaleOrderNo, uid);
                        _shippingSaleService.Shipped(sale.SaleOrderNo, uid);
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
        public IHttpActionResult SetSaleOrderStockOut([FromBody] IEnumerable<string> saleOrderNos, [UserId] int userId)
        {
            _saleService.UserId = userId;
            foreach (string orderNo in saleOrderNos)
            {
                try
                {
                    _saleService.StockOut(orderNo, userId);
                }
                catch (SaleOrderNotExistsException ex)
                {
                    return StatusCode(HttpStatusCode.NotFound);
                }
                catch (Exception e)
                {
                    // _logger.WriteError(e.Message);
                    GetLog().Error(e);
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
        [HttpPost]
        public IHttpActionResult GetSaleNoPickUp(DateTime startDate, DateTime endDate, string saleOrderNo,
            string orderNo, int pageIndex, int pageSize, [UserId] int uid)
        {
            return DoFunction(() =>
            {
                _saleService.UserId = uid;
                return _saleService.GetNoPickUp(saleOrderNo, uid, orderNo, startDate, endDate, pageIndex, pageSize);
            }, "读取未提货数据失败");
        }

        [HttpPost]
        public IHttpActionResult GetNoPickupSaleOrders([FromUri]SaleOrderQueryRequest request, [UserId] int uid)
        {
            return DoFunction(() =>
            {
                var saleDtos = _saleOrderService.QuerySaleOrders(request, uid);
                return new PageResult<SaleDto>(saleDtos, saleDtos.Count);
            });
        }

        [HttpPost]
        public IHttpActionResult GetSalePickup(string orderCode, string saleOrderNo, DateTime startDate, DateTime endDate, int pageIndex, int pageSize, [UserId] int uid)
        {
            return DoFunction(() =>
            {
                _saleService.UserId = uid;
                return _saleService.GetPickUp(saleOrderNo, orderCode, startDate, endDate, uid, pageIndex, pageSize);
            },
                "查询快递单信息失败");
        }

        [HttpPost]
        public IHttpActionResult GetShipped(string saleOrderNo, string orderNo, DateTime startDate, DateTime endDate, int pageIndex, int pageSize, [UserId] int uid)
        {
            return DoFunction(() =>
            {
                _saleService.UserId = uid;
                return _saleService.GetShipped(orderNo, uid, saleOrderNo, startDate, endDate, pageIndex, pageSize);
            },
                "查询快递单信息失败");
        }
        /// <summary>
        ///     获得已完成 打印销售单 的数据
        /// </summary>
        /// <param name="saleId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult GetSalePrintSale(DateTime startDate, DateTime endDate, string saleOrderNo,
            string orderNo, int pageIndex, int pageSize, [UserId] int uid)
        {
            return DoFunction(() =>
            {
                _saleService.UserId = uid;
                return _saleService.GetPrintSale(saleOrderNo, uid, orderNo, startDate, endDate, pageIndex, pageSize);
            }, "读取已完成打印销售单的销售单数据失败");
        }

        /// <summary>
        ///     获得已完成 打印发货单 的数据
        /// </summary>
        /// <param name="saleId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult GetSalePrintInvoice(DateTime startDate, DateTime endDate,
            string orderNo, string saleOrderNo, int pageIndex, int pageSize, [UserId] int uid)
        {
            return base.DoFunction(() =>
            {
                _saleService.UserId = uid;
                return _saleService.GetPrintInvoice(saleOrderNo, uid, orderNo, startDate, endDate, pageIndex, pageSize);
            }, "读取已完成打印发货单的销售单数据失败");
        }

        /// <summary>
        ///     获得已完成 打印快递单 的数据
        /// </summary>
        /// <param name="saleId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult GetSalePrintExpress(DateTime startDate, DateTime endDate,
            string orderNo, string saleOrderNo, int pageIndex, int pageSize, [UserId] int uid)
        {
            return base.DoFunction(() =>
            {
                _saleService.UserId = uid;
                return _saleService.GetPrintExpress(null, uid, orderNo, startDate, endDate, pageIndex, pageSize);
            }, "读取已完成打印快递单的销售单数据失败");
        }

        /// <summary>
        ///     获得已完成 物流入库 的数据
        /// </summary>
        /// <param name="saleId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult GetSaleShipInStorage(DateTime startDate, DateTime endDate, string saleOrderNo,
            string orderNo, int pageIndex, int pageSize, [UserId] int uid)
        {
            return base.DoFunction(() =>
            {
                _saleService.UserId = uid;
                return _saleService.GetShipInStorage(saleOrderNo, uid, orderNo, startDate, endDate, pageIndex, pageSize);
            }, "读取物流入库数据失败");
        }

        [HttpPost]
        public IHttpActionResult GetSaleByOrderNo(string orderID, int pageIndex, int pageSize, [UserId] int uid)
        {
            return DoFunction(() =>
            {
                _saleService.UserId = uid;
                return _saleService.GetByOrderNo(orderID, uid, pageIndex, pageSize);

            }, "读取销售单数据失败");

        }

        [Route("api/salesorder")]
        [HttpGet]
        public IHttpActionResult GetList([FromUri]SaleOrderQueryRequest request, [UserId] int uid, [UserProfile] UserProfile userProfile)
        {
            if (request == null)
            {
                request = new SaleOrderQueryRequest();
            }

            if (userProfile.IsSystem)
            {
                request.IsAllStoreIds = true;
            }
            else
            {
                if (userProfile.StoreIds == null || !userProfile.StoreIds.Any())
                {
                    _log.Debug(String.Format("uid:{0}.stores is null", uid.ToString()));
                    return ResultNotFound();
                }

                request.StoreIds = userProfile.StoreIds.ToList();
            }

            //#if DEBUG
            //            //TODO: 测试数据
            //             uid = 1;
            //#endif
            var dto = _saleOrderService.GetPagedList(request, uid);

            return RetrunHttpActionResult(dto);
        }

        [Route("api/salesorder/{orderno}")]
        [HttpGet]
        public IHttpActionResult GetItem(string orderno, [UserId] int uid, [UserProfile] UserProfile userProfile)
        {
            if (userProfile.IsSystem)
            {
            }
            else
            {
                if (userProfile.StoreIds == null || !userProfile.StoreIds.Any())
                {
                    _log.Debug(String.Format("uid:{0}.stores is null", uid.ToString()));
                    return ResultNotFound();
                }
            }

            var dto = _saleOrderService.GetItem(orderno);

            if (!userProfile.IsSystem && userProfile.StoreIds.Contains(dto.StoreId))
            {
                return BadRequest("请先维护您的门店信息后，再查询");
            }

            return RetrunHttpActionResult(dto);
        }

        [Route("api/salesorderdetail")]
        [HttpGet]
        public IHttpActionResult GetSalesOrderDetailBySaleOrderNo([UserProfile] UserProfile userProfile, [FromUri]string saleOrderNo = "", [FromUri] int? pageIndex = 1, [FromUri]int? pagesize = 10)
        {
            if (string.IsNullOrWhiteSpace(saleOrderNo))
            {
                return BadRequest("saleOrderNo为空");
            }

            return Ok(_saleService.GetSalesDetailsBySaleOrderNo(saleOrderNo, userProfile.StoreIds, pageIndex ?? 1, pagesize ?? 10));

        }

        #endregion
    }
}