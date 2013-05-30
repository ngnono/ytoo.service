using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yintai.Hangzhou.Cms.WebSiteV1.Models
{
    public class ChangePassViewModel:BaseViewModel
    {
        [Key]
        [Display(Name = "用户代码")]
        public int Id { get; set; }

        [Required]
        [StringLength(1024, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "旧密码")]
        public string Password { get; set; }

        [Required]
        [StringLength(1024, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "新密码")]
        public string NewPassword { get; set; }

        [Compare("NewPassword",ErrorMessage="密码不一致")]
        [DataType(DataType.Password)]      
        [Display(Name = "再次输入新密码")]
        public string ConfirmPassword { get; set; }
    }
}
