using System.Collections.Generic;
using Intime.OPC.Domain.Dto.Custom;
using Intime.OPC.Domain.Models;

namespace Intime.OPC.Repository
{
    public interface IRMARepository : IRepository<OPC_RMA>
    {
        IList<OPC_RMA> GetByReturnGoods(ReturnGoodsInfoGet request);
    }
}