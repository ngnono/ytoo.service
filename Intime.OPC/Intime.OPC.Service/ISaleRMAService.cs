
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
    }
}
