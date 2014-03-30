using System;
using System.Collections.Generic;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Models;

namespace Intime.OPC.Service
{
    public interface ITransService : IService
    {
        bool Finish(Dictionary<string, string> sale);
        IList<OPC_Sale> Select(string startDate, string endDate, string orderNo, string saleOrderNo);

        IList<OPC_SaleDetail> SelectSaleDetail(IEnumerable<string> saleIDs);

        /// <summary>
        /// 增加订单日志
        /// </summary>
        /// <param name="comment">The comment.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool AddOrderComment(OPC_OrderComment comment);

        /// <summary>
        /// Gets the shipping sale by order no.
        /// </summary>
        /// <param name="orderNo">The order no.</param>
        /// <returns>IList{ShippingSaleDto}.</returns>
        ShippingSaleDto GetShippingSaleBySaleNo(string saleNo);

        /// <summary>
        /// 新增发货单备注
        /// </summary>
        /// <param name="comment">The comment.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool AddShippingSaleComment(OPC_ShippingSaleComment comment);

        /// <summary>
        /// 读取发货单备注
        /// </summary>
        /// <param name="shippingCode">发货单编号</param>
        /// <returns>IList{OPC_ShippingSaleComment}.</returns>
        IList<OPC_ShippingSaleComment> GetByShippingCommentCode(string shippingCode);

        /// <summary>
        /// Creates the shipping sale.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="shippingSaleDto">The shipping sale dto.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool CreateShippingSale(int userId, ShippingSaleCreateDto shippingSaleDto);

        /// <summary>
        /// 查询快递单
        /// </summary>
        /// <param name="shippingCode">The shipping code.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="endTime">The end time.</param>
        /// <returns>IList{ShippingSaleDto}.</returns>
        IList<ShippingSaleDto> GetShippingSale(string shippingCode, System.DateTime startTime, System.DateTime endTime);

        IList<SaleDto> GetSaleByShippingSaleNo(string shippingSaleNo);
        IList<SaleDto> GetSaleOrderPickup(string orderCode, string saleOrderNo, DateTime startDate, DateTime endDate);
    }
}