using System;
using System.ComponentModel.DataAnnotations;

namespace Intime.OPC.WebApi.Models
{
    public class LoginModel
    {
        [Required]
        public String UserName { get; set; }

        [Required]
        public String Password { get; set; }
    }
}