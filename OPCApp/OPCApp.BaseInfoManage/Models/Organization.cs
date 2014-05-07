using System.Collections.Generic;

namespace Intime.OPC.Modules.Dimension.Models
{
    internal class Organization
    {
        private readonly List<Organization> _childOrg = new List<Organization>();

        public IList<Organization> ChildOrg
        {
            get { return _childOrg; }
        }

        public string OrgName { get; set; }
    }
}