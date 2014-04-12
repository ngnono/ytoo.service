using System.Collections.Generic;
using System.Linq;
using Intime.OPC.Domain;
using Intime.OPC.Domain.Dto;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository.Base;

namespace Intime.OPC.Repository.Support
{
    public class RmaDetailRepository : BaseRepository<OPC_RMADetail>, IRmaDetailRepository
    {
        public PageResult<RmaDetail> GetByRmaNo(string rmaNo,int pageIndex,int pageSize)
        {
            //todo 为实现
            using (var db = new YintaiHZhouContext())
            {
                return null;
            }
        }
    }
}