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
           return  Select(t => t.OrgID.StartsWith(orgid) && t.OrgType == orgtype && t.StoreOrSectionID.HasValue);
        }

        public bool Create(OPC_OrgInfo entity)
        {
            entity.IsDel = false;
            return base.Create(entity);
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

                    var lst=  db.OPC_OrgInfos.Where(t => t.ParentID == orgInfo.ParentID).OrderByDescending(t=>t.OrgID);
                    var e = lst.FirstOrDefault();
                    if (e == null)
                    {
                        orgInfo.OrgID = orgInfo.ParentID + "001";
                    }
                    else
                    {
                        int d = int.Parse(e.OrgID);
                        orgInfo.OrgID = (d + 1).ToString();
                    }
                    orgInfo.IsDel = false;
                    var a = db.OPC_OrgInfos.Add(orgInfo);
                    db.SaveChanges();
                    return a;
                }
                return null;
            }
        }


        public OPC_OrgInfo GetByOrgID(string orgID)
        {
            return Select(t => t.OrgID == orgID).FirstOrDefault();
        }
    }
}