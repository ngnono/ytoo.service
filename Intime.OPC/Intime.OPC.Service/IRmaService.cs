using System.Collections.Generic;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Dto.Custom;
using Intime.OPC.Domain.Enums;
using Intime.OPC.Domain.Models;

namespace Intime.OPC.Service
{
    public interface IRmaService : IService
    {
        IList<RMADto> GetAll(PackageReceiveDto dto);

        /// <summary>
        ///     获得退货单详情
        /// </summary>
        /// <param name="rmaNo">The rma no.</param>
        /// <returns>IList{RmaDetail}.</returns>
        IList<RmaDetail> GetDetails(string rmaNo);

        /// <summary>
        /// Gets the by order no.
        /// </summary>
        /// <param name="orderNo">The order no.</param>
        /// <param name="rmaStatus">退货单状态</param>
        /// <param name="returnGoodsStatus">退货状态</param>
        /// <returns>IList{RMADto}.</returns>
        IList<RMADto> GetByOrderNo(string orderNo,EnumRMAStatus rmaStatus,EnumReturnGoodsStatus returnGoodsStatus);

        void AddComment(OPC_RMAComment comment);

        IList<OPC_RMAComment> GetCommentByRmaNo(string rmaNo);

        IList<RMADto> GetAllPackVerify(PackageReceiveDto request);
    }
}