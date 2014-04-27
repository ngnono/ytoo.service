using System.Collections.Generic;

namespace OPCApp.BaseInfoManage.Models
{
    internal class OrgInfo
    {
        private readonly List<OrgInfo> _childOrg = new List<OrgInfo>();

        public IList<OrgInfo> ChildOrg
        {
            get { return _childOrg; }
        }

        public string OrgName { get; set; }
    }
}