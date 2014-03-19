using System;

namespace Intime.OPC.Domain.Models
{
    public class OPC_AuthUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? SectionId { get; set; }
        public string LogonName { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public bool? IsValid { get; set; }
        public int? OrgId { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateUserId { get; set; }
        public DateTime UpdateDate { get; set; }
        public int UpdateUserId { get; set; }
    }
}