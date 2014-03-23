using System;

namespace OPCApp.Domain.Models
{
    public class OPC_AuthMenu
    {
        public int Id { get; set; }
        public bool IsSelected { get; set; }/*for mulit selected*/
        public string MenuName { get; set; }
        public int PraentMenuId { get; set; }
        public int Sort { get; set; }
        public string Url { get; set; }
      
    }
}