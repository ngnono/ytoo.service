using System.Collections.Generic;
using Intime.OPC.Domain.Dto.Custom;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository.Base;

namespace Intime.OPC.Repository.Support
{
    public class RMARepository : BaseRepository<OPC_RMA>, IRMARepository
    {
        public IList<OPC_RMA> GetByReturnGoods(ReturnGoodsInfoGet request)
        {

            return null;

            //using (var db = new YintaiHZhouContext())
            //{
                
            //}
        }
    }
}