using Intime.OPC.Domain.Base;

namespace Intime.OPC.Domain.Models
{
    public class OPC_OrgInfo : IEntity
    {
        public string OrgName { get; set; }
        public string ParentID { get; set; }
        public int? StoreID { get; set; }
        public string StoreName { get; set; }
        public int? OrgType { get; set; }
        public bool? IsDel { get; set; }

        #region IEntity Members

        public int Id { get; set; }

        #endregion
    }
}