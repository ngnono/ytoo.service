using System;

namespace OPCApp.Domain.Models
{
    public class OPC_AuthMenu
    {
        public int Id { get; set; }
        public bool IsSelected { get; set; }
        public string MenuName { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateUserId { get; set; }
        public DateTime UpdateDate { get; set; }
        public int UpdateUserId { get; set; }
    }
}