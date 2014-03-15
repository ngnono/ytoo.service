using System;

namespace OPCApp.Domain.Models
{
    public class OPC_AuthRoleUser
    {
        public int Id { get; set; }
        public int OPC_AuthUserId { get; set; }
        public int OPC_AuthRoleId { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateUserId { get; set; }
        public DateTime UpdateDate { get; set; }
        public int UpdateUserId { get; set; }
    }
}