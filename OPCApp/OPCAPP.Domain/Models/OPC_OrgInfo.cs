
using System.ComponentModel.DataAnnotations;

namespace OPCApp.Domain.Models
{
    public class OPC_OrgInfo
    {
        public string OrgID { get; set; }
          [Required(ErrorMessage = "组织机构名称不能为空！")]
        public string OrgName { get; set; }
        public string ParentID { get; set; }
        public int? StoreOrSectionID { get; set; }
        public string StoreOrSectionName { get; set; }
        public int? OrgType { get; set; }
        public bool? IsDel { get; set; }



        #region IEntity Members

        public int Id { get; set; }

        #endregion
    }
}