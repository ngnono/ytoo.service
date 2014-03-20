using System;
using System.ComponentModel.DataAnnotations;

namespace OPCApp.Domain.Models
{
    public class OPC_AuthUser
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "用户名不能为空！")]
        public int? SectionId { get; set; }
        [Required(ErrorMessage = "登陆名不能为空！")]
        public string LogonName { get; set; }
        [Required(ErrorMessage = "密码不能为空！")]
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