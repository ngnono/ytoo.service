using System;
using System.Collections.Generic;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Dto.Custom;
using Intime.OPC.Domain.Models;

namespace Intime.OPC.Repository
{
    public interface IRMARepository : IRepository<OPC_RMA>
    {
        IList<OPC_RMA> GetByReturnGoods(ReturnGoodsInfoGet request);

        IList<RMADto> GetAll(string orderNo, string saleOrderNo, DateTime startTime, DateTime endTime);
    }
}