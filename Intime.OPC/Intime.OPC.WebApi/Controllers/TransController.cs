// ***********************************************************************
// Assembly         : 03_Intime.OPC.WebApi
// Author           : Liuyh
// Created          : 03-19-2014 22:06:35
//
// Last Modified By : Liuyh
// Last Modified On : 03-23-2014 18:24:59
// ***********************************************************************
// <copyright file="TransController.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Dto.Custom;
using Intime.OPC.Domain.Enums;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository;
using Intime.OPC.Service;
using Intime.OPC.Service.Map;
using Intime.OPC.WebApi.Bindings;
using Intime.OPC.WebApi.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.Http;
using YintaiHZhouContext = Intime.OPC.Domain.Models.YintaiHZhouContext;

namespace Intime.OPC.WebApi.Controllers
{
    /// <summary>
    ///     账户相关接口
    /// </summary>
    public class TransController : BaseController
    {
        /// <summary>
        ///     The _trans service
        /// </summary>
        private readonly ITransService _transService;

        private ISaleRMAService _saleRmaService;
        private IRmaService _rmaService;
        IEnumService _enumService;
        private IPaymentMethodRepository _paymentMethodRepository;
        /// <summary>
        ///     Initializes a new instance of the <see cref="TransController" /> class.
        /// </summary>
        /// <param name="transService">The trans service.</param>
        public TransController(ITransService transService, IEnumService enumService, ISaleRMAService saleRmaService, IRmaService rmaService, IPaymentMethodRepository paymentMethodRepository)
        {
            _transService = transService;
            _enumService = enumService;
            _saleRmaService = saleRmaService;
            _rmaService = rmaService;
            _paymentMethodRepository = paymentMethodRepository;
        }

        /// <summary>
        ///     查询快递单信息
        /// </summary>
        /// <param name="saleNo">销售单编号</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult GetShippingSaleBySaleOrderNo(string saleOrderNo, [UserId] int uid)
        {
            using (var db = new YintaiHZhouContext())
            {
                var saleOrder = db.OPC_Sale.FirstOrDefault(x => x.SaleOrderNo == saleOrderNo);
                if (saleOrder == null)
                {
                    return Error("不存在的销售单");
                }

                var shippingSale =
                    db.OPC_ShippingSale.FirstOrDefault(
                        x => x.OrderNo == saleOrder.OrderNo && x.ShippingCode == saleOrder.ShippingCode && x.SaleOrderNo == saleOrderNo);

                if (shippingSale == null)
                {
                    return Error("未生成发货单");
                }

                return Ok(Mapper.Map<OPC_ShippingSale, ShippingSaleDto>(shippingSale));
            }
        }

        /// <summary>
        /// 获得销售单数据
        /// </summary>
        /// <param name="shippingSaleNo">快递单编号</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult GetSaleByShippingSaleNo(string shippingSaleNo, [UserId] int uid)
        {
            return DoFunction(() =>
            {
                _transService.UserId = uid;
                return _transService.GetSaleByShippingSaleNo(shippingSaleNo);
            }, "查询销售单信息失败");
        }

        //ddd
        /// <summary>
        /// Gets the shipping sale.
        /// </summary>
        /// <param name="orderNo">订单号</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult GetShippingSale(string orderNo, DateTime startDate, DateTime endDate, int pageIndex, int pageSize = 20, [UserId] int uid = 0)
        {

            return DoFunction(() =>
            {
                _transService.UserId = uid;
                return _transService.GetShippingSale(orderNo, startDate, endDate, pageIndex, pageSize);
            },
                "查询快递单信息失败");
        }

        /// <summary>
        /// 获得发货单数据
        /// </summary>
        /// <param name="orderNo">The order no.</param>
        /// <param name="expressNo">The express no.</param>
        /// <param name="startGoodsOutDate">The start goods out date.</param>
        /// <param name="endGoodsOutDate">The end goods out date.</param>
        /// <param name="outGoodsCode">The out goods code.</param>
        /// <param name="sectionId">The section identifier.</param>
        /// <param name="shippingStatus">The shipping status.</param>
        /// <param name="customerPhone">The customer phone.</param>
        /// <param name="brandId">The brand identifier.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult GetShipping(string orderNo,
            string expressNo,
            DateTime startGoodsOutDate,
            DateTime endGoodsOutDate,
            string outGoodsCode,
            int storeId,
            int shippingStatus,
            string customerPhone,
            int brandId,
            int pageIndex, int pageSize, [UserId] int uid)
        {
            try
            {
                if (pageSize <= 0 || pageSize > 100)
                {
                    pageSize = 20;
                }
                _transService.UserId = uid;
                var lst = _transService.GetShippingSale(orderNo, expressNo, startGoodsOutDate, endGoodsOutDate,
                    outGoodsCode, storeId, shippingStatus, customerPhone, brandId, pageIndex, pageSize);

                return Ok(lst);
            }
            catch (Exception ex)
            {
                GetLog().Error(ex);
                return InternalServerError();
            }
        }

        /// <summary>
        ///     创建发货单
        /// </summary>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult CreateShippingSale([FromBody]ShippingSaleCreateDto shippingSaleDto, [UserId] int uid)
        {
            try
            {
                //未做用户权限校验 需要判断订单所属门店是否是当前用户所在门店
                using (var db = new YintaiHZhouContext())
                {
                    using (var ts = new TransactionScope())
                    {
                        var order = db.Orders.FirstOrDefault(x => x.OrderNo == shippingSaleDto.OrderNo);
                        foreach (var saleOrderNo in shippingSaleDto.SaleOrderIDs)
                        {
                            if (order == null)
                            {
                                return Error("无效的订单号");
                            }
                            var saleOrder = db.OPC_Sale.FirstOrDefault(x => x.SaleOrderNo == saleOrderNo);
                            if (saleOrder == null)
                            {
                                continue;
                            }
                            var section = db.Sections.FirstOrDefault(x => x.Id == saleOrder.SectionId);
                            if (section == null)
                            {
                                return Error("订单所属门店错误");
                            }

                            var shippingSale =
                                db.OPC_ShippingSale.FirstOrDefault(
                                    x => x.OrderNo == saleOrder.OrderNo && x.SaleOrderNo == saleOrder.SaleOrderNo);
                            if (shippingSale != null)
                            {
                                shippingSale.ShippingCode = shippingSaleDto.ShippingCode;
                                shippingSale.ShipViaName = shippingSaleDto.ShipViaName;
                                shippingSale.ShipViaId = shippingSaleDto.ShipViaID;
                            }
                            else
                            {
                                shippingSale = db.OPC_ShippingSale.Add(new OPC_ShippingSale()
                                {
                                    CreateDate = DateTime.Now,
                                    CreateUser = uid,
                                    UpdateDate = DateTime.Now,
                                    UpdateUser = uid,
                                    OrderNo = saleOrder.OrderNo,
                                    SaleOrderNo = saleOrder.SaleOrderNo,
                                    ShipViaId = shippingSaleDto.ShipViaID,
                                    ShippingCode = shippingSaleDto.ShippingCode,
                                    ShippingFee = (decimal) (shippingSaleDto.ShippingFee),
                                    ShippingStatus = EnumSaleOrderStatus.PrintInvoice.AsID(),
                                    ShipViaName = shippingSaleDto.ShipViaName,
                                    ShippingAddress = order.ShippingAddress,
                                    ShippingContactPerson = order.ShippingContactPerson,
                                    ShippingContactPhone = order.ShippingContactPhone,
                                    StoreId = section.StoreId,
                                });
                            }

                            saleOrder.ShipViaId = shippingSaleDto.ShipViaID;
                            saleOrder.ShippingCode = shippingSaleDto.ShippingCode;
                            saleOrder.ShippingStatus = shippingSale.ShippingStatus;
                            saleOrder.UpdatedDate = DateTime.Now;
                            saleOrder.UpdatedUser = uid;
                            saleOrder.Status = (int) EnumSaleOrderStatus.PrintExpress;
                        }
                        db.SaveChanges();
                        ts.Complete();
                    }
                }
                return this.Ok();
            }
            catch (Exception ex)
            {
                GetLog().Error(ex);
                return Error(ex.Message);
            }

        }

        #region 包裹签收

        //PackageReceiveDto

        /// <summary>
        /// 查询退货收货单信息    退货包裹管理-包裹签收
        /// </summary>
        /// <param name="dto">The dto.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult GetSaleRmaByPack([FromUri]PackageReceiveRequest dto, [UserId] int uid = 0)
        {
            return DoFunction(() =>
            {
                _saleRmaService.UserId = uid;
                return _saleRmaService.GetByPack(dto);
            }, "查询退货收货单信息失败！");
        }

        /// <summary>
        /// 查询退货单
        /// </summary>
        /// <param name="dto">The dto.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult GetRmaByPack([FromUri]PackageReceiveRequest dto, [UserId] int uid)
        {
            _rmaService.UserId = uid;
            return DoFunction(() => { return _rmaService.GetAll(dto); }, "查询退货单失败！");
        }

        #endregion

        #region 备注

        /// <summary>
        ///     增加快递单备注
        /// </summary>
        /// <param name="comment">The comment.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult AddShippingSaleComment([FromBody]OPC_ShippingSaleComment comment, [UserId] int uid)
        {
            return base.DoFunction(() =>
            {
                _transService.UserId = uid;
                comment.CreateDate = DateTime.Now;
                comment.CreateUser = uid;
                comment.UpdateDate = comment.CreateDate;
                comment.UpdateUser = comment.CreateUser;

                return _transService.AddShippingSaleComment(comment);
            }, "增加快递单备注失败！");
        }

        /// <summary>
        ///     根据订单编号读取快递单备注
        /// </summary>
        /// <param name="orderNo">The order no.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult GetShippingSaleCommentByShippingSaleNo(string shippingSaleNo, [UserId] int uid)
        {
            return DoFunction(() =>
            {
                UserId = uid;
                return _transService.GetByShippingCommentCode(shippingSaleNo);
            }, "读取快递单备注失败！");
        }

        #endregion

        [HttpPost]
        public IHttpActionResult GetPayTypeEnums()
        {
            return DoFunction(() =>
            {
                var lst = _paymentMethodRepository.GetAll(1, 1000);
                var lstDto = new List<Item>();
                foreach (var t in lst.Result)
                {
                    var ii = new Item();
                    ii.Key = t.Code;
                    ii.Value = t.Name;
                    lstDto.Add(ii);
                }
                return lstDto;
            });
        }


        [HttpPost]
        public IHttpActionResult GetShippingTypeEnums()
        {

            return DoFunction(() => { return _enumService.All("ShippingType"); }, "读取发货方式失败！");
        }

        [HttpPost]
        public IHttpActionResult GetFinancialEnums()
        {
            return DoFunction(() =>
            {
                return _enumService.All("Financial");
            }, "读取类型失败！");
        }


        [HttpPost]
        public IHttpActionResult GetOrderStatusEnums()
        {
            return DoFunction(() =>
            {
                return _enumService.All(typeof(EnumOderStatus));
                // return _enumService.All("OrderStatus");
            });
        }
        /// <summary>
        /// 获得销售单类型
        /// </summary>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult GetSaleStatusEnums()
        {
            return DoFunction(() =>
            {
                return _enumService.All(typeof(EnumSaleStatus));
                // return _enumService.All("SaleStatus");
            }, "读取销售单类型失败！");
        }

        /// <summary>
        /// 获得退货单类型
        /// </summary>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult GetRmaStatusEnums()
        {
            return DoFunction(() =>
            {
                return _enumService.All(typeof(EnumRMAStatus));
            }, "读取退货单类型失败！");
        }

        #region 退货入收银

        /// <summary>
        /// 查询 退货单
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult GetRmaCashByExpress([FromUri] RmaExpressRequest request, [UserId] int uid)
        {
            return DoFunction(() =>
            {
                _rmaService.UserId = uid;
                return _rmaService.GetRmaCashByExpress(request);
            }, "查询退货单信息失败");
        }


        /// <summary>
        /// 入收银
        /// </summary>
        /// <param name="rmaNos">The rma nos.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult SetRmaCash([FromBody]IEnumerable<string> rmaNos, [UserId] int uid)
        {
            return DoAction(() =>
            {
                _rmaService.UserId = uid;
                foreach (var rmaNo in rmaNos)
                {
                    _rmaService.SetRmaCash(rmaNo);
                }

            }, "查询退货单信息失败");
        }

        /// <summary>
        /// 完成入收银
        /// </summary>
        /// <param name="rmaNos">The rma nos.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult SetRmaCashOver([FromBody]IEnumerable<string> rmaNos, [UserId] int uid)
        {
            return DoAction(() =>
            {
                _rmaService.UserId = uid;
                foreach (var rmaNo in rmaNos)
                {
                    _rmaService.SetRmaCashOver(rmaNo);
                }

            }, "查询退货单信息失败");
        }

        #endregion


        #region 退货入库

        /// <summary>
        /// 查询 退货单
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult GetRmaReturnByExpress([FromUri] RmaExpressRequest request, [UserId] int uid)
        {
            return DoFunction(() =>
            {
                _rmaService.UserId = uid;
                return _rmaService.GetRmaReturnByExpress(request);
            }, "查询退货单信息失败");
        }


        /// <summary>
        /// 退货入库
        /// </summary>
        /// <param name="rmaNos">The rma nos.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult SetRmaShipInStorage([FromBody]IEnumerable<string> rmaNos, [UserId] int uid)
        {
            return DoAction(() =>
            {
                _rmaService.UserId = uid;
                foreach (var rmaNo in rmaNos)
                {
                    _rmaService.SetRmaShipInStorage(rmaNo);
                }

            }, "查询退货单信息失败");
        }

        #endregion

        #region  打印退货单

        /// <summary>
        /// 查询 退货单
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>IHttpActionResult.</returns>
        [HttpPost]
        public IHttpActionResult GetRmaPrintByExpress([FromUri] RmaExpressRequest request, [UserId] int uid)
        {
            return DoFunction(() =>
            {
                _rmaService.UserId = uid;
                return _rmaService.GetRmaPrintByExpress(request);
            }, "查询退货单信息失败");
        }

        [HttpPost]
        public IHttpActionResult SetRmaPint([FromBody]IEnumerable<string> rmaNos, [UserId] int uid)
        {
            return DoAction(() =>
            {
                _rmaService.UserId = uid;
                foreach (var rmaNo in rmaNos)
                {
                    _rmaService.SetRmaPint(rmaNo);
                }

            }, "查询退货单信息失败");
        }
        #endregion
    }
}