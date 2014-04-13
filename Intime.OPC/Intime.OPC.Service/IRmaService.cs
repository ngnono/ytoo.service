using System.Collections.Generic;
using Intime.OPC.Domain;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Dto.Custom;
using Intime.OPC.Domain.Enums;
using Intime.OPC.Domain.Models;

namespace Intime.OPC.Service
{
    public interface IRmaService : IService
    {
        PageResult<RMADto> GetAll(PackageReceiveRequest dto);

        /// <summary>
        ///     获得退货单详情
        /// </summary>
        /// <param name="rmaNo">The rma no.</param>
        /// <returns>IList{RmaDetail}.</returns>
        PageResult<RmaDetail> GetDetails(string rmaNo,int pageIndex,int pageSize);

        /// <summary>
        /// Gets the by order no.
        /// </summary>
        /// <param name="orderNo">The order no.</param>
        /// <param name="rmaStatus">退货单状态</param>
        /// <param name="returnGoodsStatus">退货状态</param>
        /// <returns>IList{RMADto}.</returns>
        PageResult<RMADto> GetByOrderNo(string orderNo, EnumRMAStatus rmaStatus, EnumReturnGoodsStatus returnGoodsStatus, int pageIndex, int pageSize);

        PageResult<RMADto> GetByRmaNo(string rmaNo);



        void AddComment(OPC_RMAComment comment);

        IList<OPC_RMAComment> GetCommentByRmaNo(string rmaNo);

        /// <summary>
        /// 退货包裹审核 查询退货单
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>PageResult{RMADto}.</returns>
        PageResult<RMADto> GetAllPackVerify(PackageReceiveRequest request);

        PageResult<RMADto> GetByFinaceDto(FinaceRequest request);
        /// <summary>
        /// 包裹退回-打印快递单
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>PageResult{RMADto}.</returns>
        PageResult<RMADto> GetRmaByPackPrintPress(RmaExpressRequest request);



        /// <summary>
        /// 退货入收银  查询 退货单
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>PageResult{RMADto}.</returns>
        PageResult<RMADto> GetRmaCashByExpress(RmaExpressRequest request);

        /// <summary>
        /// 退货入收银
        /// </summary>
        /// <param name="rmaNo">The rma no.</param>
        void SetRmaCash(string rmaNo);

        void SetRmaCashOver(string rmaNo);
    }
}