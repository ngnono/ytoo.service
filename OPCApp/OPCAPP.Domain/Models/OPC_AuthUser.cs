using System;
using System.ComponentModel.DataAnnotations;

namespace OPCApp.Domain.Models
{
    public class OPC_AuthUser
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "�û�������Ϊ�գ�")]
        public int? SectionId { get; set; }
        [Required(ErrorMessage = "��½������Ϊ�գ�")]
        public string LogonName { get; set; }
        [Required(ErrorMessage = "���벻��Ϊ�գ�")]
        public string Password { get; set; }
        public string Phone { get; set; }
        public bool? IsValid { get; set; }
        public int? OrgId { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateUserId { get; set; }
        public DateTime UpdateDate { get; set; }
        public int UpdateUserId { get; set; }
        public bool IsSelected { get; set; }/*for mulit selected*/
    }
}