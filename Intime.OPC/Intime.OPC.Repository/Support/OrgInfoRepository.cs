using System.Collections.Generic;
using Intime.OPC.Domain.Models;
using Intime.OPC.Repository.Base;

namespace Intime.OPC.Repository.Support
{
    public class OrgInfoRepository : BaseRepository<OPC_OrgInfo>, IOrgInfoRepository
    {
        public IList<OPC_OrgInfo> GetByOrgType(string orgid, int orgtype)
        {
           return  Select(t => t.OrgID.StartsWith(orgid) && t.OrgType == orgtype);
        }
    }
}