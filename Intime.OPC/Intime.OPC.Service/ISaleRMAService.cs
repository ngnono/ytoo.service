
using System.Collections.Generic;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Dto.Custom;
using Intime.OPC.Domain.Models;

namespace Intime.OPC.Service
{
    public interface ISaleRMAService:IService
    {
        void CreateSaleRMA(int userId, RMAPost rma);

        IList<SaleRmaDto> GetByReturnGoodsInfo(ReturnGoodsInfoGet request);


        IList<SaleRmaDto> GetByReturnGoods(ReturnGoodsGet rquest);


        void AddComment(OPC_SaleRMAComment comment);

        IList<OPC_SaleRMAComment> GetCommentByRmaNo(string rmaNo);
        IList<SaleRmaDto> GetByPack(PackageReceiveDto dto);

        /// <summary>
        /// 客服同意退货
        /// </summary>
        /// <param name="rmaNo">The rma no.</param>
        void AgreeReturnGoods(string rmaNo);

        /// <summary>
        /// 物流收货确认
        /// </summary>
        /// <param name="rmaNo">The rma no.</param>
        void ShippingReceiveGoods(string rmaNo);
    }
}
