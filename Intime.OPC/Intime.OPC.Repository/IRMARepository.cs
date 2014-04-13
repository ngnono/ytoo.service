using System;
using System.Collections.Generic;
using Intime.OPC.Domain;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Dto.Custom;
using Intime.OPC.Domain.Enums;
using Intime.OPC.Domain.Models;

namespace Intime.OPC.Repository
{
    public interface IRMARepository : IRepository<OPC_RMA>
    {
        PageResult<OPC_RMA> GetByReturnGoods(ReturnGoodsInfoRequest request);

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <param name="orderNo">The order no.</param>
        /// <param name="saleOrderNo">The sale order no.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="endTime">The end time.</param>
        /// <param name="rmaStatus">退货单状态</param>
        /// <param name="returnGoodsStatus">退货状态</param>
        /// <returns>IList{RMADto}.</returns>
        PageResult<RMADto> GetAll(string orderNo, string saleOrderNo, DateTime startTime, DateTime endTime, int? rmaStatus, string returnGoodsStatus,int pageIndex,int pageSize);

        PageResult<RMADto> GetByRmaNo(string rmaNo);

        OPC_RMA GetByRmaNo2(string rmaNo);
        PageResult<RMADto> GetByPackPrintPress(string orderNo, string saleOrderNo, DateTime dateTime1, DateTime dateTime2, int? rmaStatus, int pageIndex, int pageSize);
    }
}