using System.Collections.Generic;
using System.Linq;
using Intime.OPC.Domain;
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

        public PageResult<OPC_OrgInfo> GetAll(int pageIndex, int pageSize)
        {
            return Select2<OPC_OrgInfo, string>(t => t.IsDel == false, o => o.OrgID, true, pageIndex, pageSize);
        }


        public OPC_OrgInfo Add(OPC_OrgInfo orgInfo)
        {
            using (var db = new YintaiHZhouContext())
            {
                if (orgInfo != null)
                {

                    var lst=  db.OrgInfos.Where(t => t.ParentID == orgInfo.ParentID).OrderBy(t=>t.OrgID);
                    var e = lst.LastOrDefault();
                    if (e == null)
                    {
                        orgInfo.OrgID = orgInfo.ParentID + "001";
                    }
                    else
                    {
                        int d = int.Parse(orgInfo.OrgID);
                        orgInfo.OrgID = (d + 1).ToString();
                    }

                    var a=  db.OrgInfos.Add(orgInfo);
                    db.SaveChanges();
                    return a;
                }
                return null;
            }
        }
    }
}