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

        void AddComment(OPC_RMAComment comment);

        IList<OPC_RMAComment> GetCommentByRmaNo(string rmaNo);

        PageResult<RMADto> GetAllPackVerify(PackageReceiveRequest request);
    }
}