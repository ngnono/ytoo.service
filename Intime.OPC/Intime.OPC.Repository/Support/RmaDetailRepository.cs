using System.Collections.Generic;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository.Base;

namespace Intime.OPC.Repository.Support
{
    public class RmaDetailRepository : BaseRepository<OPC_RMADetail>, IRmaDetailRepository
    {
        public IList<OPC_RMADetail> GetByRmaNo(string rmaNo)
        {
            return Select(t => t.RMANo == rmaNo);
        }
    }
}