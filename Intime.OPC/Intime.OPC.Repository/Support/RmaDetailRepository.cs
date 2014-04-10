using System.Collections.Generic;
using System.Linq;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository.Base;

namespace Intime.OPC.Repository.Support
{
    public class RmaDetailRepository : BaseRepository<OPC_RMADetail>, IRmaDetailRepository
    {
        public IList<RmaDetail> GetByRmaNo(string rmaNo)
        {
            using (var db = new YintaiHZhouContext())
            {
                return null;
            }
        }
    }
}